namespace HowDoISampleGpu;

/// <summary>
/// The gallery's menu: every catalog row, grouped, with a search box over it.
/// The desktop gallery prints the same list down its left edge; a phone has no
/// left edge to spare, so it is a page the map returns to.
/// </summary>
public class MenuPage : ContentPage
{
    private readonly CollectionView _list;
    private readonly SearchBar _search;

    public MenuPage()
    {
        Title = "ThinkGeo GPU";

        _search = new SearchBar { Placeholder = "Search samples, APIs, tasks" };
        _search.TextChanged += (_, _) => Rebuild();

        _list = new CollectionView
        {
            IsGrouped = true,
            SelectionMode = SelectionMode.Single,
            ItemTemplate = new DataTemplate(BuildRow),
            GroupHeaderTemplate = new DataTemplate(BuildGroupHeader),
        };
        _list.SelectionChanged += OnSelectionChanged;

        Content = new Grid
        {
            RowDefinitions = { new RowDefinition(GridLength.Auto), new RowDefinition(GridLength.Star) },
            Children = { _search, _list },
        };
        Grid.SetRow(_list, 1);

        Rebuild();
    }

    private void Rebuild()
    {
        var text = _search.Text;
        var groups = SampleCatalog.Search(text)
            .GroupBy(entry => entry.Group)
            .Select(group => new SampleGroup(group.Key, group.ToList()))
            .ToList();
        _list.ItemsSource = groups;
    }

    /// <summary>
    /// A band, not a caption. The heading used to be grey 13pt text with the same
    /// left edge and the same background as the rows under it, so a list of seventy
    /// samples read as one undifferentiated column - nothing said where one group
    /// ended and the next began.
    /// </summary>
    /// <summary>
    /// A colour for each appearance. Hard-coding the light one and leaving the text
    /// to its default painted white titles onto white rows in dark mode - the list
    /// came up blank, with only the separators showing. Every colour below is stated
    /// for both, so a row always contrasts with whatever it is sitting on.
    /// </summary>
    private static void SetThemeColor(BindableObject target, BindableProperty property, string light, string dark)
        => target.SetAppThemeColor(property, Color.FromArgb(light), Color.FromArgb(dark));

    private static object BuildGroupHeader()
    {
        var title = new Label
        {
            FontAttributes = FontAttributes.Bold,
            FontSize = 17,
            VerticalOptions = LayoutOptions.Center,
        }.Bind(Label.TextProperty, nameof(SampleGroup.Title));
        SetThemeColor(title, Label.TextColorProperty, "#000000", "#FFFFFF");

        var count = new Label
        {
            FontSize = 13,
            VerticalOptions = LayoutOptions.Center,
        }.Bind(Label.TextProperty, nameof(SampleGroup.Label));
        SetThemeColor(count, Label.TextColorProperty, "#8A8A8E", "#98989D");

        var header = new Grid
        {
            ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) },
            Padding = new Thickness(16, 14, 16, 10),
            Children = { title, count },
        };
        SetThemeColor(header, VisualElement.BackgroundColorProperty, "#EFEFF4", "#2C2C2E");

        Grid.SetColumn(count, 1);
        return header;
    }

    /// <summary>
    /// The name, and nothing else. Every row used to carry two lines of summary under
    /// its title, which made each one three times as tall, pushed the list to several
    /// screens of scrolling, and buried the grouping - and a sentence that long in a
    /// list is not read anyway. The summary belongs on the sample, not in the index.
    /// </summary>
    private static object BuildRow()
    {
        var title = new Label
        {
            FontSize = 16,
            VerticalOptions = LayoutOptions.Center,
            LineBreakMode = LineBreakMode.TailTruncation,
        }.Bind(Label.TextProperty, nameof(SampleEntry.Title));
        SetThemeColor(title, Label.TextColorProperty, "#000000", "#FFFFFF");

        var rule = new BoxView { HeightRequest = 1, Margin = new Thickness(24, 0, 0, 0) };
        SetThemeColor(rule, BoxView.ColorProperty, "#E0E0E5", "#38383A");

        var row = new VerticalStackLayout
        {
            Children =
            {
                new ContentView { Padding = new Thickness(24, 12, 16, 12), Content = title },
                rule,
            },
        };
        SetThemeColor(row, VisualElement.BackgroundColorProperty, "#FFFFFF", "#1C1C1E");

        return row;
    }

    private async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not SampleEntry entry) return;
        _list.SelectedItem = null;

        // A sample holds a GL surface and tile sources, so it is built when opened
        // and torn down when left - never cached.
        await Navigation.PushAsync(entry.Create());
    }
}

/// <summary>A one-line binding helper, so the row templates read as markup would.</summary>
internal static class BindExtensions
{
    public static T Bind<T>(this T element, BindableProperty property, string path) where T : BindableObject
    {
        element.SetBinding(property, path);
        return element;
    }
}
