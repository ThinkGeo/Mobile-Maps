using System.Collections.ObjectModel;
using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.MapOfflineData;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class ExtendingFeatureSources
{
    private bool _initialized;
    public ExtendingFeatureSources()
    {
        InitializeComponent();
    }

    private async void ExtendingFeatureSources_OnSizeChanged(object sender, EventArgs e)
    {
        if (_initialized)
            return;
        _initialized = true;

        // Set the map's unit of measurement to meters(Spherical Mercator)
        MapView.MapUnit = GeographyUnit.Meter;

        // Add Cloud Maps as a background overlay
        var backgroundOverlay = new ThinkGeoVectorOverlay
        {
            ClientId = SampleKeys.ClientId,
            ClientSecret = SampleKeys.ClientSecret,
            MapType = ThinkGeoCloudVectorMapsMapType.Light,
            TileCache = new FileRasterTileCache(FileSystem.Current.CacheDirectory, "ThinkGeoVectorLight_RasterCache")
        };
        MapView.Overlays.Add(backgroundOverlay);

        // See the implementation of the new layer and feature source below.
        var csvLayer = new SimpleCsvFeatureLayer(Path.Combine(FileSystem.Current.AppDataDirectory, "Data", "Csv", "vehicle-route.csv"))
            {
                FeatureSource =
                {
                    // this converter convert Decimal Degrees GPS points(epsg:4326) to the projection of ThinkGeo Background map (epsg:3857).
                    ProjectionConverter = new ProjectionConverter(4326, 3857)
                }
            };

        // Set the points image to a car icon and then apply it to all zoomlevels
        var vehiclePointStyle = PointStyle.CreateSimpleCircleStyle(GeoColors.Blue, 8, GeoColors.Black);
        csvLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = vehiclePointStyle;
        csvLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        var layerOverlay = new LayerOverlay();
        layerOverlay.Layers.Add(csvLayer);
        MapView.Overlays.Add(layerOverlay);

        // Set the map scale and center point
        MapView.MapScale = 50_000;
        MapView.CenterPoint = new PointShape(-8236733, 4971880);
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