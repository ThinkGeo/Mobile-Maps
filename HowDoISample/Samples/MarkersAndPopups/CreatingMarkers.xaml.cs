using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.MarkersAndPopups;

public partial class CreatingMarkers
{
    private bool _initialized;
    public CreatingMarkers()
    {
        InitializeComponent();
        Map.SingleTap += Map_SingleTap;
    }

    private async void Map_OnSizeChanged(object sender, EventArgs e)
    {
        if (_initialized)
            return;
        _initialized = true;

        // Set the map's unit of measurement to meters(Spherical Mercator)
        Map.MapUnit = GeographyUnit.Meter;

        // Add Cloud Maps as a background overlay
        var backgroundOverlay = new ThinkGeoVectorOverlay
        {
            ClientId = SampleKeys.ClientId,
            ClientSecret = SampleKeys.ClientSecret,
            MapType = ThinkGeoCloudVectorMapsMapType.Light,
            TileCache = new FileRasterTileCache(FileSystem.Current.CacheDirectory, "ThinkGeoVectorLight_RasterCache")
        };
        Map.Overlays.Add(backgroundOverlay);

        Map.MapTools.Add(new ZoomMapTool());

        // Set the map extent        
        Map.CenterPoint = new PointShape(-10777132, 3908560);
        Map.MapScale = 12000;

        var simpleMarkerOverlay = new SimpleMarkerOverlay();
        Map.Overlays.Add("simpleMarkerOverlay", simpleMarkerOverlay);

        Map.IsRotationEnabled = true;
        await Map.RefreshAsync();
    }

    /// <summary>
    ///     Adds a marker to the simpleMarkerOverlay where the map tap event occurred.
    /// </summary>
    private async void Map_SingleTap(object sender, SingleTapMapViewEventArgs e)
    {
        var simpleMarkerOverlay = (SimpleMarkerOverlay)Map.Overlays["simpleMarkerOverlay"];
        var pointInWorldCoordinate = Map.ToWorldCoordinate(e.X, e.Y);

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
