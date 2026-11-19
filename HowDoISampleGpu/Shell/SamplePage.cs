using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu;

/// <summary>
/// What every sample page shares: a full-bleed MapView that has a size before the
/// sample touches it, and a place to put the sample's own controls.
/// <para>
/// The desktop gallery repeats a <c>Map_SizeChanged</c> guard in each sample, because
/// a MapView is configured once it first has a size. Here the wait is in one place
/// and a sample only writes <see cref="BuildMap"/>.
/// </para>
/// <para>
/// The desktop gallery also gives each sample a panel down the right-hand edge. A
/// phone has no edge to spare, so <see cref="Panel"/> is a card over the bottom of
/// the map instead - same controls, same order, somewhere they fit.
/// </para>
/// </summary>
public abstract class SamplePage : ContentPage
{
    private readonly VerticalStackLayout _panelContent;
    private readonly Border _panel;

    protected SamplePage()
    {
        Map = new MapView
        {
            MapUnit = GeographyUnit.Meter,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,

            // Off by default on MapView, because turning the map is not something
            // every app wants. A gallery is the one place that does: two fingers
            // twist to rotate, and slide together to tilt.
            IsRotationEnabled = true,
        };

        // The map draws itself once it first has a size. Without this nothing is ever
        // asked to draw, and the page comes up empty with nothing in the log.
        Map.Behaviors.Add(new AutoLoadMapViewBehavior());

        _panelContent = new VerticalStackLayout { Spacing = 6 };
        _panel = new Border
        {
            // Scrolled and capped: a sample whose controls are a list - every layer of
            // a style, say - would otherwise grow the card over the whole map.
            //
            // The cap is on the card and the ScrollView is told to wrap. A ScrollView
            // left to itself asks for all the height there is on Android, so the card
            // stood at the full 260 whatever it held - a third of a phone screen of
            // white. iOS wrapped it and hid the difference. VerticalOptions.Start makes
            // the ScrollView ask for its content instead, and the cap on the card still
            // takes over when the content is taller than that.
            Content = new ScrollView { Content = _panelContent, VerticalOptions = LayoutOptions.Start },
            MaximumHeightRequest = 260,
            BackgroundColor = Color.FromRgba(255, 255, 255, 235),
            StrokeThickness = 0,
            Padding = new Thickness(12, 10),
            Margin = new Thickness(10, 0, 10, 12),
            VerticalOptions = LayoutOptions.End,
            IsVisible = false,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 12 },
        };

        Content = new Grid { Children = { Map, _panel } };

        var built = false;
        Map.SizeChanged += (_, _) =>
        {
            if (built || Map.Width <= 0 || Map.Height <= 0) return;
            built = true;
            BuildMap(Map);
        };
    }

    /// <summary>
    /// Runs something once the map has drawn for the first time. An interactive
    /// overlay given its content before that first draw stops the GPU basemap from
    /// starting at all - no error, no crash, just a page with no map on it - so a
    /// sample that wants shapes on screen from the start puts them there once the
    /// ground beneath them exists.
    /// </summary>
    protected static void WhenFirstDrawn(MapView map, Action action)
    {
        void Handler(object sender, OverlaysDrawnMapViewEventArgs e)
        {
            map.OverlayDrawn -= Handler;
            action();
        }

        map.OverlayDrawn += Handler;
    }

    /// <summary>The map this page draws into.</summary>
    protected MapView Map { get; }

    /// <summary>
    /// Moves the control card to the top of the map. The default is the bottom, which
    /// is where a thumb reaches; a sample whose own output lives down there - a scale
    /// line, a scale bar, an attribution - moves it out of the way instead.
    /// </summary>
    protected void PutPanelAtTop() => _panel.VerticalOptions = LayoutOptions.Start;

    /// <summary>
    /// Adds a control to the card over the bottom of the map. The card appears with
    /// its first control, so a sample with none has a map and nothing else.
    /// </summary>
    protected T AddToPanel<T>(T view) where T : View
    {
        _panelContent.Add(view);
        _panel.IsVisible = true;
        return view;
    }

    /// <summary>A one-line status label on the panel, for what a sample wants to say.</summary>
    protected Label AddStatusLabel()
        => AddToPanel(new Label { FontSize = 12, TextColor = Colors.DimGray, LineBreakMode = LineBreakMode.TailTruncation });

    /// <summary>
    /// A picker over named choices, the mobile shape of the desktop gallery's radio
    /// button column. The callback runs on the UI thread with the chosen key.
    /// </summary>
    protected Picker AddChoicePicker(IReadOnlyList<string> choices, int selectedIndex, Action<int> chosen)
    {
        var picker = new Picker { FontSize = 14 };
        foreach (var choice in choices) picker.Items.Add(choice);
        picker.SelectedIndex = selectedIndex;
        picker.SelectedIndexChanged += (_, _) =>
        {
            if (picker.SelectedIndex >= 0) chosen(picker.SelectedIndex);
        };
        return AddToPanel(picker);
    }

    /// <summary>
    /// Builds the sample, once the map has a real size. The counterpart of the desktop
    /// gallery's <c>Map_SizeChanged</c> body.
    /// </summary>
    /// <summary>
    /// A sample's map holds GPU textures and a tile cache, and leaving the page does
    /// not free them: the managed object becomes unreachable but the native memory
    /// behind it is not what the collector measures, so nothing hurries. Opening
    /// sample after sample grew the footprint until iOS killed the process - a
    /// SIGKILL, with no crash and no stack, which is what made it look like a bug
    /// elsewhere. The page releases its own map on the way out instead.
    /// </summary>
    protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
    {
        base.OnNavigatedFrom(args);

        if (Navigation.NavigationStack.Contains(this))
            return;   // pushed over, not popped - the page is coming back

        ReleaseMap();
    }

    private void ReleaseMap()
    {
        if (_released)
            return;

        _released = true;

        foreach (var map in this.GetVisualTreeDescendants().OfType<MapView>().ToList())
        {
            map.Basemap?.Dispose();
            map.Basemap = null;
            map.Dispose();
        }

        (this as IDisposable)?.Dispose();
    }

    private bool _released;

    protected abstract void BuildMap(MapView map);
}
