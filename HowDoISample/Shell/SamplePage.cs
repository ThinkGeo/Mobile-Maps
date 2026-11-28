using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample;

/// <summary>
/// What every sample page shares: a full-bleed MapView that has a size before the
/// sample touches it, and a place to put the sample's own controls.
/// <para>
/// A MapView is configured once it first has a size. The wait for that is here, so a
/// sample only writes <see cref="BuildMap"/>.
/// </para>
/// <para>
/// A phone has no edge to spare for a side panel, so <see cref="AddToPanel{T}"/> puts
/// a sample's controls on a card over the bottom of the map.
/// </para>
/// </summary>
public abstract class SamplePage : ContentPage
{
    private readonly VerticalStackLayout _panelContent;
    private readonly ScrollView _panelScroller;
    private readonly Border _panel;
    private readonly Label _attribution;
    private readonly Border _backChip;

    /// <summary>True while the page is a phone turned on its side.</summary>
    private bool? _compact;

    protected SamplePage()
    {
        // A page insets its content inside the safe area by default on iOS, which is
        // right for text and wrong for a map: it leaves a band of page background
        // below the map. This asks for the inset to go; inside a NavigationPage the
        // ask does not reach the system, so FillSafeArea below is what removes it.
        Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific.Page.SetUseSafeArea(this, false);

        // The home indicator is asked to dim itself, since it lies across the map.
        // Inside a NavigationPage UIKit asks the navigation's own controller rather
        // than the page, so this takes effect only when a sample is shown outside the
        // navigation stack.
        Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific.Page.SetPrefersHomeIndicatorAutoHidden(this, true);

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

        KeepLayersToEdges(Map);

        // The map draws itself once it first has a size. Without this nothing is ever
        // asked to draw, and the page comes up empty with nothing in the log.
        Map.Behaviors.Add(new AutoLoadMapViewBehavior());

        // A map that fails to draw says nothing by default - the debug log starts at
        // LogLevel.None, so the only way an application hears about it is these two
        // events. A gallery is exactly where a silent blank page costs the most time,
        // so it listens and prints.
        Map.OverlayDrawFailed += (_, e) =>
            Console.WriteLine($"[Sample] {e.Overlay?.GetType().Name} failed to draw: {e.Error}");

        _panelContent = new VerticalStackLayout { Spacing = 5 };
        _panel = new Border
        {
            // Scrolled and capped: a sample whose controls are a list would otherwise
            // grow the card over the whole map. The cap is on the card and the
            // ScrollView is told to wrap its content (VerticalOptions.Start): left to
            // itself a ScrollView asks for all the height there is on Android.
            Content = _panelScroller = BuildPanelScroller(),
            MaximumHeightRequest = PanelMaxHeight,
            BackgroundColor = Color.FromRgba(255, 255, 255, 235),
            StrokeThickness = 0,
            Padding = new Thickness(10, 8),
            Margin = new Thickness(10, 0, 10, 0),
            VerticalOptions = LayoutOptions.End,
            IsVisible = false,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 12 },
        };

        _panelScroller.Content = _panelContent;

        // Credit for the data, over the map and never hidden behind anything. Every
        // basemap the gallery draws is built from OpenStreetMap, and both the ODbL the
        // data is licensed under and the tile servers' own terms ask for the credit to
        // be shown on the map. A sample drawing something else as well says so with
        // SetAttribution.
        _attribution = new Label
        {
            Text = DefaultAttribution,
            FontSize = 10,
            Padding = new Thickness(6, 2),
            HorizontalOptions = LayoutOptions.End,
            Margin = new Thickness(0, 0, 14, 4),
            TextColor = Color.FromArgb("#3C4043"),
            BackgroundColor = Color.FromRgba(255, 255, 255, 190),
        };

        // One group at the bottom so the credit always sits just over the card, and
        // sits on the same line when a sample has no card at all.
        var bottom = new VerticalStackLayout
        {
            VerticalOptions = LayoutOptions.End,
            Margin = new Thickness(0, 0, 0, PanelBottomMargin),
            Children = { _attribution, _panel },
        };

        // No navigation bar: a sample is a map, and the bar took a band off the top
        // of it for a title and a back arrow. A small back button with the title
        // stands in for it, over the map.
        NavigationPage.SetHasNavigationBar(this, false);
        _backChip = BuildBackChip(this);

