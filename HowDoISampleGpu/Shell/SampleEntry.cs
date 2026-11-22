namespace HowDoISampleGpu;

/// <summary>What a sample draws with. A property of a row, never a group of its
/// own - nobody browses for "the GPU samples", they browse for tilt or terrain
/// or labels.</summary>
public enum SampleDraws
{
    /// <summary>A GPU basemap, drawn from a style document.</summary>
    Gpu,

    /// <summary>The classic overlay pipeline only - no GPU basemap. A few samples
    /// are about that pipeline itself: reprojecting a raster as it draws, say.</summary>
    Classic,

    /// <summary>A GPU basemap with classic overlays above it.</summary>
    GpuAndOverlays,

    /// <summary>Two maps side by side, for a comparison.</summary>
    SideBySide,

    /// <summary>A page about the gallery rather than a sample of the SDK.</summary>
    Page,
}

/// <summary>
/// One row of the catalog. The menu, the search index and the home page are all
/// projections of this, so a sample is described once.
/// </summary>
public sealed class SampleEntry
{
    public SampleEntry(
        string group,
        string title,
        string summary,
        string api,
        SampleDraws draws,
        Func<Page> create,
        string aliases = null)
    {
        Group = group;
        Title = title;
        Summary = summary;
        Api = api;
        Draws = draws;
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

    /// <summary>What the sample draws with.</summary>
    public SampleDraws Draws { get; }

    /// <summary>Builds the page. A factory rather than an instance: a sample holds
    /// a GL surface and tile sources, so it is built when opened and torn down when
    /// left.</summary>
    public Func<Page> Create { get; }

    /// <summary>Extra words the search should match - old names, API spellings,
    /// the question a customer actually types.</summary>
    public string Aliases { get; }

    /// <summary>
    /// Builds the page and gives it this row's name, so the bar over a sample says
    /// what is open rather than standing empty with a back arrow in it. A page that
    /// titles itself - the few that are not <c>SamplePage</c> - keeps its own.
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
