using System.Text.Json.Nodes;
using Microsoft.Maui.Controls.Shapes;
using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.Gpu;
using ThinkGeo.UI.Maui;

namespace HowDoISample.Samples;

/// <summary>
/// The ground with its real shape: satellite imagery draped over a terrain source and
/// hill-shaded, with roads and place names drawn over the photograph, at the Grand
/// Canyon.
/// <para>
/// No controls at all. Terrain is what the sample is, so it is on; and the lean is
/// the only reason the terrain is visible as terrain rather than as shading, so the
/// map opens with it. Two fingers sliding together change it from there, which is
/// the gesture the platform already gives for it.
/// </para>
/// <para>
/// It was called "Terrain in 3D", which said the wrong thing twice: every GPU sample
/// here is drawn in 3D and most of them can be leaned over, so the words singled this
/// one out for the thing it shares with all of them, and said nothing about what it
/// alone has - a DEM under the imagery and a hillshade layer reading it.
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
    /// Mount Rainier, from the south. The camera sits south of the centre and looks
    /// north at it, which is the face the glaciers are on.
    /// <para>
    /// It replaced the Grand Canyon, which was the better picture and the worse sample:
    /// the canyon has almost no roads, so a street layer drawn over the imagery had
    /// nothing to draw and the one thing this sample newly shows - that lines and
    /// labels sit ON the ground rather than float above it - was invisible. Rainier has
    /// Paradise Road and Stevens Canyon Road switchbacking up 1,500 m of it, and a road
    /// that climbs is a road you can see climbing.
    /// </para>
    /// <para>
    /// The centre is not the summit. A tilted camera looks AT its centre, so that point
    /// lands mid-screen and whatever is nearer than it fills the lower half: centred on
    /// the summit, the mountain runs off the bottom of the screen and the top half is
    /// the forest behind it. Five kilometres short of the summit puts it a third of the
    /// way down, with the whole south face under it and Paradise Road at its foot.
    /// </para>
    /// </summary>
    private static readonly PointShape StartCenter = new(-13_554_290, 5_911_000);

    /// <summary>
    /// A scale rather than an extent, so every screen opens at the same zoom and a
    /// bigger one simply shows more ground. That matters here because the aerial
    /// imagery is two different pictures: true colour close in, and from a certain zoom
    /// outward a coarser mosaic in which Rainier's snow is teal. An extent is fitted to
    /// the screen, so the same rectangle lands a small phone further out than a large
    /// one. Not much further out than this and the mountain turns teal on all of them.
    /// </summary>
    private const double StartScale = 91_280;

    /// <summary>
    /// Low to the ground, because a mountain seen from above is a contour map. Flat, a
    /// DEM and a hillshade image look the same - the relief is in the shading either
    /// way, and nothing tells you the ground itself has height. At 78 the summit stands
    /// against the sky instead of sitting inside the frame, which is the whole reason
    /// to carry terrain at all.
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
        // of it. Lean the camera back towards vertical and all three come up solid,
        // because from above nothing can stand in front of anything.
        var markers = new SimpleMarkerOverlay();
        markers.AddMarker("muir", PlaceMarker(new PointShape(-13_551_044, 5_915_362), "Camp Muir"));
        markers.AddMarker("gibraltar", PlaceMarker(new PointShape(-13_553_148, 5_916_973), "Gibraltar Rock"));
        markers.AddMarker("liberty", PlaceMarker(new PointShape(-13_554_595, 5_920_718), "Liberty Cap"));
        map.Overlays.Add(markers);

        AddToPanel(BuildShadedReliefToggle());

        map.CenterPoint = StartCenter;
        map.MapScale = StartScale;

        // A mountain is worth leaning further over than a city, so this view lets the
        // hand go past the 60 degrees a map allows by default. Past that the horizon
        // comes in and there is no sky behind it, which is why it is not the default.
        map.MaxTiltAngle = 80;


        // The map opens flat and rises into the view rather than arriving in it. Two
        // fingers do the same thing by hand; the opening just shows that they can,
        // and a mountain coming up out of a flat map says more about what terrain is
        // than the finished picture does.
        map.TiltAngle = EntranceFromTilt;
        map.MapRotation = EntranceFromRotation;
        _ = map.RefreshAsync();
        _ = RunEntranceAsync(map);
    }

    /// <summary>
    /// Buildings, standing on the terrain in both styles: the ground under them is the
    /// same DEM whichever look is drawn over it, so they do not come and go with the
    /// switch. One neutral wall colour serves both. Over the photograph it is the only
    /// invented colour in the frame, so it is kept unsaturated; over the drawn map it
    /// is close to the flat building fill it stands on.
    /// <para>
    /// Solid, not translucent. A see-through block on a mountainside shows the slope
    /// behind it through its walls, and reads as a ghost of a building rather than as
    /// one; opaque, its faces take the light and it has a shape.
    /// </para>
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
    /// sample carries no image file and the label follows the theme.
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
    /// Leans and turns the camera into the opening view over about two and a half
    /// seconds.
    /// </summary>
    /// <remarks>
    /// Driven here rather than through ZoomToAsync because that animates the extent,
    /// not the tilt - and the extent is already where it belongs. Eased rather than
    /// linear: a camera that starts and stops abruptly reads as a jump even when it
    /// takes the same time, and the ease-out is what makes the end feel like arriving
    /// rather than stopping.
    /// </remarks>
    private static async Task RunEntranceAsync(MapView map)
    {
        const int steps = 48;
        const int frameMs = 16;

        // Long enough for the first tiles, so the lean is not spent on an empty map.
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
    /// Imagery over terrain, and no hillshade. A photograph already carries the sun
    /// that was shining when it was taken - the ridges have their own shadows and the
    /// slopes their own shading - so a hillshade layer on top of it shades the picture
    /// a second time and only greys it. It also arrives later than the imagery, after
    /// the DEM tiles land, so the map visibly dulls a moment after every move.
    /// </summary>
    private MapStyle ImageryStyle() =>
        new MapStyle()
            .AddRaster(new ThinkGeoRasterTileSource(SampleShared.CloudApiKey, ThinkGeoCloudRasterMapsMapType.Aerial_V2_X1),
                sourceId: "thinkgeo")
            .SetTerrain(_dem, 1.35)

            // Roads over the photograph, and nothing else from that document. Lines
            // only, no symbols: a road is draped on the terrain, so a ridge in front of
            // it hides it like anything else solid, while a label is a screen-space
            // billboard and the engine never depth-tests those - with symbols on, every
            // peak name behind the mountain floated through it.
            .AddStyle(StreetsStyleUri, null, StyleLayerType.Line)
            .AddLayer(BuildingLayer())
            .SetSky(GeoColor.FromHtml("#6FB2EF"), GeoColor.FromHtml("#EAF4FF"), 0.85);

    /// <summary>
    /// The drawn map, where hillshade earns its place. A vector style has no light in
    /// it at all - a glacier and a valley floor are the same flat colour - so the
    /// terrain under it is invisible until something shades it. This is the half of
    /// the switch that shows what the layer is for.
    /// </summary>
    private MapStyle VectorStyle() =>
        new MapStyle(StreetsStyleUri)
            .SetTerrain(_dem, 1.35)
            .AddStyle(HillshadeStyle)

            // After the hillshade, so the shade is not laid over the walls: it is a
            // picture of the ground's slope, and a wall is not ground.
            .AddLayer(BuildingLayer())
            .SetSky(GeoColor.FromHtml("#6FB2EF"), GeoColor.FromHtml("#EAF4FF"), 0.85);

    /// <summary>The switch between the two, and the point of the sample.</summary>
    private View BuildShadedReliefToggle()
    {
        var toggle = new Switch { IsToggled = false, VerticalOptions = LayoutOptions.Center };
        toggle.Toggled += async (_, e) =>
        {
            // The camera is not touched: only the look changes, so the view holds its
            // place, its scale and its lean across the swap.
            await _basemap.SetStyleAsync(e.Value ? VectorStyle() : ImageryStyle());
        };

        return new HorizontalStackLayout
        {
            Spacing = 8,
            Children =
            {
                new Label { Text = "Vector map with hillshade", FontSize = 13, VerticalOptions = LayoutOptions.Center },
                toggle,
            },
        };
    }
}
