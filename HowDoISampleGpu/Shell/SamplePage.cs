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
        };

        // The map draws itself once it first has a size. Without this nothing is ever
        // asked to draw, and the page comes up empty with nothing in the log.
        Map.Behaviors.Add(new AutoLoadMapViewBehavior());

        _panelContent = new VerticalStackLayout { Spacing = 6 };
        _panel = new Border
        {
            // Scrolled and capped: a sample whose controls are a list - every layer of
            // a style, say - would otherwise grow the card over the whole map.
            Content = new ScrollView { Content = _panelContent, MaximumHeightRequest = 260 },
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
    protected abstract void BuildMap(MapView map);
}
