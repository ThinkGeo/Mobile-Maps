using System.Globalization;
using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.Gpu;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// Measuring what a finger draws: tap to add vertices, and the line or the polygon
/// reports its length or its area, where its centre is, and how far it is from a
/// fixed landmark - all of it on the ground rather than on the web-mercator plane,
/// which grows areas away from the equator by about forty per cent in Texas.
/// </summary>
/// <remarks>
/// The four questions a shape can be asked - GetLength, GetArea, GetCenterPoint and
/// GetShortestLineTo - used to be a sample of their own, asked of parks and trails
/// read from a file. They are the same four calls on a shape the finger made, and a
/// shape the finger made is the one a phone can show them with.
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
    /// <summary>Toyota Stadium: the fixed end of the shortest-line measurement.</summary>
    private static readonly PointShape Stadium = new(-10779651.5, 3915933.0);

    /// <summary>The landmark and the line drawn to it, which no tracking overlay owns.</summary>
    private readonly DynamicSource _landmark = new();

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
        style.AddDynamic(_landmark);
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
        _detail.Text = _area
            ? "Tap to add corners, double-tap to close the polygon"
            : "Tap to add points, double-tap to end the line";

        _landmark.Clear();
        _landmark.SetPoint("stadium", new Feature(Stadium), color: GeoColors.Gold, sizeInPixel: 14f);
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
    /// The other two questions, asked of the same shape: where its centre is, and the
    /// shortest line from it to the stadium - drawn, because a distance to somewhere
    /// is easier to believe with the line on the map.
    /// </summary>
    private void Describe(BaseShape shape)
    {
        var centre = shape.GetCenterPoint();
        var toStadium = shape.GetShortestLineTo(Stadium, GeographyUnit.Meter);
        var gap = toStadium.GetLength(new Projection(3857), DistanceUnit.Kilometer);

        var lonLat = (PointShape)ToLonLat(centre);

        _landmark.SetPoint("centre", new Feature(centre), color: GeoColors.OrangeRed, sizeInPixel: 12f);
        _landmark.SetLine("to-stadium", new Feature(toStadium), GeoColors.Gold, 3f);

        _detail.Text = string.Create(CultureInfo.InvariantCulture,
            $"centre {lonLat.Y:F4}, {lonLat.X:F4}  ·  {gap:N2} km to Toyota Stadium");
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
