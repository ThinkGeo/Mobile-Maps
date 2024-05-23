using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.MapOnlineData;

public partial class DisplayWms
{
    private bool _initialized;
    public DisplayWms()
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

        // Set the map extent
        MapView.CenterPoint = new RectangleShape(-10786436, 3918518, -10769429, 3906002).GetCenterPoint();
        MapView.MapScale = MapUtil.GetScale(new RectangleShape(-10786436, 3918518, -10769429, 3906002),
            MapView.MapWidth, MapView.MapUnit);

        // Create a WmsOverlay and add it to the map.
        var wmsOverlay = new WmsOverlay();
        wmsOverlay.AxisOrder = WmsAxisOrder.XY;
        wmsOverlay.Uri = new Uri("https://ows.mundialis.de/services/service");
        wmsOverlay.Parameters.Add("VERSION", "1.3.0");
        wmsOverlay.Parameters.Add("LAYERS", "OSM-WMS");
        wmsOverlay.Parameters.Add("STYLES", "default");
        wmsOverlay.Parameters.Add("CRS", "EPSG:3857");  // Make sure to match the WMS CRS to the Map's projection

        MapView.Overlays.Add(wmsOverlay);


        // ScaleFactor determines the ratio of physical pixels to virtual pixels. For instance,
        // on an iPhone Retina display, ScaleFactor is 3, indicating there are 3 physical pixels
        // for every virtual pixel. If you have a ScaleFactor of 3 and a map size of 100x100 pixels,
        // the system requests a 300x300 pixel image to display on the 100x100 pixel area. High-resolution
        // images on servers that support them appear sharper when displayed this way. However, if the server
        // does not support high-resolution images, as is the case here, features like roads may appear too small.
        // As a result, here we set the ScaleFactor to 1, we request an image that matches the map's resolution (100x100), ensuring road sizes are more visible, albeit at the cost of image sharpness. Uncomment the following line to observe the effect on the map's appearance.
        MapView.ScaleFactor = 1;

        await MapView.RefreshAsync();
    }
}