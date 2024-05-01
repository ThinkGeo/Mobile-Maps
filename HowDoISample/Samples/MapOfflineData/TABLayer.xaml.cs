using System.Collections.ObjectModel;
using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.MapOfflineData;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class TabLayer
{
    private bool _initialized;
    public TabLayer()
    {
        InitializeComponent();
    }

    private async void TABLayer_OnSizeChanged(object sender, EventArgs e)
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
        var cityBoundaryOverlay = new LayerOverlay();
        MapView.Overlays.Add(cityBoundaryOverlay);

        // Create the new layer and set the projection as the data is in srid 2276 and our background is srid 3857 (spherical mercator).
        var cityBoundaryLayer = new TabFeatureLayer(Path.Combine(FileSystem.Current.AppDataDirectory, "Data", "Tab", "City_ETJ.tab"))
            {
                FeatureSource =
                {
                    ProjectionConverter = new ProjectionConverter(2276, 3857)
                }
            };

        // Add the layer to the overlay we created earlier.
        cityBoundaryOverlay.Layers.Add("City Boundary", cityBoundaryLayer);

        // Set this so we can use our own styles as opposed to the styles in the file.
        cityBoundaryLayer.StylingType = TabStylingType.StandardStyling;

        // Create an Area style on zoom level 1 and then apply it to all zoom levels up to 20.
        cityBoundaryLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle =
            AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(100, GeoColors.Green), GeoColors.Green);
        cityBoundaryLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        // Set the map scale and center point
        MapView.MapScale = 250_000;
        MapView.CenterPoint = new PointShape(-10778771, 3915235);
        await MapView.RefreshAsync();
    }

    // Here we are creating a simple CVS feature source using the minimum set of overloads.
    // Since CSV doesn't include a way to do spatial queries we only need to return all the features
    // in the method below and the base class will do the rest.  Of course if you had large dataset this
    // would be slow, so I recommend you look at other overloads and implement optimized versions of these methods
    public class SimpleCsvFeatureSource(string csvPathFileName) : FeatureSource
    {
        private readonly Collection<Feature> _features = [];

        public string CsvPathFileName { get; set; } = csvPathFileName;

        protected override Collection<Feature> GetAllFeaturesCore(IEnumerable<string> returningColumnNames)
        {
            // If we haven't loaded the CSV then load it and return all the _features
            if (_features.Count != 0) return _features;
            var locations = File.ReadAllLines(CsvPathFileName);

            foreach (var location in locations)
            {
                var items = location.Split(',');
                var (lat, lon) = (double.Parse(items[0]), double.Parse(items[1]));
                _features.Add(new Feature(lon, lat));
            }
            return _features;
        }
    }

    // We need to create a layer that wraps the feature source.  FeatureLayer has everything we need we just need
    // to provide a constructor and set the feature source and all the methods on the feature layer just work.
    public class SimpleCsvFeatureLayer : FeatureLayer
    {
        public SimpleCsvFeatureLayer(string csvPathFileName)
        {
            FeatureSource = new SimpleCsvFeatureSource(csvPathFileName);
        }
        public override bool HasBoundingBox => true;
    }
}