using System.Text.Json.Nodes;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Devices.Sensors;
using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.UI.Maui;

namespace HowDoISample.Samples;

/// <summary>
/// The city with its real height. One fill-extrusion layer over a published style
/// stands Manhattan up, and the map opens already leaning because the lean is what
/// makes the height visible - the companion to Terrain and Hillshade, which does the
/// same for the shape of the ground.
/// <para>
/// The switch changes the whole map between the two themes the same server
/// publishes, which is what a style URI being the only input actually buys: a
/// different map is a different URI. It is also the shortest path onto a GPU map, and
/// was this gallery's opening sample for that reason: everything specific to the GPU
/// is the one line that assigns the basemap, and the map view, the camera and the
/// gestures are the ordinary MAUI ones.
/// </para>
/// </summary>
public class BuildingsIn3D : ContentPage
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
    /// Fifth Avenue in the eighties, where the Upper East Side meets Central Park, in
    /// spherical mercator metres.
    /// <para>
    /// The vantage is the whole picture. A tilted camera sees the near side of the
    /// view at eye level and everything else behind it, so standing among towers -
    /// which is where this used to open, over Hudson Yards - puts three of them across
    /// the screen and the city behind them is gone. Union Square, which came after
    /// that, solved the blocking but filled every corner with blocks that all stand
    /// about the same height.
    /// </para>
    /// <para>
    /// The park is what this view adds. It puts an empty half against a dense one, so
    /// the city has an edge to be read against instead of running out of frame on all
    /// four sides, and the reservoir gives the eye somewhere flat to measure the
    /// skyline from. It is also the cheaper half: a park carries no buildings, so
    /// roughly half the frame costs nothing to extrude, and the same tilt buys a
    /// longer view for fewer features.
    /// </para>
    /// <para>
    /// North stays up, so the park falls on the left of the frame and the avenues run
    /// diagonally - that is Manhattan's grid, which is about 29 degrees off true
    /// north, not a camera rotation.
    /// </para>
    /// </summary>
    private static readonly PointShape HomeCenter = new(-8233300d, 4980400d);

    /// <summary>
    /// Close enough that the buildings are the view rather than a texture on it, and
    /// far enough that there is a view to be had. A level further in is a street
    /// canyon: whatever stands in the first block fills the screen, and the three
    /// kilometres of city that make the picture are behind it.
    /// </summary>
    private static readonly double HomeScale = new ThinkGeoCloudMapsZoomLevelSet().GetScales()[15];

    /// <summary>Flat, the extrusions are just roofs.</summary>
    private const double HomeTilt = 60;

    /// <summary>Where the camera goes once it knows where you are: a few blocks.</summary>
    private static readonly double LocatedScale = new ThinkGeoCloudMapsZoomLevelSet().GetScales()[16];

    /// <summary>The pulsing halo's resting diameter, in units of the dot it surrounds.</summary>
    private const double PuckSize = 64;

    private readonly MapView _map;
    private readonly GpuBasemap _basemap;
    private readonly SimpleMarkerOverlay _markers = new();
    private Marker _puck;
    private Label _locateStatus;
    private View _locateIcon;
    private bool _locating;
    private bool _dark = true;

    public BuildingsIn3D()
    {
        Title = "Buildings in 3D";
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

        // The puck is a view on the map rather than a drawing in it, so it can be a
        // MAUI animation - see Markers for the same trick on an ordinary pin.
        _map.Overlays.Add(_markers);

        Content = new Grid { Children = { _map, BuildAttribution(), BuildThemeToggle(), BuildLocateButton(), BuildLocateStatus() } };

        // As on every sample page - see SamplePage.FillSafeArea.
        Loaded += (_, _) => SamplePage.FillSafeArea(_map);
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

    /// <summary>
    /// The round button in the bottom right corner, the one every map has: it asks the
    /// phone where it is and takes the camera there.
    /// </summary>
    /// <remarks>
    /// The icon is drawn rather than loaded - a ring, a dot and four ticks, which is
    /// what this button looks like everywhere - so the page carries no image file and
    /// the colour follows the theme like everything else on it.
    /// </remarks>
    private View BuildLocateButton()
    {
        var ring = new Ellipse
        {
            Stroke = new SolidColorBrush(Color.FromArgb("#1E88E5")),
            StrokeThickness = 2,
            WidthRequest = 18,
            HeightRequest = 18,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
        };

        var centre = new Ellipse
        {
            Fill = new SolidColorBrush(Color.FromArgb("#1E88E5")),
            WidthRequest = 6,
            HeightRequest = 6,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
        };

        var icon = new Grid
        {
            WidthRequest = 26,
            HeightRequest = 26,
            Children = { ring, centre },
        };

        // The four ticks that make a ring read as a sight rather than as a circle.
        foreach (var (horizontal, vertical) in new[]
                 {
                     (LayoutOptions.Center, LayoutOptions.Start),
                     (LayoutOptions.Center, LayoutOptions.End),
                     (LayoutOptions.Start, LayoutOptions.Center),
                     (LayoutOptions.End, LayoutOptions.Center),
                 })
        {
            var along = horizontal == LayoutOptions.Center;
            icon.Children.Add(new Rectangle
            {
                Fill = new SolidColorBrush(Color.FromArgb("#1E88E5")),
                WidthRequest = along ? 2 : 5,
                HeightRequest = along ? 5 : 2,
                HorizontalOptions = horizontal,
                VerticalOptions = vertical,
            });
        }

        _locateIcon = icon;

        var button = new Border
        {
            Content = icon,
            Padding = new Thickness(11),
            StrokeThickness = 0,
            StrokeShape = new RoundRectangle { CornerRadius = 24 },
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.End,
            Margin = new Thickness(0, 0, 16, 12),
        };

        button.SetAppThemeColor(VisualElement.BackgroundColorProperty,
            Color.FromArgb("#F2FFFFFF"), Color.FromArgb("#F2202124"));

        var tap = new TapGestureRecognizer();
        tap.Tapped += async (_, _) => await LocateAsync();
        button.GestureRecognizers.Add(tap);

        return button;
    }

    /// <summary>
    /// What the button has to say, and only while it has something to say. Locating
    /// can be refused, can time out and can take a few seconds, and a button that
    /// gives no account of any of that reads as broken.
    /// </summary>
    private View BuildLocateStatus()
    {
        _locateStatus = new Label
        {
            FontSize = 12,
            IsVisible = false,
            Padding = new Thickness(12, 6),
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.End,
            Margin = new Thickness(0, 0, 0, 68),
        };

        _locateStatus.SetAppThemeColor(VisualElement.BackgroundColorProperty,
            Color.FromArgb("#F2FFFFFF"), Color.FromArgb("#F2202124"));
        _locateStatus.SetAppThemeColor(Label.TextColorProperty,
            Color.FromArgb("#1F2328"), Color.FromArgb("#E6E6E6"));

        return _locateStatus;
    }

    private void ShowLocateStatus(string text)
    {
        _locateStatus.Text = text;
        _locateStatus.IsVisible = true;
    }

    /// <summary>
    /// Ask the phone where it is, and put the camera and the puck there.
    /// </summary>
    /// <remarks>
    /// The permission is asked for rather than assumed, and a refusal is an answer
    /// rather than an error: the button says so and the map stays where it was. The
    /// manifest and the Info.plist declare what is being asked for, which is what makes
    /// the ask legal to make at all.
    /// </remarks>
    private async Task LocateAsync()
    {
        if (_locating)
        {
            return;
        }

        _locating = true;
        _locateIcon.Opacity = 0.35;
        ShowLocateStatus("Finding you\u2026");

        try
        {
            var permission = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (permission != PermissionStatus.Granted)
            {
                permission = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            }

            if (permission != PermissionStatus.Granted)
            {
                ShowLocateStatus("Location is turned off for this app");
                return;
            }

            // The last fix first, because it is instant and is usually good enough to
            // put the camera in the right city while the radio works on the real one.
            var location = await Geolocation.Default.GetLastKnownLocationAsync()
                ?? await Geolocation.Default.GetLocationAsync(
                    new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(12)));

            if (location == null)
            {
                ShowLocateStatus("No fix yet - try again with a view of the sky");
                return;
            }

            var world = ToMercator(location.Longitude, location.Latitude);
            ShowPuck(world);

            // The lean is kept: this map's subject is the height, and arriving flat
            // would throw that away to show the same thing every other map shows.
            _map.CenterPoint = world;
            _map.MapScale = LocatedScale;
            await _map.RefreshAsync();

            ShowLocateStatus(location.Accuracy is > 0
                ? FormattableString.Invariant($"Here, to about {location.Accuracy:N0} m")
                : "Here");
        }
        catch (FeatureNotSupportedException)
        {
            ShowLocateStatus("This device has no location service");
        }
        catch (Exception exception)
        {
            ShowLocateStatus("Location failed: " + exception.Message.Split('\n')[0]);
        }
        finally
        {
            _locating = false;
            _locateIcon.Opacity = 1;
        }
    }

    /// <summary>Longitude and latitude as the map's own metres.</summary>
    private static PointShape ToMercator(double longitude, double latitude)
    {
        var converter = new ProjectionConverter(4326, 3857);
        converter.Open();

        try
        {
            var vertex = converter.ConvertToExternalProjection(longitude, latitude);
            return new PointShape(vertex.X, vertex.Y);
        }
        finally
        {
            converter.Close();
        }
    }

    /// <summary>
    /// The blue dot, with the halo that breathes out of it. A second fix moves the one
    /// that is there rather than leaving a trail of them.
    /// </summary>
    private void ShowPuck(PointShape position)
    {
        if (_puck != null)
        {
            _puck.Position = position;
            _ = _markers.RefreshAsync();
            return;
        }

        var halo = new Ellipse
        {
            Fill = new SolidColorBrush(Color.FromRgba(30, 136, 229, 90)),
            WidthRequest = 22,
            HeightRequest = 22,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
        };

        var dot = new Ellipse
        {
            Fill = new SolidColorBrush(Color.FromArgb("#1E88E5")),
            Stroke = new SolidColorBrush(Colors.White),
            StrokeThickness = 3,
            WidthRequest = 20,
            HeightRequest = 20,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
        };

        _puck = new Marker
        {
            Position = position,
            WidthRequest = PuckSize,
            HeightRequest = PuckSize,
            Content = new Grid { WidthRequest = PuckSize, HeightRequest = PuckSize, Children = { halo, dot } },
        };

        // Out and away to nothing, then again from the dot: the halo is a wave leaving
        // the fix rather than a circle changing size, which is the difference between
        // "it is here" and "something is blinking".
        _puck.Loaded += async (_, _) =>
        {
            while (_puck != null && _puck.IsLoaded)
            {
                halo.Scale = 1;
                halo.Opacity = 0.9;
                await Task.WhenAll(
                    halo.ScaleTo(2.6, 1100, Easing.SinOut),
                    halo.FadeTo(0, 1100, Easing.SinOut));
                await Task.Delay(400);
            }
        };

        _markers.AddMarker(Guid.NewGuid().ToString(), _puck);
        _ = _markers.RefreshAsync();
    }

    /// <summary>
    /// Credit for the data, over the map and never hidden. Both published styles draw
    /// OpenStreetMap, the building heights included, and the ODbL it is licensed under
    /// asks for the credit to be on the map. This page is not a SamplePage, so it
    /// carries its own rather than inheriting one.
    /// </summary>
    private View BuildAttribution()
    {
        var credit = new Label
        {
            Text = "\u00a9 OpenStreetMap contributors",
            FontSize = 10,
            Padding = new Thickness(6, 2),
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.End,
            Margin = new Thickness(0, 0, 16, 68),
        };

        credit.SetAppThemeColor(VisualElement.BackgroundColorProperty,
            Color.FromRgba(255, 255, 255, 190), Color.FromRgba(32, 33, 36, 190));
        credit.SetAppThemeColor(Label.TextColorProperty,
            Color.FromArgb("#3C4043"), Color.FromArgb("#E6E6E6"));

        return credit;
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
