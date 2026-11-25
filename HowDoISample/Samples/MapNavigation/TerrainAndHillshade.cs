using System.Text.Json.Nodes;
using Microsoft.Maui.Controls.Shapes;
using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.UI.Maui;

namespace HowDoISample.Samples;

/// <summary>
/// The ground with its real shape: satellite imagery draped over a terrain source,
/// with roads drawn over the photograph, at Mount Rainier - and a switch to a vector
/// map of the same ground, shaded by a hillshade layer reading the same terrain.
/// <para>
/// The map opens tilted, because the tilt is what shows the terrain as terrain
/// rather than as shading. Two fingers sliding together change it from there.
/// </para>
/// </summary>
public class TerrainAndHillshade : SamplePage
{
    /// <summary>
    /// The published street style, read for its lines and symbols alone.
    /// </summary>
    private const string StreetsStyleUri = "https://tiles.preludemaps.com/styles/WorldStreets_Light/style.json";

    /// <summary>The vector source that style declares; the buildings come from it.</summary>
    private const string BuildingSource = "planet_06-20-2026";

    private const string HillshadeStyle = """
    {
      "layers": [
        { "type": "hillshade",
          "source": "terrain-dem",
          "paint": { "hillshade-exaggeration": 0.4 } }
      ]
    }
    """;

    /// <summary>
    /// Mount Rainier, from the south, where Paradise Road and Stevens Canyon Road
    /// climb its foot: roads on a slope show that lines are draped on the ground.
    /// <para>
    /// The centre is not the summit. A tilted camera looks at its centre, so that point
    /// lands mid-screen and whatever is nearer fills the lower half. Five kilometres
    /// short of the summit puts the summit a third of the way down the screen, with
    /// the whole south face under it.
    /// </para>
    /// </summary>
    private static readonly PointShape StartCenter = new(-13_554_290, 5_911_000);

    /// <summary>
    /// A scale rather than an extent, so every screen opens at the same zoom and a
    /// bigger one simply shows more ground. The aerial imagery changes to a coarser
    /// mosaic not far out from this scale, and an extent fitted to a small screen
    /// would land there.
    /// </summary>
    private const double StartScale = 91_280;

    /// <summary>
    /// Low to the ground. Seen from straight above, terrain and a hillshade image look
    /// the same; from here the summit stands against the sky.
    /// </summary>
    private const double StartTilt = 72;

    protected override void BuildMap(MapView map)
    {
        map.MapUnit = GeographyUnit.Meter;

        // The AWS open terrarium tiles (no key), as a code-built RasterDemSource. Both
        // styles read the same one: the ground does not change when the look does.
        _dem = new RasterDemSource(
            "https://s3.amazonaws.com/elevation-tiles-prod/terrarium/{z}/{x}/{y}.png",
            DemEncoding.Terrarium);

        _basemap = new GpuBasemap(ImageryStyle());
        map.Basemap = _basemap;

        // Three places on the mountain itself, close enough together that one view holds
        // all of them. A marker whose ground is hidden by terrain fades to a fifth of
        // its opacity rather than being drawn over the ridge as though it were in front
        // of it.
        var markers = new SimpleMarkerOverlay();
        markers.AddMarker("muir", PlaceMarker(new PointShape(-13_551_044, 5_915_362), "Camp Muir"));
        markers.AddMarker("gibraltar", PlaceMarker(new PointShape(-13_553_148, 5_916_973), "Gibraltar Rock"));
        markers.AddMarker("liberty", PlaceMarker(new PointShape(-13_554_595, 5_920_718), "Liberty Cap"));
        map.Overlays.Add(markers);

        AddToPanel(BuildShadedReliefToggle());

        map.CenterPoint = StartCenter;
        map.MapScale = StartScale;

        // Past the 60 degrees a map allows by default, the horizon comes into view.
        map.MaxTiltAngle = 80;

        // The map opens nearly flat and rises into the view.
        map.TiltAngle = EntranceFromTilt;
        map.MapRotation = EntranceFromRotation;
        _ = map.RefreshAsync();
        _ = RunEntranceAsync(map);
    }

    /// <summary>
    /// Buildings, standing on the terrain in both styles. Opaque, so the slope behind
    /// a building does not show through its walls.
    /// </summary>
    private static JsonObject BuildingLayer() => new()
    {
        ["id"] = "building-3d",
        ["type"] = "fill-extrusion",
        ["source"] = BuildingSource,
        ["source-layer"] = "building",
        ["minzoom"] = 13,
        ["filter"] = new JsonArray("!=", "hide_3d", true),
        ["paint"] = new JsonObject
        {
            ["fill-extrusion-color"] = "#D9DEE5",
            ["fill-extrusion-height"] = new JsonArray("get", "render_height"),
            ["fill-extrusion-base"] = new JsonArray("get", "render_min_height"),
            ["fill-extrusion-opacity"] = 1,
        },
    };

