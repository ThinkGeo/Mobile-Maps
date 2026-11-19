using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// What a map should keep when its view changes size - the scale, the scale and the
/// centre, or the whole extent. Resize the map with the switch, or turn the phone.
/// </summary>
/// <remarks>
/// WPF and the native Android SDK have MapView.MapResizeMode; MAUI does not, so the
/// three modes are the app's own, applied where a phone actually resizes a map: the
/// view's SizeChanged, which is what a rotation or a split screen raises.
/// </remarks>
public class ResizeTheMap : SamplePage
{
    private enum ResizeMode
    {
        PreserveScale,
        PreserveScaleAndCenter,
        PreserveExtent,
    }

    private static readonly string[] ModeNames =
    {
        "Preserve the scale",
        "Preserve the scale and the centre",
        "Preserve the extent",
    };

    private MapView _map;
    private Label _status;
    private ResizeMode _mode = ResizeMode.PreserveScaleAndCenter;

    private RectangleShape _extentBefore;
    private PointShape _centerBefore;
    private double _scaleBefore;
    private bool _narrow;

    protected override void BuildMap(MapView map)
    {
        _map = map;
        map.MapUnit = GeographyUnit.Meter;
        PutPanelAtTop();

        map.Basemap = new GpuBasemap(new MapStyle(ThinkGeoVectorStyles.Dark,
            new ThinkGeoVectorTileSource(SampleShared.CloudApiKey)
            { VectorTileCache = new FileTileCache(SampleShared.VectorTileCacheFolder) }));

        map.CenterPoint = new PointShape(-10336000, 5260000);
        map.MapScale = 37000000;

        AddChoicePicker(ModeNames, 1, index => _mode = (ResizeMode)index);

        var resize = new Button { Text = "Resize the map", FontSize = 12, Padding = new Thickness(10, 4) };
        resize.Clicked += (_, _) =>
        {
            Remember();
            _narrow = !_narrow;

            // Changing the margin changes the view's size, which is the whole event
            // this sample is about - the same one a rotation raises.
            _map.Margin = _narrow ? new Thickness(60, 140, 60, 140) : new Thickness(0);
        };
        AddToPanel(resize);

        _status = AddStatusLabel();

        // SizeChanged fires after the view has its new size, which is where the mode is
        // applied: the map has already been laid out, and what it keeps is put back.
        map.SizeChanged += (_, _) => Restore();

        Remember();
        Show();
        _ = map.RefreshAsync();
    }

    private void Remember()
    {
        _extentBefore = _map.CurrentExtent;
        _centerBefore = _map.CenterPoint;
        _scaleBefore = _map.MapScale;
    }

    private void Restore()
    {
        if (_extentBefore == null || _map.CanvasWidth <= 0 || _map.CanvasHeight <= 0) return;

        switch (_mode)
        {
            case ResizeMode.PreserveScale:
                // The scale survives; where the map is centred is left to the view, so
                // the ground slides as the view grows or shrinks.
                _map.MapScale = _scaleBefore;
                break;

            case ResizeMode.PreserveScaleAndCenter:
                _map.MapScale = _scaleBefore;
                _map.CenterPoint = _centerBefore;
                break;

            case ResizeMode.PreserveExtent:
                // Everything that was on screen stays on screen, so the scale has to
                // give: a narrower view of the same ground is a smaller scale.
                _map.CurrentExtent = _extentBefore;
                break;
        }

        Show();
        Remember();
        _ = _map.RefreshAsync();
    }

    private void Show() => _status.Text = FormattableString.Invariant(
        $"{_map.CanvasWidth:0} x {_map.CanvasHeight:0} px · 1:{_map.MapScale:N0}");
}
