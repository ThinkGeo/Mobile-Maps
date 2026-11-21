using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// Measuring what a finger draws: tap to add vertices, and the line or the polygon
/// reports its length or its area - measured on the ground rather than on the
/// web-mercator plane, which grows areas away from the equator by about 40 per cent
/// in Texas.
/// </summary>
/// <remarks>
/// WPF has MeasureInteractiveOverlay, which is a TrackInteractiveOverlay that labels
/// itself. MAUI has the TrackInteractiveOverlay but not the measuring subclass, so
/// both modes here are plain tracking and the measurement is the sample's own - which
/// is the part worth seeing anyway: the shape is converted to longitude and latitude
/// first, and only then measured.
/// </remarks>
public class MeasureDistanceAndArea : SamplePage
{
    private MapView _map;
    private Label _status;
    private bool _area;

    protected override void BuildMap(MapView map)
    {
        _map = map;
        map.MapUnit = GeographyUnit.Meter;

        map.Basemap = new GpuBasemap(new MapStyle(ThinkGeoVectorStyles.Dark,
            new ThinkGeoVectorTileSource(SampleShared.CloudApiKey)
            { VectorTileCache = new FileTileCache(SampleShared.VectorTileCacheFolder) }));

        AddChoiceStrip(new[] { "Distance", "Area" }, 0, index =>
        {
            _area = index == 1;
            Reset();
        });

        var clear = new Button { Text = "Clear", FontSize = 12, Padding = new Thickness(10, 4) };
        clear.Clicked += (_, _) => Reset();
        AddToPanel(clear);

        _status = AddStatusLabel();

        map.CurrentExtent = new RectangleShape(-10_790_000, 3_925_000, -10_770_000, 3_905_000); // Frisco, TX
        Reset();
        _ = map.RefreshAsync();
    }

    // A fresh overlay of the same kind is an empty one.
    private void Reset()
    {
        _map.TrackOverlay = _area ? NewAreaOverlay() : NewRuler();
        _status.Text = _area
            ? "Tap to add corners, double-tap to close the polygon"
            : "Tap to add points, double-tap to end the line";
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
            _status.Text = FormattableString.Invariant($"{length:N2} km on the ground");
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
            _status.Text = FormattableString.Invariant($"{area:N2} km² on the ground");
            await overlay.RefreshAsync();
        };

        return overlay;
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
