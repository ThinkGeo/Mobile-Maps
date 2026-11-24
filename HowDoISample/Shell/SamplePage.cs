using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample;

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
    private readonly ScrollView _panelScroller;
    private readonly Border _panel;
    private readonly Label _attribution;

    protected SamplePage()
    {
        // A page insets its content inside the safe area by default on iOS, which is
        // right for text and wrong for a map: it leaves a band of page background
        // below the map on every sample - white, on a gallery whose maps are mostly
        // dark. This asks for the inset to go; FillSafeArea below is what actually
        // removes it, because this ask does not reach the system.
        Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific.Page.SetUseSafeArea(this, false);

        // And the home indicator is asked to dim itself, since it is now a bar lying
        // across a map rather than across a strip of page background. Dimming after a
        // few idle seconds is all iOS allows; the bar cannot be removed.
        //
        // UIKit asks whichever controller is in charge of the screen, and inside a
        // NavigationPage that is the navigation's own wrapper, not the page - measured
        // on an iPad: PageViewController answered true to this, ParentingViewController
        // above it answered false, and false is what the system acted on. So the ask is
        // correct and currently has no effect here; it starts working the day a sample
        // is shown outside the navigation stack.
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

        // The map draws itself once it first has a size. Without this nothing is ever
        // asked to draw, and the page comes up empty with nothing in the log.
        Map.Behaviors.Add(new AutoLoadMapViewBehavior());

        // A map that fails to draw says nothing by default - the debug log starts at
        // LogLevel.None, so the only way an application hears about it is these two
        // events. A gallery is exactly where a silent blank page costs the most time,
        // so it listens and prints.
        Map.OverlayDrawFailed += (_, e) =>
            Console.WriteLine($"[Sample] {e.Overlay?.GetType().Name} failed to draw: {e.Error}");

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
            Content = _panelScroller = BuildPanelScroller(),
            MaximumHeightRequest = 260,
            BackgroundColor = Color.FromRgba(255, 255, 255, 235),
            StrokeThickness = 0,
            Padding = new Thickness(12, 10),
            // Clear of the home indicator, now that the page no longer insets for it.
            Margin = new Thickness(10, 0, 10, 0),
            VerticalOptions = LayoutOptions.End,
            IsVisible = false,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 12 },
        };

        _panelScroller.Content = _panelContent;

        // Credit for the data, over the map and never hidden behind anything. Every
        // basemap this gallery draws is built from OpenStreetMap - ThinkGeo's own
        // streets, OpenFreeMap's, the imagery's road network - and both the ODbL the
        // data is licensed under and the tile servers' own terms ask for the credit to
        // be shown on the map rather than filed on a page about the app. A sample
        // drawing something else as well says so with SetAttribution.
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

        Content = new Grid { Children = { Map, bottom } };

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
    /// Gives the map the strips the safe area holds back, by letting it hang that far
    /// past the sides and the bottom of the page's content.
    /// <para>
    /// SetUseSafeArea(false) on the page is the documented way to ask for this and it
    /// does nothing here - measured on an iPhone, with the page reporting 758 points
    /// of height and its map arranged into 724, the missing 34 being exactly the
    /// window's bottom inset. A page inside a NavigationPage is wrapped in a
    /// controller of the navigation's own, and that is the controller holding the
    /// inset; the same wrapper is why the page cannot dim the home indicator either.
    /// What it leaves behind is a band of page background under every map - white,
    /// 34 points tall, the thing that kept being reported as a bar across the bottom.
    /// </para>
    /// <para>
    /// A negative margin is a blunt instrument and it is the one that fits: the map
    /// is arranged past the end of its container, iOS does not clip it, and the band
    /// is covered by the map that should have been there. The insets are read from
    /// the window rather than written down, because there is no one set of them: 34
    /// points below on a phone and 21 on an iPad, nothing at the sides in portrait,
    /// and 84 down the right of an iPhone Duo's cover display, which is where this
    /// stopped being about the home indicator. That display keeps a column beside the
    /// app for the clock and the back affordance; a map that respects it ends in a
    /// white gutter down the right of a black globe. Settings does the same thing and
    /// gets away with it by letting its own background run under the column, which is
    /// exactly what the map is being told to do here.
    /// </para>
    /// <para>
    /// Only the map is moved. The control card and the credit are its siblings in the
    /// page's Grid, so they stay inside the inset content and clear of the column -
    /// which is the half of this that has to keep working.
    /// </para>
    /// <para>
    /// Not the top: every sample either has a navigation bar over it or, in Buildings
    /// in 3D, has deliberately given that space to its own controls.
    /// </para>
    /// </summary>
    public static void FillSafeArea(View map)
    {
#if IOS
        var insets = UIKit.UIApplication.SharedApplication.ConnectedScenes
            .OfType<UIKit.UIWindowScene>()
            .SelectMany(scene => scene.Windows)
            .FirstOrDefault()?.SafeAreaInsets ?? default;

        map.Margin = new Thickness(-insets.Left, 0, -insets.Right, -insets.Bottom);
#endif
    }

    /// <summary>
    /// How far the control card sits above the bottom of the page's content.
    /// <para>
    /// Zero on iOS, because the content already ends one home indicator short of the
    /// screen - the card then rests exactly on that line, which is where a sheet sits
    /// on this platform. It was 34, which added a second indicator's worth of nothing
    /// under the card: measured, 68 points of gap where 34 was intended.
    /// </para>
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

        // BuildMap is itself called from a SizeChanged, so by the time this runs the
        // view usually has its size already and no further event is owed - a handler
        // on its own would wait for a layout pass that may never come, and did come
        // on iOS only because MAUI happens to lay the page out more than once.
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
    /// <para>
    /// A full-width slider with its caption on a line of its own costs two lines each,
    /// and a card four lines tall over a phone screen is mostly card. Two of these fit
    /// on one line and still read.
    /// </para>
    /// <para>
    /// The callback runs on every move rather than on release. The desktop gallery
    /// redraws while the mouse is down, and a slider that does nothing until it is let
    /// go reads as a slider that does nothing - which is exactly what it looked like.
    /// </para>
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
    /// Named choices on one line, the mobile shape of the desktop gallery's radio
    /// button column.
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
    /// What it replaces is a Picker, which costs two taps and spends the first of
    /// them covering the map with a list - and on these samples the map is the thing
    /// the list is asking you to choose between.
    /// </para>
    /// <para>
    /// Labels wrap to a second line rather than being cut off. A cell is a third of
    /// a phone's width, which is about seventeen characters, and a name that does not
    /// fit is better read over two lines than ending in an ellipsis - though the
    /// names themselves were shortened first, because two lines of card is still card.
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
                    Padding = new Thickness(4, 6),
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

        /// <summary>The choice the line is on.</summary>
        public int SelectedIndex => _index;

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
            _ = this.TranslateTo(0, 0, 160, Easing.CubicOut);
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
