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

    private static object BuildGroupHeader() => new Label
    {
        FontAttributes = FontAttributes.Bold,
        FontSize = 13,
        Padding = new Thickness(12, 10, 12, 4),
        TextColor = Colors.DimGray,
    }.Bind(Label.TextProperty, nameof(SampleGroup.Title));

    private static object BuildRow()
    {
        var title = new Label { FontSize = 16 }.Bind(Label.TextProperty, nameof(SampleEntry.Title));
        var summary = new Label { FontSize = 12, TextColor = Colors.Gray, MaxLines = 2, LineBreakMode = LineBreakMode.TailTruncation }
            .Bind(Label.TextProperty, nameof(SampleEntry.Summary));

        return new VerticalStackLayout
        {
            Padding = new Thickness(14, 8),
            Spacing = 2,
            Children = { title, summary },
        };
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
