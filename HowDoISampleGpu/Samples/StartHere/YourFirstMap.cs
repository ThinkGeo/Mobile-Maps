using System.Text.Json.Nodes;
using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// The smallest thing that proves the chain: a MAUI MapView whose basemap is drawn
/// by the GPU pipeline. Everything specific to that is the one line that assigns the
/// basemap - the map view, the camera and the gestures are the ordinary MAUI ones.
/// <para>
/// It opens close in and already leaning, because the one thing worth seeing first
/// is that the city has height. The switch changes the whole map between the two
/// themes the same server publishes, which is what a style URI being the only input
/// actually buys: a different map is a different URI.
/// </para>
/// </summary>
public class YourFirstMap : ContentPage
{
    /// <summary>
    /// The same map in two themes, published side by side by the same server and
    /// declaring the same vector source - so the extrusion layer below is written
    /// once and reads whichever is loaded.
    /// </summary>
    private const string DarkStyleUri = "https://tiles.preludemaps.com/styles/WorldStreets_Dark/style.json";
    private const string LightStyleUri = "https://tiles.preludemaps.com/styles/WorldStreets_Light/style.json";

    /// <summary>The vector source both styles declare; the buildings come from it.</summary>
    private const string BuildingSource = "planet_06-20-2026";

    /// <summary>
    /// Over Union Square, looking north up Broadway at Midtown, in spherical mercator
    /// metres.
    /// <para>
    /// The vantage is the whole picture. A tilted camera sees the near side of the
    /// view at eye level and everything else behind it, so standing among towers -
    /// which is where this used to open, over Hudson Yards - puts three of them across
    /// the screen and the city behind them is gone. From here the near side is the
    /// Village and Union Square at six to fifteen storeys, the Flatiron blocks are
    /// behind that, and Midtown stands along the top: the height rises away from the
    /// camera instead of blocking it, which is the only arrangement in which a tilted
    /// map shows more than a flat one.
    /// </para>
    /// </summary>
    private static readonly PointShape HomeCenter = new(-8236529d, 4973874d);

    /// <summary>
    /// Close enough that the buildings are the view rather than a texture on it, and
    /// far enough that there is a view to be had. A level further in is a street
    /// canyon: whatever stands in the first block fills the screen, and the three
    /// kilometres of city that make the picture are behind it.
    /// </summary>
    private static readonly double HomeScale = new ThinkGeoCloudMapsZoomLevelSet().GetScales()[15];

    /// <summary>Flat, the extrusions are just roofs.</summary>
    private const double HomeTilt = 60;

    private readonly MapView _map;
    private readonly GpuBasemap _basemap;
    private bool _dark = true;

    public YourFirstMap()
    {
        Title = "Your First Map";
        Shell.SetNavBarIsVisible(this, false);

        // The map draws to the bottom edge of the screen rather than being inset
        // above the home indicator, as on every other sample page.
        Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific.Page.SetUseSafeArea(this, false);

        // And the home indicator is asked to dim itself, as on every sample page - see
        // SamplePage for what a NavigationPage does with that ask.
        Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific.Page.SetPrefersHomeIndicatorAutoHidden(this, true);

        _map = new MapView
        {
            MapUnit = GeographyUnit.Meter,
            IsRotationEnabled = true,
            CenterPoint = HomeCenter,
            MapScale = HomeScale,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
        };

        // A style URI and nothing else. GpuBasemap is the whole public surface of the
        // GPU story on MAUI as it is on the desktop, and a basemap is not one of the
        // Overlays - it is the ground they sit on.
        _basemap = new GpuBasemap(StyleFor(_dark));
        _map.Basemap = _basemap;

        // The map opens already leaning so the first thing it shows is the thing worth
        // showing. Two fingers sliding together change it from here.
        _map.TiltAngle = HomeTilt;

        // The map draws itself once it first has a size. Without this nothing is ever
        // asked to draw, and the page comes up empty with nothing in the log.
        _map.Behaviors.Add(new AutoLoadMapViewBehavior());

        Content = new Grid { Children = { _map, BuildThemeToggle() } };

        // As on every sample page - see SamplePage.FillBottomSafeArea.
        Loaded += (_, _) => SamplePage.FillBottomSafeArea(_map);
    }

