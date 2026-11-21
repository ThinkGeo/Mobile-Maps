using System.Globalization;
using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.Gpu;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// Measuring what a finger draws: tap to add vertices, double-tap or long-press to
/// finish, and the line or the polygon reports its length or its area and where its
/// centre is - on the ground rather than on the web-mercator plane, which grows
/// areas away from the equator by about forty per cent in Texas.
/// </summary>
/// <remarks>
/// GetLength, GetArea and GetCenterPoint used to be a sample of their own, asked of
/// parks and trails read from a file. They are the same calls on a shape the finger
/// made, and a shape the finger made is the one a phone can show them with.
/// GetShortestLineTo was here too, drawn from the shape to a fixed stadium, and it
/// is gone: a second measurement to somewhere the reader never chose read as part of
/// what they had just drawn rather than as another question being asked of it.
/// <para>
/// Two gestures end a shape, and both are the overlay's own: a double tap, which is
/// the double click every desktop sketch tool ends on, and a long press, which is
/// the one that survives a shaky thumb. Neither reaches the map underneath - the
/// overlay reports the gesture handled - so the double tap does not also zoom.
/// </para>
/// <para>
/// WPF has MeasureInteractiveOverlay, a TrackInteractiveOverlay that labels itself.
/// MAUI has the tracking overlay but not the measuring subclass, so both modes here
/// are plain tracking and the measurement is the sample's own - which is the part
/// worth seeing anyway: the shape is converted to longitude and latitude first, and
/// only then measured.
/// </para>
/// </remarks>
public class MeasureDistanceAndArea : SamplePage
{
    /// <summary>The centre dot, which no tracking overlay owns.</summary>
    private readonly DynamicSource _centre = new();

    private MapView _map;
    private Label _result;
    private Label _detail;
    private bool _area;

    protected override void BuildMap(MapView map)
    {
        _map = map;
        map.MapUnit = GeographyUnit.Meter;

        var style = new MapStyle(ThinkGeoVectorStyles.Dark, new ThinkGeoVectorTileSource(SampleShared.CloudApiKey)
        {
            VectorTileCache = new FileTileCache(SampleShared.VectorTileCacheFolder),
        });
        style.AddDynamic(_centre);
        map.Basemap = new GpuBasemap(style);

        AddChoiceStrip(new[] { "Distance", "Area" }, 0, index =>
        {
            _area = index == 1;
            Reset();
        });

        var clear = new Button { Text = "Clear", FontSize = 12, Padding = new Thickness(10, 4) };
        clear.Clicked += (_, _) => Reset();
        AddToPanel(clear);

        _result = AddToPanel(new Label { FontSize = 13, FontAttributes = FontAttributes.Bold });
        _detail = AddStatusLabel();

        map.CurrentExtent = new RectangleShape(-10_790_000, 3_925_000, -10_770_000, 3_905_000); // Frisco, TX
        Reset();
        _ = map.RefreshAsync();
    }

    // A fresh overlay of the same kind is an empty one.
    private void Reset()
    {
        _map.TrackOverlay = _area ? NewAreaOverlay() : NewRuler();
        _result.Text = string.Empty;
        // Short enough to fit the one line the panel gives it: the longer form of
        // this - "Tap to add corners, then double-tap or long-press to close the
        // polygon" - was cut off mid-word on a phone, which hid the half that was
        // worth saying.
        _detail.Text = _area
            ? "Tap corners, then double-tap or long-press to close"
            : "Tap points, then double-tap or long-press to finish";

        _centre.Clear();
        _ = _map.RefreshAsync();
    }

    private TrackInteractiveOverlay NewRuler()
    {
        var overlay = new TrackInteractiveOverlay { TrackMode = TrackMode.Line };
        var zoomLevel = overlay.TrackShapeLayer.ZoomLevelSet.ZoomLevel01;
        zoomLevel.DefaultLineStyle = LineStyle.CreateSimpleLineStyle(GeoColors.CornflowerBlue, 3, false);
        zoomLevel.DefaultTextStyle = TextStyle.CreateSimpleTextStyle("label", "Noto Sans", 11,
            DrawingFontStyles.Regular, GeoColors.Black, GeoColors.White, 4);
        zoomLevel.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        overlay.TrackEnded += async (_, args) =>
        {
            if (args.TrackShape is not LineShape line) return;

            var onGround = (LineShape)ToLonLat(line);
            var length = onGround.GetLength(GeographyUnit.DecimalDegree, DistanceUnit.Kilometer);
            Label(overlay, FormattableString.Invariant($"{length:N2} km"));
            _result.Text = FormattableString.Invariant($"{length:N2} km on the ground");
            Describe(line);
            await overlay.RefreshAsync();
        };

        return overlay;
    }

    private TrackInteractiveOverlay NewAreaOverlay()
    {
        var overlay = new TrackInteractiveOverlay { TrackMode = TrackMode.Polygon };
        var zoomLevel = overlay.TrackShapeLayer.ZoomLevelSet.ZoomLevel01;
        zoomLevel.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(
            GeoColor.FromArgb(60, GeoColors.CornflowerBlue), GeoColors.CornflowerBlue, 2);
        zoomLevel.DefaultTextStyle = TextStyle.CreateSimpleTextStyle("label", "Noto Sans", 11,
            DrawingFontStyles.Regular, GeoColors.Black, GeoColors.White, 4);
        zoomLevel.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        overlay.TrackEnded += async (_, args) =>
        {
            if (args.TrackShape is not PolygonShape polygon) return;

            // Measured on the ground, not on the web-mercator plane, which grows areas
            // away from the equator - by about 40% in Texas.
            var onGround = (PolygonShape)ToLonLat(polygon);
            var area = onGround.GetArea(GeographyUnit.DecimalDegree, AreaUnit.SquareKilometers);
            Label(overlay, FormattableString.Invariant($"{area:N2} km²"));
            _result.Text = FormattableString.Invariant($"{area:N2} km² on the ground");
            Describe(polygon);
            await overlay.RefreshAsync();
        };

        return overlay;
    }

    /// <summary>
    /// The other question, asked of the same shape: where its centre is - marked, and
    /// read back in degrees, which is where the shape was measured.
    /// </summary>
    private void Describe(BaseShape shape)
    {
        var centre = shape.GetCenterPoint();
        var lonLat = (PointShape)ToLonLat(centre);

        _centre.SetPoint("centre", new Feature(centre), color: GeoColors.OrangeRed, sizeInPixel: 12f);

        _detail.Text = string.Create(CultureInfo.InvariantCulture,
            $"centre {lonLat.Y:F4}, {lonLat.X:F4}");
    }

    private static BaseShape ToLonLat(BaseShape shape)
    {
        var converter = new ProjectionConverter(3857, 4326);
        converter.Open();
        var converted = converter.ConvertToExternalProjection(shape);
        converter.Close();
        return converted;
    }

    private static void Label(TrackInteractiveOverlay overlay, string text)
    {
        var drawn = overlay.TrackShapeLayer.InternalFeatures.LastOrDefault();
        if (drawn != null) drawn.ColumnValues["label"] = text;
    }
}
