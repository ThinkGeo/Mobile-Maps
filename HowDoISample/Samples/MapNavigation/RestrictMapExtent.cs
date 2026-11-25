using System.Globalization;
using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.Gpu;
using ThinkGeo.UI.Maui;

namespace HowDoISample.Samples;

/// <summary>
/// Keeping the map inside a box and between two scales, with the box drawn in red so
/// what is being enforced is visible.
/// <para>
/// Three properties and no code: <see cref="MapView.RestrictExtent"/>,
/// <see cref="MapView.MinScale"/> and <see cref="MapView.MaxScale"/>.
/// </para>
/// <para>
/// Restricted, not frozen: the map opens tilted and every gesture still works. The
/// box holds through all of it - the edge stops the pan under the finger rather than
/// letting it out and pulling it back on release. A tilted camera does see past the
/// box, towards the horizon; that is the view reaching further, not the map leaving
/// the box.
/// </para>
/// </summary>
public class RestrictMapExtent : SamplePage
{
    /// <summary>
    /// Downtown Dallas, TX: latitude 32.757 to 32.802, longitude -96.824 to -96.770,
    /// in EPSG:3857 - about five kilometres a side, from Victory Park across to Deep
    /// Ellum. Small enough that a pan reaches the edge in one stroke.
    /// </summary>
    private readonly RectangleShape _restrictExtent = new(-10778400, 3869100, -10772400, 3863100);

    // The scales are the box's: far enough out to see all of it with room around it,
    // far enough in to read a street.
    private const double MaxZoomOutScale = 50000;
    private const double MinZoomInScale = 2000;

    /// <summary>The tilt the map opens at.</summary>
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

        // Written from the same two constants the map is given.
        AddToPanel(new Label
        {
            Text = string.Format(CultureInfo.InvariantCulture,
                "Tilt it, turn it, zoom it - it stays inside the red box, between 1:{0:N0} and 1:{1:N0}.",
                MinZoomInScale, MaxZoomOutScale),
            FontSize = 13,
            TextColor = PanelInk,
        });

        // Opened a little outside the box rather than on it, so the red border is on
        // screen from the first frame. The restriction centres the view, since a view
        // this size cannot be held inside.
        var opening = (RectangleShape)_restrictExtent.CloneDeep();
        opening.ScaleUp(60);
        map.CurrentExtent = opening;

        // Set after the extent, not before it: while the view is still the whole
        // world the camera levels its tilt, so a tilt set first would be lost.
        map.TiltAngle = StartTilt;

        _ = map.RefreshAsync();
    }
}
