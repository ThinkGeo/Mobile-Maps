using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.Gpu;
using ThinkGeo.UI.Maui;

namespace HowDoISample.Samples;

/// <summary>
/// The shape the world is drawn in. The same map morphs between a sphere and three
/// projections, drawn either from satellite imagery or from vector streets - the
/// projection is the camera's, not the data's, so nothing is reprojected and no tile
/// is rebuilt to change it.
/// </summary>
public class TheGlobe : SamplePage
{
    private const double HalfWorld = 20037508.342789244;

    private static readonly TimeSpan MorphDuration = TimeSpan.FromMilliseconds(900);

    /// <summary>
    /// How much of the world the zoomed-out window holds, as a multiple of its width.
    /// <para>
    /// Above 1 for the flat shapes: fitting the world exactly crops the ones that are
    /// not rectangles, because Mollweide's ellipse and Robinson's rounded edges reach
    /// their widest at the equator and at 1.0 their left and right tips are off the
    /// window. Well below 1 for the sphere, which is the one shape whose drawn size is
    /// not the window's: a globe is the world wrapped round a ball a third of the
    /// world's width across, so framing the full world leaves it a marble in the
    /// middle of a black screen.
    /// </para>
    /// </summary>
    private const double FlatMargin = 1.25;
    private const double GlobeMargin = 0.60;

    /// <summary>
    /// Four shapes, not forty. A sphere, the projection every web map uses, and two
    /// world projections that look nothing like it - enough to show that
    /// <see cref="DisplayProjection.FromProjString"/> takes whatever proj can
    /// describe, without turning the sample into a list of names.
    /// </summary>
    /// <remarks>
    /// What each one gives up, which is the only thing a projection is a choice
    /// about: a sphere gives up nothing and cannot be printed, which is why the other
    /// three exist. Web Mercator keeps angles right everywhere, so a square mile is
    /// square - and Greenland comes out the size of Africa. Robinson is right about
    /// nothing in particular, which is what makes it look right. Mollweide gives
    /// every country its true share of the page and pays for it at the edges, where
    /// shapes shear.
    /// <para>
    /// This used to be four sentences on the card, one showing at a time. On a phone
    /// the card is the map's screen space, and four lines of prose about a projection
    /// cost more of it than the projection is worth looking at.
    /// </para>
    /// </remarks>
    private static readonly (string Name, string Definition)[] Choices =
    {
        ("Globe", "globe"),
        ("Web Mercator", "3857"),
        ("Robinson", "+proj=robin +datum=WGS84 +units=m +no_defs"),
        ("Mollweide", "+proj=moll +datum=WGS84 +units=m +no_defs"),
    };

    private MapView _map;
    private string _choice = "globe";
    private bool _raster = true;

    /// <summary>
    /// The two maps, each built once and kept. A MapStyle made from a URI has to fetch
    /// that document, its sprite sheet and its glyph ranges and then compile it before
    /// the map can draw a tile - and building a fresh one on every tap of the strip
    /// paid all of that again each time, for documents that had not changed. Held
    /// here, the first switch pays it and no later one does.
    /// </summary>
    private MapStyle _rasterStyle;
    private MapStyle _vectorStyle;

    protected override void BuildMap(MapView map)
    {
        _map = map;
        map.MapUnit = GeographyUnit.Meter;

        // Two rows and nothing else: which shape, and which map. Both are the
        // gallery's own choice strip - see SamplePage.AddChoiceStrip.
        AddChoiceStrip(Choices.Select(choice => choice.Name).ToList(), 0,
            index => _ = MorphAsync(Choices[index].Definition));
        AddChoiceStrip(new[] { "Satellite imagery", "Vector streets" }, 0,
            index => _ = SetRendererAsync(index == 0));

        map.Basemap = new GpuBasemap(BuildStyle()) { DisplayProjection = DisplayProjection.Globe };

        SetAttribution("Imagery \u00a9 ThinkGeo");
        FrameWorld(_choice);
        _ = map.RefreshAsync();
    }

