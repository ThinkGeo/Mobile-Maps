using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.MarkersAndPopups;

public partial class UsingPopups
{
    private bool _initialized;
    public UsingPopups()
    {
        InitializeComponent();
    }

    private async void MapView_OnSizeChanged(object sender, EventArgs e)
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

        // Set the map extent        
        MapView.CenterPoint = new PointShape(-10777800, 3908700);
        MapView.MapScale = 10000;

        AddHotelPopups();

        MapView.IsRotationEnabled = true;
        await MapView.RefreshAsync();
    }

    private void AddHotelPopups()
    {
        var hotelsLayer =
            new ShapeFileFeatureLayer(Path.Combine(FileSystem.Current.AppDataDirectory, "Data", "Shapefile", "Hotels.shp"));
        hotelsLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = PointStyle.CreateSimpleCircleStyle(GeoColors.Black, 5);
        hotelsLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
        hotelsLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);
            
        var layerOverlay = new LayerNonRotationGraphicsViewOverlay();
        layerOverlay.Layers.Add(hotelsLayer);
        MapView.Overlays.Add(layerOverlay);

        // Create a PopupOverlay
        var popupOverlay = new PopupOverlay();
       
        // Open the layer so that we can begin querying
        hotelsLayer.Open();
        // Query all the hotel features
        var hotelFeatures = hotelsLayer.QueryTools.GetAllFeatures(ReturningColumnsType.AllColumns);

        // Add each hotel feature to the popupOverlay
        foreach (var feature in hotelFeatures)
        {
            var popup = new Popup
            {
                Position = feature.GetShape() as PointShape,
                Text = feature.ColumnValues["NAME"]
            };

            popupOverlay.Children.Add(popup);
        }

        // Close the hotel layer
        hotelsLayer.Close();

        // Add the popupOverlay to the map and refresh
        MapView.Overlays.Add(popupOverlay);
    }
}