    /// <summary>
    /// One of the two published styles, with a 3D building layer added on top of it.
    /// <para>
    /// Neither style has any 3D in it, but the tiles behind both do: the building layer
    /// carries render_height, render_min_height and hide_3d, so one extrusion layer is
    /// enough to stand the city up. The buildings are only in the tiles from zoom 13,
    /// so the layer waits for it.
    /// </para>
    /// </summary>
    /// <summary>
    /// The two themes, each built once and kept.
    /// <para>
    /// A MapStyle made from a URI has to fetch and parse that document before the map
    /// can draw it - measured at 659 ms on an iPad for this one - and building a fresh
    /// one on every flip paid that again each time, for a document that had not
    /// changed. Held here, the first flip pays it and no later one does.
    /// </para>
    /// </summary>
    private MapStyle StyleFor(bool dark)
        => dark ? _darkStyle ??= BuildStyle(true) : _lightStyle ??= BuildStyle(false);

    private MapStyle _darkStyle;
    private MapStyle _lightStyle;

    private static MapStyle BuildStyle(bool dark)
    {
        var style = new MapStyle(dark ? DarkStyleUri : LightStyleUri);

        style.AddLayer(new JsonObject
        {
            ["id"] = "building-3d",
            ["type"] = "fill-extrusion",
            ["source"] = BuildingSource,
            ["source-layer"] = "building",
            ["minzoom"] = 13,
            ["filter"] = new JsonArray("!=", "hide_3d", true),
            ["paint"] = new JsonObject
            {
                // Walls the theme's own buildings would have had: lit against a light
                // map, and a shade above the dark map's background against a dark one.
                ["fill-extrusion-color"] = dark ? "#39414B" : "#CFD3DA",
                ["fill-extrusion-height"] = new JsonArray("get", "render_height"),
                ["fill-extrusion-base"] = new JsonArray("get", "render_min_height"),
                ["fill-extrusion-opacity"] = 0.95,
            },
        });

        return style;
    }

    /// <summary>A small card over the map, out of the way of the gestures.</summary>
    private View BuildThemeToggle()
    {
        var toggle = new Switch { IsToggled = _dark, VerticalOptions = LayoutOptions.Center };
        toggle.Toggled += async (_, e) =>
        {
            if (e.Value == _dark) return;
            _dark = e.Value;

            // The camera is not touched: the style is what changes, so the view holds
            // its place, its scale and its lean across the swap.
            //
            // And nothing is asked for after it. SetStyleAsync already redraws; a
            // RefreshAsync behind it starts a SECOND pass over the same view, which
            // supersedes the first one mid-build - measured on this swap: two tile
            // generations instead of one, six tile builds cancelled after they had
            // been started, and a fetch abandoned. The swap gets slower by being
            // hurried.
            await _basemap.SetStyleAsync(StyleFor(_dark));
        };

        var card = new HorizontalStackLayout
        {
            Spacing = 8,
            Padding = new Thickness(12, 6),
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.End,
            Margin = new Thickness(16, 0, 0, 12),
            Children =
            {
                new Label { Text = "Dark theme", FontSize = 13, VerticalOptions = LayoutOptions.Center },
                toggle,
            },
        };

        // Readable in both themes: the card sits on whatever the map is drawing.
        card.SetAppThemeColor(VisualElement.BackgroundColorProperty, Color.FromArgb("#F2FFFFFF"), Color.FromArgb("#F2202124"));
        ((Label)card.Children[0]).SetAppThemeColor(Label.TextColorProperty, Color.FromArgb("#1F2328"), Color.FromArgb("#E6E6E6"));

        return card;
    }
}