    /// <summary>
    /// The two maps this sample can draw itself from. Swapped whole rather than
    /// patched: a raster registration and a vector style have no source in common, so
    /// there is nothing to carry from one to the other - but each is kept, because
    /// what a style costs to build it costs once.
    /// </summary>
    private MapStyle BuildStyle()
    {
        if (_raster)
        {
            return _rasterStyle ??= BuildRasterStyle();
        }

        return _vectorStyle ??= BuildVectorStyle();
    }

    private static MapStyle BuildRasterStyle()
    {
        var imagery = new ThinkGeoRasterTileSource(SampleShared.CloudApiKey, ThinkGeoCloudRasterMapsMapType.Aerial_V2_X2);
        return new MapStyle().AddRaster(imagery, sourceId: "thinkgeo");
    }

    private static MapStyle BuildVectorStyle()
    {
        // Light, not dark. The switch is between two drawings of the same world, and
        // the one it replaces is daylit satellite imagery - a dark map in its place
        // reads as the lights going out rather than as the same globe drawn another
        // way, which is the one thing this switch is here to show.
        return new MapStyle(ThinkGeoVectorStyles.Light, new ThinkGeoVectorTileSource(SampleShared.CloudApiKey)
        {
            VectorTileCache = new FileTileCache(SampleShared.VectorTileCacheFolder),
        });
    }

    /// <summary>
    /// Swaps the map underneath without touching the camera or the shape it is drawn
    /// in: the projection lives on the basemap, not in the style, so a restyle leaves
    /// the globe a globe.
    /// </summary>
    private async Task SetRendererAsync(bool raster)
    {
        if (raster == _raster) return;
        _raster = raster;

        // No refresh behind it: SetStyleAsync redraws, and a second pass over the
        // same view only supersedes the first one mid-build.
        await _map.Basemap.SetStyleAsync(BuildStyle());

        SetAttribution(_raster
            ? "Imagery \u00a9 ThinkGeo"
            : DefaultAttribution);
    }

    private async Task MorphAsync(string definition)
    {
        if (definition == _choice) return;
        _choice = definition;

        var target = definition switch
        {
            "globe" => DisplayProjection.Globe,
            "3857" => DisplayProjection.WebMercator,
            _ => DisplayProjection.FromProjString(definition),
        };

        // Every shape opens on the whole world. Holding the camera still reads well
        // between two outlines of similar proportion and badly between four: a view
        // framed for a globe leaves Mollweide's ellipse in a corner of a black
        // window, because the two do not cover the same ground at the same scale.
        FrameWorld(definition);
        await _map.RefreshAsync();

        try
        {
            // The morph itself - the shape under the camera changes rather than the
            // map being redrawn.
            await _map.Basemap.AnimateDisplayProjectionAsync(target, MorphDuration);
        }
        catch (OperationCanceledException)
        {
            // Another projection was picked before this morph finished.
        }
    }

    private double WorldFillScale(double margin)
    {
        var world = new RectangleShape(-HalfWorld, HalfWorld, HalfWorld, -HalfWorld);
        return MapUtil.GetScale(world, Math.Max(1, _map.CanvasWidth), GeographyUnit.Meter) * margin;
    }

    /// <summary>
    /// The whole world, centred on its own origin, so every shape arrives whole and
    /// in the middle of the window.
    /// </summary>
    /// <remarks>
    /// Zoom-out stops where the world fills the window: a projected canvas is not
    /// cyclic, so there is nothing to show past its edge, and zooming out further
    /// only adds black. Web Mercator is cyclic, so it keeps its freedom.
    /// </remarks>
    private void FrameWorld(string definition)
    {
        var margin = definition == "globe" ? GlobeMargin : FlatMargin;
        var widthInMeters = HalfWorld * 2 * margin;
        var height = widthInMeters * _map.CanvasHeight / Math.Max(1, _map.CanvasWidth);

        _map.MaxScale = definition == "3857" ? double.MaxValue : WorldFillScale(margin);
        _map.CurrentExtent = new RectangleShape(
            widthInMeters * -0.5, height * 0.5, widthInMeters * 0.5, height * -0.5);
    }
}
