using System.Collections.ObjectModel;
using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.Miscellaneous;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class ShowFeaturesProgressively
{
    private bool _initialized;
    public ShowFeaturesProgressively()
    {
        InitializeComponent();
    }

    private async void ShapefileLayer_OnSizeChanged(object sender, EventArgs e)
    {
        if (_initialized)
            return;
        _initialized = true;

        // It is important to set the map unit first to either feet, meters or decimal degrees.
        MapView.MapUnit = GeographyUnit.Meter;

        // Create the background world maps using vector tiles requested from the ThinkGeo Cloud Service and add it to the map.
        var backgroundOverlay = new ThinkGeoVectorOverlay
        {
            ClientId = SampleKeys.ClientId,
            ClientSecret = SampleKeys.ClientSecret,
            MapType = ThinkGeoCloudVectorMapsMapType.Light,
            TileCache = new FileRasterTileCache(FileSystem.Current.CacheDirectory, "ThinkGeoVectorLight_RasterCache")
        };
        MapView.Overlays.Add(backgroundOverlay);

        // Create a new overlay that will hold our new layer and add it to the map.
        var parksOverlay = new ProgressiveFeaturesTileOverlay();
        MapView.Overlays.Add(parksOverlay);

        // Create the new layer and set the projection as the data is in srid 2276 and our background is srid 3857 (spherical mercator).
        var parksLayer = new ShapefileProgressiveFeatureLayer(Path.Combine(FileSystem.Current.AppDataDirectory, "Data", "Shapefile", "Parks.shp"))
        {
            FeatureSource =
                {
                    ProjectionConverter = new ProjectionConverter(2276, 3857)
                }
        };

        // Add the layer to the overlay we created earlier.
        parksOverlay.FeatureLayer = parksLayer;
        parksOverlay.DrawingBulkCount = 1;

        // Create an Area style on zoom level 1 and then apply it to all zoom levels up to 20.
        parksLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColors.Green);
        parksLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        // Set the map scale and center point
        MapView.MapScale = 35_000;
        MapView.CenterPoint = new PointShape(-10778209, 3914820);
        await MapView.RefreshAsync();
    }

    class ShapefileProgressiveFeatureLayer : ProgressiveFeatureLayer
    {
        public ShapefileProgressiveFeatureLayer(string shapeFilePath)
        {
            FeatureSource = new ShapeFileFeatureSource(shapeFilePath);
        }
        protected override async IAsyncEnumerable<Collection<Feature>> GetProgressiveFeaturesCore(RectangleShape boundingBox, int bulkSize)
        {
            var projectedBoundingBox = boundingBox;
            if (FeatureSource.ProjectionConverter != null)
                projectedBoundingBox = FeatureSource.ProjectionConverter.ConvertToExternalProjection(boundingBox);

            var featureIds = FeatureSource.GetFeatureIdsInsideBoundingBox(projectedBoundingBox).ToArray();
            int currentIndex = 0;

            while (currentIndex < featureIds.Length)
            {
                int renderIFeatureIds = Math.Min(featureIds.Length - currentIndex, bulkSize);
                string[] renderFeatures = new string[renderIFeatureIds];

                Array.Copy(featureIds, currentIndex, renderFeatures, 0, renderFeatures.Length);
                var features = new Collection<Feature>();
                await Task.Run(() => features = FeatureSource.GetFeaturesByIds(renderFeatures, ReturningColumnsType.NoColumns));
                currentIndex += renderIFeatureIds;

                yield return features;
            }
        }
    }
}