using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.Gpu;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// Keeping the map inside a box and between two scales: the three restrictions, each
/// switchable, with the box drawn in red so what is being enforced is visible.
/// </summary>
public class RestrictMapExtent : SamplePage
{
    // Dallas, TX: latitude 32.58 to 33.04, longitude -97.10 to -96.54, in EPSG:3857.
    private readonly RectangleShape _restrictExtent = new(-10809000, 3900000, -10747000, 3840000);

    private const double MaxZoomOutScale = 500000;
    private const double MinZoomInScale = 10000;

    private MapView _map;
    private Label _status;
    private bool _restrictBox = true;
    private bool _restrictMaximum = true;
    private bool _restrictMinimum = true;
    private bool _correcting;

    protected override void BuildMap(MapView map)
    {
        _map = map;
        map.MapUnit = GeographyUnit.Meter;
        PutPanelAtTop();

        // The red border is drawn with the basemap, so it stays sharp at every zoom.
        var boundary = new DynamicSource();
        var style = new MapStyle(ThinkGeoVectorStyles.Dark, new ThinkGeoVectorTileSource(SampleShared.CloudApiKey)
        {
            VectorTileCache = new FileTileCache(SampleShared.VectorTileCacheFolder),
        });
        style.AddDynamic(boundary);
        map.Basemap = new GpuBasemap(style);

        // A border, not a shaded box - a fill is drawn unless one is asked not to be.
        boundary.SetArea("boundary", new Feature(_restrictExtent), GeoColors.Transparent, new GeoColor(180, GeoColors.Red), 2f);

        // WPF has MapView.RestrictExtent; MAUI does not, and its CurrentExtentChanging
        // hands over the move it is about to make with every field but IsCanceled set
        // internally - so the move can be refused, not corrected. Refusing a pan leaves
        // the map stuck rather than resting against the edge, so the box is enforced
        // just after the move instead: CurrentExtentChanged, and the centre put back to
        // the nearest one that fits. The guard is because putting it back is itself a
        // move.
        map.CurrentExtentChanged += (_, _) =>
        {
            if (!_restrictBox || _correcting) return;

            var clamped = Clamp(_map.CurrentExtent);
            var center = clamped.GetCenterPoint();
            if (Math.Abs(center.X - _map.CenterPoint.X) < 1 && Math.Abs(center.Y - _map.CenterPoint.Y) < 1) return;

            _correcting = true;
            _map.CenterPoint = center;
            _correcting = false;
            _ = _map.RefreshAsync();
        };

        AddToPanel(Toggle("Keep it inside the red box", true, value => { _restrictBox = value; Apply(); }));
        AddToPanel(Toggle("Stop zooming out at 1:500,000", true, value => { _restrictMaximum = value; Apply(); }));
        AddToPanel(Toggle("Stop zooming in at 1:10,000", true, value => { _restrictMinimum = value; Apply(); }));
        _status = AddStatusLabel();

        Apply();
        map.CurrentExtent = _restrictExtent;
        _ = map.RefreshAsync();
    }

    private View Toggle(string text, bool value, Action<bool> changed)
    {
        var row = new HorizontalStackLayout { Spacing = 8 };
        var toggle = new Switch { IsToggled = value, Scale = 0.75 };
        toggle.Toggled += (_, e) => changed(e.Value);
        row.Add(toggle);
        row.Add(new Label { Text = text, FontSize = 12, VerticalOptions = LayoutOptions.Center });
        return row;
    }

    private void Apply()
    {
        // MaxScale stops zooming out beyond a scale (a larger number is further out);
        // MinScale stops zooming in past one. Both are the map's own, as on the desktop.
        _map.MaxScale = _restrictMaximum ? MaxZoomOutScale : double.MaxValue;
        _map.MinScale = _restrictMinimum ? MinZoomInScale : 0;

        if (_status == null) return;
        _status.Text = $"box {(_restrictBox ? "on" : "off")} · out {(_restrictMaximum ? "1:500,000" : "free")} · in {(_restrictMinimum ? "1:10,000" : "free")}";
        _ = _map.RefreshAsync();
    }

    /// <summary>
    /// The nearest extent of the same size that lies inside the box. A view wider or
    /// taller than the box cannot fit, so that dimension is centred on it instead -
    /// which is what a map does when it is zoomed further out than the box allows.
    /// </summary>
    private RectangleShape Clamp(RectangleShape extent)
    {
        var allowed = _restrictExtent;
        var width = extent.Width;
        var height = extent.Height;

        var centerX = extent.GetCenterPoint().X;
        var centerY = extent.GetCenterPoint().Y;

        if (width >= allowed.Width)
        {
            centerX = allowed.GetCenterPoint().X;
        }
        else
        {
            var minX = allowed.UpperLeftPoint.X + (width / 2);
            var maxX = allowed.LowerRightPoint.X - (width / 2);
            centerX = Math.Min(Math.Max(centerX, minX), maxX);
        }

        if (height >= allowed.Height)
        {
            centerY = allowed.GetCenterPoint().Y;
        }
        else
        {
            var minY = allowed.LowerRightPoint.Y + (height / 2);
            var maxY = allowed.UpperLeftPoint.Y - (height / 2);
            centerY = Math.Min(Math.Max(centerY, minY), maxY);
        }

        return new RectangleShape(
            centerX - (width / 2), centerY + (height / 2),
            centerX + (width / 2), centerY - (height / 2));
    }
}
