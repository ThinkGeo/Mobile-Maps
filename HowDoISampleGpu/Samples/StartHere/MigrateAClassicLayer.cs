using System.Collections.ObjectModel;
using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// The same five classic layers, drawn twice: above by the GPU basemap, after their
/// styles have been translated into a style document, and below by the classic
/// pipeline that has always drawn them. The two views move together.
/// </summary>
/// <remarks>
/// Not a SamplePage: this one needs two maps, stacked rather than side by side.
/// </remarks>
public class MigrateAClassicLayer : ContentPage
{
    private static readonly PointShape FriscoCenter = new(-10777290, 3908740);
    private const double FriscoScale = 9000;

    private readonly MapView _newMap;
    private readonly MapView _classicMap;
    private readonly Label _status;

    private bool _syncing;
    private bool _built;

    public MigrateAClassicLayer()
    {
        _newMap = NewMap();
        _classicMap = NewMap();
        _status = new Label { FontSize = 11, TextColor = Colors.DimGray };

        var card = new Border
        {
            Content = new ScrollView { MaximumHeightRequest = 120, Content = _status },
            BackgroundColor = Color.FromRgba(255, 255, 255, 235),
            StrokeThickness = 0,
            Padding = new Thickness(12, 10),
            Margin = new Thickness(10, 10, 10, 0),
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 12 },
        };

