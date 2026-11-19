using System.Globalization;
using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.Gpu;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// Why a measurement needs the right projection: Greenland, carried away from its own
/// latitude, swells in Web Mercator while the ground it covers never changes.
/// </summary>
/// <remarks>
/// The desktop sample lets you pick the copy up and drag it. A MAUI map raises
/// TouchDown, TouchMove and TouchUp but none of them carries a Handled flag, so a drag
/// on a shape also pans the map underneath it. The copy moves on a latitude slider
/// instead: the same ground, put down about a different parallel, with the same
/// figures.
/// </remarks>
public class MeasureInTheRightProjection : SamplePage
{
    // An equal-area projection about a latitude and Greenland's own longitude.
    private const string EqualArea = "+proj=laea +lat_0={0} +lon_0={1} +x_0=0 +y_0=0 +datum=WGS84 +units=m +no_defs";

    // 84.5 degrees north or south in Web Mercator; past it the stretch runs away
    // towards infinity.
    private const double MaxMercatorY = 19363000;

    private const string StyleJson = """
    {
      "layers": [
        { "type": "fill",
          "source-layer": "Countries02",
          "paint": {
              "fill-color": "#F2EFE9",
              "fill-outline-color": "#B8B2A7" }
        }
      ]
    }
    """;

    private readonly DynamicSource _greenland = new();

    private MultipolygonShape _flat;
    private MultipolygonShape _placed;
    private MapView _map;
    private Label _truth;
    private Label _here;
    private double _homeLatitude;
    private double _homeLongitude;
    private double _groundArea;
    private double _latitude;
    private double _drawnLatitude = double.NaN;

    protected override void BuildMap(MapView map)
    {
        _map = map;
        map.MapUnit = GeographyUnit.Meter;
        PutPanelAtTop();

        var tileSource = new FeatureSourceVectorTileSource();
        tileSource.FeatureSources.Add(new ShapeFileFeatureSource(SampleShared.Shapefile("Countries02.shp"))
        {
            ProjectionConverter = new ProjectionConverter(4326, 3857),
        });

        map.Basemap = new GpuBasemap(new MapStyle()
            .SetBackground(GeoColor.FromHtml("#C9DCE8"))
            .AddStyle(StyleJson, tileSource)
            .AddDynamic(_greenland));

        map.CenterPoint = new PointShape(-4608600, 5621500);
        map.MapScale = 130000000;

        // Greenland in metres of an equal-area projection about its own middle: the
        // ground it covers, wherever it is put down.
        var countries = new ShapeFileFeatureSource(SampleShared.Shapefile("Countries02.shp"));
        countries.Open();
        var greenland = (MultipolygonShape)countries.GetAllFeatures(new[] { "CNTRY_NAME" })
            .First(feature => feature.ColumnValues["CNTRY_NAME"].Trim() == "Greenland").GetShape();
        countries.Close();

        var home = greenland.GetCenterPoint();
        _homeLatitude = home.Y;
        _homeLongitude = home.X;

        var flatten = new ProjectionConverter(new Projection(4326),
            new Projection(string.Format(CultureInfo.InvariantCulture, EqualArea, _homeLatitude, _homeLongitude)));
        flatten.Open();
        var flat = ((MultipolygonShape)flatten.ConvertToExternalProjection(greenland))
            .Simplify(4000, SimplificationType.DouglasPeucker);
        flatten.Close();

        _flat = new MultipolygonShape(flat.Polygons
            .Where(polygon => polygon.OuterRing.GetArea(GeographyUnit.Meter, AreaUnit.SquareKilometers) > 500)
            .Select(polygon => new PolygonShape(polygon.OuterRing)));
        _groundArea = _flat.GetArea(GeographyUnit.Meter, AreaUnit.SquareKilometers);

        var atHome = Place(_homeLatitude);
        _greenland.SetArea("home", new Feature(atHome),
            GeoColor.FromArgb(46, 90, 90, 90), GeoColor.FromHtml("#666666"), 1.5f);

        _truth = AddStatusLabel();
        _truth.Text = FormattableString.Invariant(
            $"The ground: {_groundArea / 1e6:0.00} M km² · where it belongs: {atHome.GetArea(GeographyUnit.Meter, AreaUnit.SquareKilometers) / 1e6:0.00} M km²");

        _here = AddStatusLabel();

        AddToPanel(new Label { Text = "Carry it to another latitude", FontSize = 11, TextColor = Colors.Gray });
        var slider = new Slider { Minimum = -80, Maximum = 80, Value = _homeLatitude };
        slider.ValueChanged += (_, e) =>
        {
            _latitude = e.NewValue;
            Draw();
        };
        AddToPanel(slider);

        _latitude = _homeLatitude;
        Draw();
    }

    // The same ground put down about another latitude, in Web Mercator.
    private MultipolygonShape Place(double latitude)
    {
        var converter = new ProjectionConverter(
            new Projection(string.Format(CultureInfo.InvariantCulture, EqualArea, latitude, _homeLongitude)),
            new Projection(3857));
        converter.Open();
        var placed = (MultipolygonShape)converter.ConvertToExternalProjection(_flat);
        converter.Close();
        return placed;
    }

    private void Draw()
    {
        if (_latitude != _drawnLatitude)
        {
            var candidate = Place(_latitude);
            var bounds = candidate.GetBoundingBox();
            if (Math.Max(Math.Abs(bounds.UpperLeftPoint.Y), Math.Abs(bounds.LowerRightPoint.Y)) > MaxMercatorY)
            {
                _latitude = _drawnLatitude;
            }
            else
            {
                _placed = candidate;
                _drawnLatitude = _latitude;

                var here = _placed.GetArea(GeographyUnit.Meter, AreaUnit.SquareKilometers);
                var blown = here / _groundArea;
                _here.Text = blown < 1.05
                    ? FormattableString.Invariant($"At {_latitude:0.0}° it draws {here / 1e6:0.00} M km² - Web Mercator tells the truth at the equator.")
                    : FormattableString.Invariant($"At {_latitude:0.0}° it draws {here / 1e6:0.00} M km², {blown:0.0} times the ground it covers.");
                _here.TextColor = blown < 1.05 ? Colors.SeaGreen : Colors.IndianRed;
            }
        }

        _greenland.SetArea("copy", new Feature(_placed),
            GeoColor.FromArgb(115, 200, 60, 30), GeoColor.FromHtml("#C0392B"), 2f);
        _ = _map.RefreshAsync();
    }
}
