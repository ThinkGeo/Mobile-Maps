using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.MarkersAndPopups;

public partial class UsingPopups
{
    private bool _initialized;
    private ShapeFileFeatureLayer _hotelsLayer;
    private LayerGraphicsViewOverlay _layerOverlay;
    private PopupOverlay _popupOverlay;

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

        // Initialize hotels layer
        _hotelsLayer = new ShapeFileFeatureLayer(
           Path.Combine(FileSystem.Current.AppDataDirectory, "Data", "Shapefile", "Hotels.shp"));

        _hotelsLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle =
            PointStyle.CreateSimpleCircleStyle(GeoColors.Black, 5);
        _hotelsLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
        _hotelsLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

        // Add LayerOverlay
        _layerOverlay = new LayerGraphicsViewOverlay();
        _layerOverlay.Layers.Add(_hotelsLayer);
        MapView.Overlays.Add(_layerOverlay);

        // Add PopupOverlay
        _popupOverlay = new PopupOverlay();
        MapView.Overlays.Add(_popupOverlay);

        // Set the map extent        
        MapView.CenterPoint = new PointShape(-10777800, 3908700);
        MapView.MapScale = 10000;

        await MapView.RefreshAsync();

        AddHotelPopups();
        MapView.IsRotationEnabled = true;
    }

    private void AddHotelPopups()
    {
        // Open the layer so that we can begin querying
        _hotelsLayer.Open();
        // Query all the hotel features
        var hotelFeatures = _hotelsLayer.QueryTools.GetAllFeatures(ReturningColumnsType.AllColumns);

        // Add each hotel feature to the popupOverlay
        foreach (var feature in hotelFeatures)
        {
            var popup = new Popup
            {
                Position = feature.GetShape() as PointShape,
                Text = feature.ColumnValues["NAME"]
            };

            _popupOverlay.Children.Add(popup);
        }

        // Close the hotel layer
        _hotelsLayer.Close();
    }
}
