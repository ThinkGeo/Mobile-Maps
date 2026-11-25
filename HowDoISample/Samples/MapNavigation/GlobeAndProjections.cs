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
public class GlobeAndProjections : SamplePage
{
    private const double HalfWorld = 20037508.342789244;

    private static readonly TimeSpan MorphDuration = TimeSpan.FromMilliseconds(900);

    /// <summary>
    /// How much of the world the zoomed-out window holds, as a multiple of its width.
    /// <para>
    /// Above 1 for the flat shapes: fitting the world exactly crops the ones that are
    /// not rectangles, because Mollweide's ellipse and Robinson's rounded edges reach
    /// their widest at the equator. Well below 1 for the sphere: a globe is the world
    /// wrapped round a ball a third of the world's width across, so framing the full
    /// world would leave it small in the middle of the screen.
    /// </para>
    /// </summary>
    private const double FlatMargin = 1.25;
    private const double GlobeMargin = 0.60;

    /// <summary>
    /// A sphere, the projection every web map uses, and two world projections that
    /// look nothing like it - <see cref="DisplayProjection.FromProjString"/> takes
    /// whatever proj can describe.
    /// </summary>
    /// <remarks>
    /// What each one gives up: a sphere gives up nothing and cannot be printed. Web
    /// Mercator keeps angles right everywhere - and Greenland comes out the size of
    /// Africa. Robinson is exact about nothing in particular, which is what makes it
    /// look right. Mollweide gives every country its true share of the page and pays
    /// for it at the edges, where shapes shear.
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
    /// The two maps, each built once and kept: a MapStyle made from a URI fetches that
    /// document, its sprite sheet and its glyph ranges and compiles it before the map
    /// can draw a tile, so a style built on every switch would pay for that each time.
    /// </summary>
    private MapStyle _rasterStyle;
    private MapStyle _vectorStyle;

    protected override void BuildMap(MapView map)
    {
        _map = map;
        map.MapUnit = GeographyUnit.Meter;

        // Two rows: which shape, and which map.
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
    /// patched: a raster registration and a vector style have no source in common.
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
        // Light, to sit beside daylit satellite imagery.
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

        // SetStyleAsync redraws the map itself, so no RefreshAsync follows it.
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

        // Every shape opens on the whole world: the shapes do not cover the same
        // ground at the same scale, so a view framed for one would leave another off
        // centre.
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
