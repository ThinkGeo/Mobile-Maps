using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.XYZBasedLayers;

public partial class MVT
{
    public MVT()
    {
        InitializeComponent();
    }

    private async void MapView_OnSizeChanged(object sender, EventArgs e)
    {
        MapView.MapUnit = GeographyUnit.Meter;
        LayerOverlay layerOverlay = new LayerOverlay();
        MapView.Overlays.Add(layerOverlay);

        var openstackMbtiles = new MvtTilesAsyncLayer($@"https://tiles.preludemaps.com/data/planet_1015",
            "https://tiles.preludemaps.com/styles/TG_Savannah_Light/style.json");
        openstackMbtiles.MaxZoom = 14;

        layerOverlay.Layers.Add(openstackMbtiles);
        await openstackMbtiles.OpenAsync();
        var rectangleShape = new RectangleShape(-14377, 6712202, -12504, 6710862);
        MapView.MapScale = 20000;
        MapView.CenterPoint = rectangleShape.GetCenterPoint();

        await MapView.RefreshAsync();
    }
}