        Content = new Grid { Children = { Map, bottom, _backChip } };

        // The strips the safe area holds back are given to the map.
        Loaded += (_, _) => FillSafeArea(Map);

        var built = false;
        Map.SizeChanged += (_, _) =>
        {
            if (built || Map.Width <= 0 || Map.Height <= 0) return;
            built = true;
            BuildMap(Map);
        };
    }

    /// <summary>
    /// The card's scroller.
    /// <para>
    /// A UIScrollView delays the touches it hands to its content by about a sixth of
    /// a second while it decides whether the finger meant to scroll. On a slider that
    /// delay is the difference between a control that answers the finger and one that
    /// looks broken, and the card here is usually shorter than its cap and has nothing
    /// to scroll at all. It is told not to delay and not to take a touch back once a
    /// control has it.
    /// </para>
    /// </summary>
    private static ScrollView BuildPanelScroller()
    {
        var scroller = new ScrollView { VerticalOptions = LayoutOptions.Start };

#if IOS || MACCATALYST
        scroller.HandlerChanged += (sender, _) =>
        {
            if (((ScrollView)sender).Handler?.PlatformView is UIKit.UIScrollView platform)
            {
                platform.DelaysContentTouches = false;
                platform.CanCancelContentTouches = false;
            }
        };
#endif

        return scroller;
    }

    /// <summary>
    /// The tallest a page can be and still count as a phone on its side. Phones in
    /// landscape are 320 to 440 high; the smallest tablet is over 700.
    /// </summary>
    private const double CompactHeight = 500;

    /// <summary>
    /// Lays the page out for its shape. A phone on its side has a few hundred points
    /// of height, and a card the width of the screen took a good part of them, so on
    /// its side the card keeps to the left and to less of the height, and the map
    /// shows beside and above it.
    /// </summary>
    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        if (width <= 0 || height <= 0)
            return;

        // Again whenever the page changes shape, because the strips move when the
        // phone turns - and after the layout pass, because while it is turning the
        // window still reports the insets of the orientation it is leaving.
        Dispatcher.Dispatch(() => FillSafeArea(Map));

        var compact = width > height && height < CompactHeight;
        _panel.MaximumHeightRequest = compact ? Math.Min(PanelMaxHeight, height * 0.45) : PanelMaxHeight;

        // A width, not a cap: held to the left, a card measured by its content comes
        // out as narrow as its narrowest row, and a strip of choices folds in half.
        _panel.WidthRequest = compact ? Math.Min(CompactPanelWidth, width - 20) : -1;
        if (compact == _compact)
            return;

        _compact = compact;
        _panel.HorizontalOptions = compact ? LayoutOptions.Start : LayoutOptions.Fill;
    }

    /// <summary>The card's cap: past it the controls scroll.</summary>
    private const double PanelMaxHeight = 260;

    /// <summary>How wide the card is on a phone on its side: room for a strip of three.</summary>
    private const double CompactPanelWidth = 440;

    /// <summary>
    /// The back button and title that stand in for the navigation bar, at the top
    /// left over the map.
    /// </summary>
    public static Border BuildBackChip(Page page)
    {
        var title = new Label
        {
            FontSize = 13,
            TextColor = PanelInk,
            VerticalOptions = LayoutOptions.Center,
            LineBreakMode = LineBreakMode.TailTruncation,
        };
        title.SetBinding(Label.TextProperty, new Binding(nameof(Page.Title), source: page));

        var chip = new Border
        {
            Content = new HorizontalStackLayout
            {
                Spacing = 6,
                Children =
                {
                    new Label
                    {
                        Text = "\u2039",
                        FontSize = 22,
                        TextColor = PanelInk,
                        VerticalOptions = LayoutOptions.Center,
                        Margin = new Thickness(0, -3, 0, 0),
                    },
                    title,
                },
            },
            BackgroundColor = Color.FromRgba(255, 255, 255, 235),
            StrokeThickness = 0,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 16 },
            Padding = new Thickness(10, 4, 14, 4),
            Margin = new Thickness(10, 10, 0, 0),
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
        };

        var tap = new TapGestureRecognizer();
        tap.Tapped += async (_, _) => await page.Navigation.PopAsync();
        chip.GestureRecognizers.Add(tap);
        return chip;
    }

    /// <summary>
    /// Lets the map's own layers reach the strips the safe area holds back.
    /// <para>
    /// A MAUI layout keeps its children out of the unsafe areas by default, and the
    /// map and every layer inside it is a layout. With the map hung past the safe area
    /// (see <see cref="FillSafeArea"/>) each of them would push its contents back in:
    /// on a phone on its side the map drew a band short of the edge by the notch.
    /// </para>
    /// </summary>
    public static void KeepLayersToEdges(MapView map)
    {
        static void Release(Element element)
        {
            if (element is Layout layout)
                layout.SafeAreaEdges = SafeAreaEdges.None;
            else if (element is ContentView view)
                view.SafeAreaEdges = SafeAreaEdges.None;
        }

        Release(map);
        map.DescendantAdded += (_, e) => Release(e.Element);
        foreach (var element in map.GetVisualTreeDescendants().OfType<Element>())
            Release(element);
    }

    /// <summary>
    /// Gives the map the strips the safe area holds back, by letting it hang that far
    /// past the edges of the page's content.
    /// <para>
    /// SetUseSafeArea(false) on the page does not do this for a page inside a
    /// NavigationPage: the navigation wraps the page in a controller of its own, and
    /// that controller holds the inset. What is left is a band of page background
    /// under the map.
    /// </para>
    /// <para>
    /// With a negative margin the map is arranged past the end of its container, iOS
    /// does not clip it, and the band is covered by map. The insets are read from the
    /// window because they differ by device and orientation: above the map for the
    /// status bar, below it for the home indicator, and down a side on displays that
    /// keep a column beside the app.
    /// </para>
    /// <para>
    /// Only the map is moved. The control card and the credit are its siblings in the
    /// page's Grid, so they stay inside the safe area.
    /// </para>
    /// </summary>
    public static void FillSafeArea(View map)
    {
#if IOS
        var insets = UIKit.UIApplication.SharedApplication.ConnectedScenes
            .OfType<UIKit.UIWindowScene>()
            .SelectMany(scene => scene.Windows)
            .FirstOrDefault()?.SafeAreaInsets ?? default;

        map.Margin = new Thickness(-insets.Left, -insets.Top, -insets.Right, -insets.Bottom);
#endif
    }

    /// <summary>
    /// How far the control card sits above the bottom of the page's content. Zero on
    /// iOS, where the content already ends one home indicator short of the screen.
    /// </summary>
    private const double PanelBottomMargin =
