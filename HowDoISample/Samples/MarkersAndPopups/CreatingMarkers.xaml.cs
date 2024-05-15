using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.MarkersAndPopups;

public partial class CreatingMarkers
{
    private bool _initialized;
    public CreatingMarkers()
    {
        InitializeComponent();
        MapView.SingleTap += MapView_SingleTap;
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
        MapView.CenterPoint = new PointShape(-10777132, 3908560);
        MapView.MapScale = 12000;

        var simpleMarkerOverlay = new SimpleMarkerOverlay();
        MapView.Overlays.Add("simpleMarkerOverlay", simpleMarkerOverlay);

        MapView.IsRotationEnabled = true;
        await MapView.RefreshAsync();
    }

    /// <summary>
    ///     Adds a marker to the simpleMarkerOverlay where the map tap event occurred.
    /// </summary>
    private async void MapView_SingleTap(object sender, SingleTapMapViewEventArgs e)
    {
        var simpleMarkerOverlay = (SimpleMarkerOverlay)MapView.Overlays["simpleMarkerOverlay"];
        var pointInWorldCoordinate = MapView.ToWorldCoordinate(e.X, e.Y);

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