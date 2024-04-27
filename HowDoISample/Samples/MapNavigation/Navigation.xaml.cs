using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.MapNavigation;

public partial class Navigation
{
    private bool _initialized;
    public Navigation()
    {
        InitializeComponent();

        ThinkGeoDebugger.LogLevel = ThinkGeoLogLevel.All;
        ThinkGeoDebugger.LogType = ThinkGeoLogType.All;

    }

    private async void MapView_OnSizeChanged(object sender, EventArgs e)
    {
        if (_initialized)
            return;
        _initialized = true;

        // Set the map's unit of measurement to meters(Spherical Mercator)
        MapView.MapUnit = GeographyUnit.Meter;

        // Add ThinkGeo Cloud Maps as the background 
        var backgroundOverlay = new ThinkGeoRasterOverlay
        {
            ClientId = SampleKeys.ClientId,
            ClientSecret = SampleKeys.ClientSecret,
            MapType = ThinkGeoCloudRasterMapsMapType.Light_V2_X2,
            TileCache = new FileRasterTileCache(FileSystem.Current.CacheDirectory, "ThinkGeoRasterCache")
        };
        MapView.Overlays.Add(backgroundOverlay);

        // create a point for empire state building, convert the Lat/Lon (srid:4326) to Spherical Mercator(srid:3857), which is the projection of the background
        var empireStateBuilding =
            ProjectionConverter.Convert(4326, 3857, new PointShape(-73.985665442769, 40.7484366107232));

        var marker = new RollingTextMarker
        { 
            Position = empireStateBuilding,
            Text = "Empire State Building",
            ImagePath = "empire_state_building.png"
        };

        var simpleMarkerOverlay = new SimpleMarkerOverlay();
        MapView.Overlays.Add("simpleMarkerOverlay", simpleMarkerOverlay);
        simpleMarkerOverlay.Children.Add(marker);

        MapView.IsRotationEnabled = true;

        // events
        DefaultExtentButton.Clicked += async (_, _) =>
            await MapView.ZoomToExtentAsync(empireStateBuilding, 100000, -30);
        CompassButton.Clicked += async (_, _) =>
            await MapView.ZoomToExtentAsync(MapView.CenterPoint, MapView.MapScale, 0);
        ThemeCheckBox.CheckedChanged += async (_, args) =>
        {
            backgroundOverlay.MapType = args.Value
                ? ThinkGeoCloudRasterMapsMapType.Dark_V2_X2
                : ThinkGeoCloudRasterMapsMapType.Light_V2_X2;
            await backgroundOverlay.RefreshAsync();
        };

        // set up the map extent and refresh
        MapView.MapRotation = -30;
        MapView.MapScale = 100000;
        MapView.CenterPoint = empireStateBuilding;
        await MapView.RefreshAsync();
    }
}