    /// <summary>
    /// A dot and a name, anchored on its ground point. Drawn rather than loaded, so the
    /// sample carries no image file.
    /// </summary>
    private static Marker PlaceMarker(PointShape position, string name) => new()
    {
        Position = position,
        WidthRequest = 92,
        HeightRequest = 18,
        Content = new HorizontalStackLayout
        {
            Spacing = 5,
            Children =
            {
                new Ellipse
                {
                    Fill = new SolidColorBrush(Color.FromArgb("#FFFFFF")),
                    Stroke = new SolidColorBrush(Color.FromArgb("#1E88E5")),
                    StrokeThickness = 3,
                    WidthRequest = 12,
                    HeightRequest = 12,
                    VerticalOptions = LayoutOptions.Center,
                },
                new Label
                {
                    Text = name,
                    FontSize = 11,
                    TextColor = Color.FromArgb("#FFFFFF"),
                    VerticalOptions = LayoutOptions.Center,
                },
            },
        },
    };

    /// <summary>Where the opening starts: flat enough to read as a map.</summary>
    private const double EntranceFromTilt = 25d;

    /// <summary>And turned off north, so the swing to it is visible.</summary>
    private const double EntranceFromRotation = -28d;

    /// <summary>
    /// Tilts and turns the camera into the opening view over about a second, with a
    /// cubic ease-out. Driven here rather than through ZoomToAsync because that
    /// animates the extent, and the extent is already where it belongs.
    /// </summary>
    private static async Task RunEntranceAsync(MapView map)
    {
        const int steps = 48;
        const int frameMs = 16;

        // Long enough for the first tiles, so the animation is not spent on an empty map.
        await Task.Delay(900);

        for (var i = 1; i <= steps; i++)
        {
            var t = (double)i / steps;
            var eased = 1d - Math.Pow(1d - t, 3d);   // cubic ease-out

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                map.TiltAngle = EntranceFromTilt + ((StartTilt - EntranceFromTilt) * eased);
                map.MapRotation = EntranceFromRotation * (1d - eased);
            });

            await Task.Delay(frameMs);
        }

        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            map.TiltAngle = StartTilt;
            map.MapRotation = 0;
            await map.RefreshAsync();
        });
    }

    private GpuBasemap _basemap;
    private RasterDemSource _dem;

    /// <summary>
    /// Imagery over terrain, and no hillshade: a photograph already carries the
    /// shadows of the sun it was taken under, and a hillshade layer over it only
    /// greys it.
    /// </summary>
    private MapStyle ImageryStyle() =>
        new MapStyle()
            .AddRaster(new ThinkGeoRasterTileSource(SampleShared.CloudApiKey, ThinkGeoCloudRasterMapsMapType.Aerial_V2_X1),
                sourceId: "thinkgeo")
            .SetTerrain(_dem, 1.35)

            // Roads over the photograph, and nothing else from that document. Lines
            // only, no symbols: a road is draped on the terrain, so a ridge in front of
            // it hides it, while a label is drawn in screen space and would show
            // through the mountain.
            .AddStyle(StreetsStyleUri, null, StyleLayerType.Line)
            .AddLayer(BuildingLayer())
            .SetSky(GeoColor.FromHtml("#6FB2EF"), GeoColor.FromHtml("#EAF4FF"), 0.85);

    /// <summary>
    /// The drawn map, with a hillshade layer. A vector style has no light in it - a
    /// glacier and a valley floor are the same flat colour - so the terrain under it
    /// is invisible until something shades it.
    /// </summary>
    private MapStyle VectorStyle() =>
        new MapStyle(StreetsStyleUri)
            .SetTerrain(_dem, 1.35)
            .AddStyle(HillshadeStyle)

            // After the hillshade, so the shade is not laid over the walls.
            .AddLayer(BuildingLayer())
            .SetSky(GeoColor.FromHtml("#6FB2EF"), GeoColor.FromHtml("#EAF4FF"), 0.85);

    /// <summary>The switch between the two styles.</summary>
    private View BuildShadedReliefToggle()
    {
        var toggle = new Switch { IsToggled = false, VerticalOptions = LayoutOptions.Center };
        toggle.Toggled += async (_, e) =>
        {
            // The camera is not touched: only the style changes, so the view holds its
            // place, its scale and its tilt across the swap.
            await _basemap.SetStyleAsync(e.Value ? VectorStyle() : ImageryStyle());
        };

        return new HorizontalStackLayout
        {
            Spacing = 8,
            Children =
            {
                new Label { Text = "Vector map with hillshade", FontSize = 13, TextColor = PanelInk, VerticalOptions = LayoutOptions.Center },
                toggle,
            },
        };
    }
}
