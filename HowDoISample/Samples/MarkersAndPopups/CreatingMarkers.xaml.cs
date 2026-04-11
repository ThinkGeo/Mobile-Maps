using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.MarkersAndPopups;

public partial class CreatingMarkers
{
    private bool _initialized;
    public CreatingMarkers()
    {
        InitializeComponent();
        mapView.SingleTap += Map_SingleTap;
    }

    private async void Map_OnSizeChanged(object sender, EventArgs e)
    {
        if (_initialized)
            return;
        _initialized = true;

        // Set the map's unit of measurement to meters(Spherical Mercator)
        mapView.MapUnit = GeographyUnit.Meter;

        // Add Cloud Maps as a background overlay
        var backgroundOverlay = new ThinkGeoVectorOverlay
        {
            ClientId = SampleKeys.ClientId,
            ClientSecret = SampleKeys.ClientSecret,
            MapType = ThinkGeoCloudVectorMapsMapType.Light,
            TileCache = new FileRasterTileCache(FileSystem.Current.CacheDirectory, "ThinkGeoVectorLight_RasterCache")
        };
        mapView.Overlays.Add(backgroundOverlay);

        mapView.MapTools.Add(new ZoomMapTool());

        // Set the map extent        
        mapView.CenterPoint = new PointShape(-10777132, 3908560);
        mapView.MapScale = 12000;

        var simpleMarkerOverlay = new SimpleMarkerOverlay();
        mapView.Overlays.Add("simpleMarkerOverlay", simpleMarkerOverlay);

        mapView.IsRotationEnabled = true;
        await mapView.RefreshAsync();
    }

    /// <summary>
    ///     Adds a marker to the simpleMarkerOverlay where the map tap event occurred.
    /// </summary>
    private async void Map_SingleTap(object sender, SingleTapMapViewEventArgs e)
    {
        var simpleMarkerOverlay = (SimpleMarkerOverlay)mapView.Overlays["simpleMarkerOverlay"];
        var pointInWorldCoordinate = mapView.ToWorldCoordinate(e.X, e.Y);

        // Create a marker at the position the mouse was tapped
        var marker = new ImageMarker
        {
            Position = pointInWorldCoordinate,
            ImagePath = "marker.png",
            TranslationY = -17,
            WidthRequest = 20,
            HeightRequest = 34
        };

        // Add the marker to the simpleMarkerOverlay and refresh the map        
        simpleMarkerOverlay.Children.Add(marker);

        await simpleMarkerOverlay.RefreshAsync();
    }
}