#if IOS
        0;
#else
        12;
#endif

    /// <summary>What every map here is ultimately drawn from.</summary>
    protected const string DefaultAttribution = "\u00a9 OpenStreetMap contributors";

    /// <summary>The map this page draws into.</summary>
    protected MapView Map { get; }

    /// <summary>
    /// Says who the data on screen belongs to. A sample that draws something beyond
    /// the OpenStreetMap-derived basemap - imagery, a government service - names it
    /// here, because the credit has to match what is actually being shown.
    /// </summary>
    protected void SetAttribution(string text)
        => _attribution.Text = string.IsNullOrWhiteSpace(text) ? DefaultAttribution : text;

    /// <summary>
    /// Adds a control to the card over the bottom of the map. The card appears with
    /// its first control, so a sample with none has a map and nothing else.
    /// </summary>
    protected T AddToPanel<T>(T view) where T : View
    {
        // A label with nothing to say takes no line: a result that is not there yet,
        // or a status that has been cleared, would otherwise hold a blank row open.
        if (view is Label label)
        {
            label.IsVisible = !string.IsNullOrEmpty(label.Text);
            label.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(Label.Text))
                    label.IsVisible = !string.IsNullOrEmpty(label.Text);
            };
        }

        _panelContent.Add(view);
        _panel.IsVisible = true;
        return view;
    }

    /// <summary>
    /// Opens the map on an extent with the whole of it on screen.
    /// <para>
    /// A phone is tall and most extents worth opening on are wide. Handed one that
    /// does not match the view's shape, the map fits it to the view's height and crops
    /// the sides - which is how a map of the lower forty-eight loses both coasts. This
    /// widens the extent to the view instead, and can only do it once the view has a
    /// size, hence the one-shot handler rather than a constant.
    /// </para>
    /// </summary>
    protected static void OpenOn(MapView map, RectangleShape extent)
    {
        map.CurrentExtent = extent;

        void Fit()
        {
            map.CurrentExtent = MapUtil.GetDrawingExtent(extent, map.Width, map.Height);
            _ = map.RefreshAsync();
        }

        // BuildMap is itself called from a SizeChanged, so the view usually has its
        // size already and no further event is owed.
        if (map.Width > 0 && map.Height > 0)
        {
            Fit();
            return;
        }

        void FitWhenSized(object sender, EventArgs e)
        {
            if (map.Width <= 0 || map.Height <= 0) return;

            map.SizeChanged -= FitWhenSized;
            Fit();
        }

        map.SizeChanged += FitWhenSized;
    }

    /// <summary>
    /// The ink the panel's own text is written in.
    /// <para>
    /// The card the panel sits on is the same near white whatever the system theme is,
    /// so a label left to MAUI's default colour is white on white in a dark theme - and
    /// worse, a label built while the theme was dark keeps that resolved colour after
    /// the theme is changed back, so the text stays invisible until the sample is
    /// reopened. Every label on the card names its colour for that reason.
    /// </para>
    /// </summary>
    protected static readonly Color PanelInk = Color.FromArgb("#1F2328");

    /// <summary>
    /// A button for the panel, in the panel's own colours. Left to the platform, a
    /// button in a dark theme is drawn dark with light text, which on the near-white
    /// card reads as a black block on Android; for the same reason as
    /// <see cref="PanelInk"/>, the colours are named here. Named colours also stop the
    /// platform greying a disabled button, so it is dimmed instead.
    /// </summary>
    protected static Button PanelButton(string text, double fontSize, Thickness padding)
    {
        var button = new Button
        {
            Text = text,
            FontSize = fontSize,
            Padding = padding,
            TextColor = Colors.White,
            BackgroundColor = Color.FromArgb("#2D7FF9"),
            CornerRadius = 8,
        };
        button.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(Button.IsEnabled))
                button.Opacity = button.IsEnabled ? 1 : 0.5;
        };
        return button;
    }

    /// <summary>A one-line status label on the panel, for what a sample wants to say.</summary>
    protected Label AddStatusLabel()
        => AddToPanel(new Label { FontSize = 12, TextColor = Colors.DimGray, LineBreakMode = LineBreakMode.TailTruncation });

    /// <summary>
    /// One slider in a row: what it is called, the range it moves over, where it
    /// starts and what to do when it moves. <paramref name="Caption"/> turns the
    /// current value into the line above the slider, so the number is read off the
    /// control that sets it rather than from a separate readout.
    /// </summary>
    protected sealed record SliderSpec(
        string Name, double Min, double Max, double Value, Action<double> Changed, Func<double, string> Caption = null);

    /// <summary>
    /// Named sliders side by side on one line, each as wide as its share of the panel.
    /// The callback runs on every move rather than on release, so the map follows the
    /// slider.
    /// </summary>
    protected Grid AddSliderRow(params SliderSpec[] sliders)
    {
        var row = new Grid { ColumnSpacing = 14 };

        for (var column = 0; column < sliders.Length; column++)
        {
            var spec = sliders[column];
            row.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));

            var caption = new Label
            {
                Text = spec.Caption?.Invoke(spec.Value) ?? spec.Name,
                FontSize = 11,
                TextColor = Colors.Gray,
                LineBreakMode = LineBreakMode.TailTruncation,
            };

            var slider = new Slider { Minimum = spec.Min, Maximum = spec.Max, Value = spec.Value };
            slider.ValueChanged += (_, e) =>
            {
                if (spec.Caption != null) caption.Text = spec.Caption(e.NewValue);
                spec.Changed(e.NewValue);
            };

            row.Add(new VerticalStackLayout { Children = { caption, slider } }, column);
        }

        return AddToPanel(row);
    }

    /// <summary>
    /// Named choices on one line.
    /// <para>
    /// Three or fewer and they are all on screen at once. More, and the line shows
    /// three at a time - the choice in the middle with the one before and the one
    /// after it either side - and tapping a side brings it to the middle, the line
    /// sliding as it goes. Either way the choice is one tap and nothing covers the
    /// map while it is made.
    /// </para>
    /// </summary>
    protected ChoiceStrip AddChoiceStrip(IReadOnlyList<string> choices, int selectedIndex, Action<int> chosen)
        => AddToPanel(new ChoiceStrip(choices, selectedIndex, chosen));

    /// <summary>
    /// The line of choices itself. There is no such control in MAUI, so it is a Grid
    /// of tappable cells.
    /// <para>
    /// A Picker would cost two taps and spend the first of them covering the map with
    /// a list, and on these samples the map is what is being chosen between.
    /// </para>
    /// <para>
    /// Labels wrap to a second line rather than being cut off: a cell is a third of a
    /// phone's width, which is about seventeen characters.
    /// </para>
    /// </summary>
    protected sealed class ChoiceStrip : Grid
    {
        private static readonly Color SelectedFill = Color.FromArgb("#2D7FF9");
        private static readonly Color SelectedText = Colors.White;

        // A faint fill rather than none, for two reasons: a cell with nothing drawn
        // in it does not read as something to press, and on iOS a fully transparent
        // background is the kind of thing that stops hit-testing.
        private static readonly Color RestingFill = Color.FromArgb("#12000000");
        private static readonly Color RestingText = Color.FromArgb("#5A6672");

        private readonly IReadOnlyList<string> _choices;
        private readonly Action<int> _chosen;
        private readonly Border[] _cells;
        private readonly Label[] _labels;

        /// <summary>True when there are more choices than cells, so the line scrolls.</summary>
        private readonly bool _scrolls;

        private int _index;

        public ChoiceStrip(IReadOnlyList<string> choices, int selectedIndex, Action<int> chosen)
        {
            _choices = choices;
            _chosen = chosen;
            _index = selectedIndex;
            _scrolls = choices.Count > 3;

            var cells = _scrolls ? 3 : choices.Count;
            _cells = new Border[cells];
            _labels = new Label[cells];
            ColumnSpacing = 6;

            for (var cell = 0; cell < cells; cell++)
            {
                ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));

                var label = new Label
                {
                    FontSize = 12,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                    LineBreakMode = LineBreakMode.WordWrap,
                    MaxLines = 2,
                };

                var pill = new Border
                {
                    Content = label,
                    StrokeThickness = 0,
                    Padding = new Thickness(4, 5),
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 8 },
                };

                var tapped = cell;
                var tap = new TapGestureRecognizer();
                tap.Tapped += (_, _) => Choose(tapped);
                pill.GestureRecognizers.Add(tap);

                _labels[cell] = label;
                _cells[cell] = pill;
                Children.Add(pill);
                SetColumn((BindableObject)pill, cell);
            }

            Show();
        }

        private void Choose(int cell)
        {
            if (_scrolls)
            {
                // -1 for the cell on the left, +1 for the one on the right; the middle
                // cell is already where it is going.
                var step = cell - 1;
                if (step == 0) return;

                _index = ((_index + step) % _choices.Count + _choices.Count) % _choices.Count;
                Show();
                Slide(step);
            }
            else
            {
                if (cell == _index) return;
                _index = cell;
                Show();
            }

            _chosen(_index);
        }

        /// <summary>
        /// The cells are relabelled first, which puts the tapped choice in the middle
        /// straight away; the line is then placed back where that choice was standing
        /// and animated to zero, so what the eye follows is the choice travelling to
        /// the middle rather than three labels changing at once.
        /// </summary>
        private void Slide(int step)
        {
            var cellWidth = Width / 3d;
            if (cellWidth <= 0) return;

            TranslationX = step * cellWidth;
            _ = this.TranslateToAsync(0, 0, 160, Easing.CubicOut);
        }

        private void Show()
        {
            for (var cell = 0; cell < _cells.Length; cell++)
            {
                var choice = _scrolls
                    ? ((_index + cell - 1) % _choices.Count + _choices.Count) % _choices.Count
                    : cell;
                var selected = _scrolls ? cell == 1 : cell == _index;

                _labels[cell].Text = _choices[choice];
                _labels[cell].TextColor = selected ? SelectedText : RestingText;
                _labels[cell].FontAttributes = selected ? FontAttributes.Bold : FontAttributes.None;
                _cells[cell].BackgroundColor = selected ? SelectedFill : RestingFill;
            }
        }
    }

    /// <summary>
    /// A sample's map holds GPU textures and a tile cache, which are native memory the
    /// garbage collector does not measure, so leaving the page does not free them
    /// promptly. The page releases its own map on the way out instead.
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

    /// <summary>Builds the sample, once the map has a real size.</summary>
    protected abstract void BuildMap(MapView map);
}
