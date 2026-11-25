namespace HowDoISample;

/// <summary>
/// One row of the catalog. The menu and the search index are both projections of
/// this, so a sample is described once.
/// </summary>
public sealed class SampleEntry
{
    public SampleEntry(
        string group,
        string title,
        string summary,
        string api,
        Func<Page> create,
        string aliases = null)
    {
        Group = group;
        Title = title;
        Summary = summary;
        Api = api;
        Create = create;
        Aliases = aliases ?? string.Empty;
    }

    /// <summary>The menu group this row sits in.</summary>
    public string Group { get; }

    /// <summary>The row's name in the menu.</summary>
    public string Title { get; }

    /// <summary>One sentence on what the sample does.</summary>
    public string Summary { get; }

    /// <summary>The API to search the documentation for.</summary>
    public string Api { get; }

    /// <summary>Builds the page. A factory rather than an instance: a sample holds
    /// a GL surface and tile sources, so it is built when opened and torn down when
    /// left.</summary>
    public Func<Page> Create { get; }

    /// <summary>Extra words the search should match - other names for the same
    /// thing, API spellings.</summary>
    public string Aliases { get; }

    /// <summary>
    /// Builds the page and gives it this row's name, so the navigation bar over a
    /// sample says what is open. A page that titles itself keeps its own.
    /// </summary>
    public Page CreateTitledPage()
    {
        var page = Create();
        if (string.IsNullOrEmpty(page.Title))
        {
            page.Title = Title;
        }

        return page;
    }

    /// <summary>True when the row matches the search text.</summary>
    public bool Matches(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return true;
        var needle = text.Trim();
        return Title.Contains(needle, StringComparison.OrdinalIgnoreCase)
            || Summary.Contains(needle, StringComparison.OrdinalIgnoreCase)
            || Api.Contains(needle, StringComparison.OrdinalIgnoreCase)
            || Group.Contains(needle, StringComparison.OrdinalIgnoreCase)
            || Aliases.Contains(needle, StringComparison.OrdinalIgnoreCase);
    }
}
