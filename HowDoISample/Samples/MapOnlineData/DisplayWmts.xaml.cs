using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.MapOnlineData;

public partial class DisplayWmts
{
    private bool _initialized;
    public DisplayWmts()
    {
        InitializeComponent();
    }

    private async void MapView_OnSizeChanged(object sender, EventArgs e)
    {
        if (_initialized)
            return;
        _initialized = true;

        // It is important to set the map unit first to either feet, meters or decimal degrees.
        MapView.MapUnit = GeographyUnit.Meter;

        // Create a WMTS overlay using the WMS parameters below.
        // This is a public service and performance may be slow.
        var wmtsLayer = new ThinkGeo.Core.Async.WmtsLayer
        {
            DrawingExceptionMode = DrawingExceptionMode.DrawException,
            WmtsSeverEncodingType = WmtsSeverEncodingType.Restful
        };

        wmtsLayer.ServerUris.Add(new Uri("https://wmts.geo.admin.ch/1.0.0"));
        wmtsLayer.CapabilitesCacheTimeout = new TimeSpan(0, 0, 0, 1);
        wmtsLayer.ActiveLayerName = "ch.swisstopo.pixelkarte-farbe-pk25.noscale";
        wmtsLayer.ActiveStyleName = "default";
        wmtsLayer.OutputFormat = "image/png";
        wmtsLayer.TileMatrixSetName = "21781_26";
        wmtsLayer.TileCache = new FileRasterTileCache(FileSystem.Current.CacheDirectory, "WmtsTmpTileCache");

        //Add the new WMTS Layer to our LayerOverlay
        var layerOverlay = new LayerOverlay();
        layerOverlay.TileType = TileType.MultiTile;
        layerOverlay.Layers.Add(wmtsLayer);

        //Add the overlay to the mapView's Overlay collection.
        MapView.Overlays.Add(layerOverlay);

        // Set the extent to the Eiger - a famous peak in Switzerland.
        MapView.CenterPoint = new PointShape(643000, 158000);
        MapView.MapScale = 20000;

        await MapView.RefreshAsync();
    }
}