using System.Globalization;
using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.Gpu;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// Keeping the map inside a box and between two scales, with the box drawn in red so
/// what is being enforced is visible.
/// <para>
/// Three properties and no code: <see cref="MapView.RestrictExtent"/>,
/// <see cref="MapView.MinScale"/> and <see cref="MapView.MaxScale"/>, which is the
/// same trio the WPF map has. They were switches at first, one per restriction, which
/// asked the reader to turn a thing on before they could see what it did - and a
/// restriction that is off is a map that behaves normally, which is not worth a
/// control. The card says what the map will not let you do; the map then does not let
/// you do it, and the red box shows where the first of the three bites.
/// </para>
/// <para>
/// Restricted, not frozen: the map opens leaning and every gesture still works. Two
/// fingers turn it, two fingers sliding together change the lean, and the box holds
/// through all of it - the edge stops the pan under the finger rather than letting it
/// out and pulling it back on release. A lean does let the camera SEE past the box,
/// into the far field above the horizon; that is the view reaching further, not the
/// map leaving the box, and a restriction that forbade it would be a restriction on
/// the lens.
/// </para>
/// </summary>
public class RestrictMapExtent : SamplePage
{
    /// <summary>
    /// Downtown Dallas, TX: latitude 32.757 to 32.802, longitude -96.824 to -96.770,
    /// in EPSG:3857 - about five kilometres a side, from Victory Park across to Deep
    /// Ellum.
    /// <para>
    /// A city rather than a metroplex. The box was the whole of Dallas-Fort Worth
    /// before, fifty kilometres across, and at that size the restriction is something
    /// you have to go looking for: a thumb cannot reach the edge in one stroke, so the
    /// sample read as an ordinary map with a red line somewhere off screen. Small
    /// enough to hit is what makes it a demonstration.
    /// </para>
    /// </summary>
    private readonly RectangleShape _restrictExtent = new(-10778400, 3869100, -10772400, 3863100);

    // The scales are the box's: far enough out to see all of it with room around it,
    // far enough in to read a street. They move with the box, so a smaller box gets
    // tighter limits rather than keeping a metroplex's.
    private const double MaxZoomOutScale = 50000;
    private const double MinZoomInScale = 2000;

    /// <summary>Leaning enough to read as a camera rather than a sheet of paper.</summary>
    private const double StartTilt = 45;

    protected override void BuildMap(MapView map)
    {
        map.MapUnit = GeographyUnit.Meter;

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

        // The three restrictions. RestrictExtent is the box; MaxScale stops zooming
        // out beyond a scale (a larger number is further out) and MinScale stops
        // zooming in past one.
        map.RestrictExtent = _restrictExtent;
        map.MaxScale = MaxZoomOutScale;
        map.MinScale = MinZoomInScale;

        // The one line on the card, written from the same two constants the map is
        // given, so what it promises cannot drift from what it does.
        AddToPanel(new Label
        {
            Text = string.Format(CultureInfo.InvariantCulture,
                "Tilt it, turn it, zoom it - it stays inside the red box, between 1:{0:N0} and 1:{1:N0}.",
                MinZoomInScale, MaxZoomOutScale),
            FontSize = 13,
        });

        // Opened a little outside the box rather than on it, so the red border is on
        // screen from the first frame: a sample that says "keep it inside the box"
        // and opens with the box past all four edges has only the sentence. The
        // restriction centres it, since a view this size cannot be held inside.
        var opening = (RectangleShape)_restrictExtent.CloneDeep();
        opening.ScaleUp(60);
        map.CurrentExtent = opening;

        // Leaning from the first frame, and free to be turned and leaned further. A
        // restriction sample that opened flat and held still would never show that the
        // box is the only thing being taken away.
        //
        // After the extent, not before it. The GPU camera levels its own tilt towards
        // nothing while the view it is given is still the whole world - that is what
        // keeps a globe from leaning - so a tilt set before the map has been told
        // where to look is a tilt multiplied by nearly zero, and the map came up flat.
        map.TiltAngle = StartTilt;

        _ = map.RefreshAsync();
    }
}