        var grid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Star),
                new RowDefinition(GridLength.Star),
            },
        };
        grid.Add(card, 0, 0);
        grid.Add(Titled("translated into a style document, drawn by the GPU", _newMap), 0, 1);
        grid.Add(Titled("the classic pipeline, drawing the same layers", _classicMap), 0, 2);
        Content = grid;

        _newMap.SizeChanged += (_, _) => Build();
        _classicMap.SizeChanged += (_, _) => Build();
    }

    private static MapView NewMap() => new()
    {
        MapUnit = GeographyUnit.Meter,
        IsRotationEnabled = true,
        HorizontalOptions = LayoutOptions.Fill,
        VerticalOptions = LayoutOptions.Fill,
        Behaviors = { new AutoLoadMapViewBehavior() },
    };

    private static View Titled(string text, MapView map)
    {
        var grid = new Grid { Children = { map } };
        grid.Add(new Label
        {
            Text = text,
            FontSize = 10,
            TextColor = Colors.White,
            BackgroundColor = Color.FromRgba(0, 0, 0, 140),
            Padding = new Thickness(8, 3),
            VerticalOptions = LayoutOptions.Start,
            HorizontalOptions = LayoutOptions.Start,
            Margin = new Thickness(8, 8, 0, 0),
        });
        return grid;
    }

    private void Build()
    {
        if (_built) return;
        if (_newMap.Width <= 0 || _newMap.Height <= 0 || _classicMap.Width <= 0 || _classicMap.Height <= 0) return;
        _built = true;

        var style = new MapStyle().SetBackground(GeoColor.FromHtml("#EAE8E2"));
        var warnings = new List<string>();

        foreach (var layer in ClassicLayers())
        {
            // WPF passes the map's own zoom ladder; MAUI's MapView does not expose
            // one, and the default ladder of a metre-unit map is the MapLibre one the
            // translator assumes, so the single-argument overload is the right call.
            var translation = FeatureLayerTranslator.Translate(layer);
            style.AddFeatureLayer(translation);
            warnings.AddRange(translation.Warnings);
        }

        _newMap.Basemap = new GpuBasemap(style);
        _status.Text = warnings.Count == 0
            ? "Every style on these layers has an equivalent; nothing was left out."
            : "Not translated:\n" + string.Join("\n", warnings);

        // The desktop asks for TileType.SingleTile here; a single-tile overlay draws
        // nothing on MAUI, so this one keeps the default.
        var overlay = new LayerOverlay();
        foreach (var layer in ClassicLayers())
        {
            overlay.Layers.Add(layer);
        }

        _classicMap.Overlays.Add(overlay);

        _newMap.CenterPoint = FriscoCenter;
        _newMap.MapScale = FriscoScale;
        _classicMap.CenterPoint = FriscoCenter;
        _classicMap.MapScale = FriscoScale;

        _newMap.CurrentExtentChanged += (_, _) => Mirror(_classicMap, _newMap.CurrentExtent);
        _classicMap.CurrentExtentChanged += (_, _) => Mirror(_newMap, _classicMap.CurrentExtent);

        _ = _newMap.RefreshAsync();
        _ = _classicMap.RefreshAsync();
    }

    private void Mirror(MapView target, RectangleShape extent)
    {
        if (_syncing) return;

        _syncing = true;
        target.CurrentExtent = extent;
        _ = target.RefreshAsync();
        _syncing = false;
    }

    /// <summary>
    /// The layers as a classic application builds them. Built once per map: a
    /// shapefile is read by one renderer at a time.
    /// </summary>
    private static IEnumerable<FeatureLayer> ClassicLayers()
    {
        ShapeFileFeatureLayer Layer(string file) =>
            new(SampleShared.Shapefile(file)) { FeatureSource = { ProjectionConverter = new ProjectionConverter(2276, 3857) } };

        var zoning = Layer("Zoning.shp");
        zoning.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(new ValueStyle("ZONING", new Collection<ValueItem>
        {
            new("C-1", new AreaStyle(new GeoSolidBrush(GeoColor.FromHtml("#F2C9B4")))),
            new("I", new AreaStyle(new GeoSolidBrush(GeoColor.FromHtml("#CBC3D8")))),
            new("AG", new AreaStyle(new GeoSolidBrush(GeoColor.FromHtml("#D7E3BE")))),
            new("O-1", new AreaStyle(new GeoSolidBrush(GeoColor.FromHtml("#F4E2A6")))),
            new("O-2", new AreaStyle(new GeoSolidBrush(GeoColor.FromHtml("#F4E2A6")))),
        })
        {
            DefaultStyle = new AreaStyle(new GeoSolidBrush(GeoColor.FromHtml("#E9E4DA"))),
        });
        zoning.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        var parks = Layer("Parks.shp");
        parks.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(AreaStyle.CreateHatchStyle(
            GeoHatchStyle.DiagonalCross, GeoColor.FromHtml("#5E9E62"),
            GeoColor.FromArgb(110, 180, 224, 182), GeoColor.FromHtml("#3F7F45")));
        parks.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(new TextStyle("NAME",
            new GeoFont("Noto Sans", 12, DrawingFontStyles.Bold), new GeoSolidBrush(GeoColors.DarkGreen))
        {
            HaloPen = new GeoPen(GeoColors.White, 2),
        });
        parks.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        var trails = Layer("Hike_Bike.shp");
        trails.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(
            new LineStyle(new GeoPen(GeoColor.FromHtml("#8C5A2B"), 2) { DashStyle = LineDashStyle.Dash }));
        trails.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        var streets = Layer("Streets.shp");
        streets.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(
            new LineStyle(new GeoPen(GeoColors.DimGray, 6), new GeoPen(GeoColors.WhiteSmoke, 4)));
        streets.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
        streets.ZoomLevelSet.ZoomLevel17.CustomStyles.Add(new TextStyle("FULL_NAME",
            new GeoFont("Noto Sans", 11, DrawingFontStyles.Bold), new GeoSolidBrush(GeoColors.MidnightBlue))
        {
            HaloPen = new GeoPen(GeoColors.White, 2),
        });
        streets.ZoomLevelSet.ZoomLevel17.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        var schools = Layer("Schools.shp");
        schools.ZoomLevelSet.ZoomLevel15.CustomStyles.Add(new PointStyle(PointSymbolType.Star, 14,
            new GeoSolidBrush(GeoColor.FromHtml("#F5B301")), new GeoPen(GeoColor.FromHtml("#8A5A00"), 1)));
        schools.ZoomLevelSet.ZoomLevel15.CustomStyles.Add(new TextStyle("NAME",
            new GeoFont("Noto Sans", 11, DrawingFontStyles.Bold), new GeoSolidBrush(GeoColor.FromHtml("#8A5A00")))
        {
            TextPlacement = TextPlacement.Lower,
            YOffsetInPixel = 2,
            HaloPen = new GeoPen(GeoColors.White, 2),
        });
        schools.ZoomLevelSet.ZoomLevel15.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        yield return zoning;
        yield return parks;
        yield return trails;
        yield return streets;
        yield return schools;
    }
}
