using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.MarkersAndPopups;

public partial class UsingMarkers
{
    private bool _initialized;
    public UsingMarkers()
    {
        InitializeComponent();
    }

    protected override async void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);

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

        MapView.MapTools.Add(new ZoomMapTool());

        // Set the map extent        
        MapView.CenterPoint = new PointShape(-10777032, 3908560);
        MapView.MapScale = 10000;

        await AddHotelMarkersAsync();

        MapView.IsRotationEnabled = true;
        await MapView.RefreshAsync();
    }

    private async Task AddHotelMarkersAsync()
    {
        // Create a PopupOverlay
        var markerOverlay = new SimpleMarkerOverlay();

        var hotelsLayer = new ShapeFileFeatureLayer(Path.Combine(FileSystem.Current.AppDataDirectory, "Data", "Shapefile", "Hotels.shp"))
        {
            FeatureSource =
                {
                    // Project the data to match the map's projection
                    ProjectionConverter = new ProjectionConverter(2276, 3857)
                }
        };

        // Open the layer so that we can begin querying
        hotelsLayer.Open();

        // Query all the hotel features
        var hotelFeatures = hotelsLayer.QueryTools.GetAllFeatures(ReturningColumnsType.AllColumns);

        // Add each hotel feature to the popupOverlay
        foreach (var feature in hotelFeatures)
        {
            var popup = new RollingTextMarker
            {
                Position = feature.GetShape() as PointShape,
                ImagePath = "hotel_icon.png",
                Text = feature.ColumnValues["NAME"]
            };

            markerOverlay.Children.Add(popup);
        }

        // Close the hotel layer
        hotelsLayer.Close();

        // Add the popupOverlay to the map and refresh
        MapView.Overlays.Add(markerOverlay);
        await MapView.RefreshAsync();
    }
}
