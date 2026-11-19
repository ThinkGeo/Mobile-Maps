using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu;

public partial class App
{
    /// <summary>
    /// Gives back what a finished sample was holding. A MapView keeps GL textures and
    /// a tile cache, which are native memory the collector does not account for, so
    /// an unreferenced map is not a freed one - it has to be told.
    /// <para>
    /// Disposing is all of it. This used to walk the whole visual tree disconnecting
    /// every handler, because without that a popped page measured as still alive -
    /// but that was the SDK not releasing its own native surface, fixed now, and an
    /// application should not have to know to do it. What is left here is what any
    /// application would write.
    /// </para>
    /// </summary>
    private static void ReleaseMaps(Page page)
    {
        if (page == null) return;

        var maps = page.GetVisualTreeDescendants().OfType<MapView>().ToList();

        foreach (var map in maps)
        {
            map.Basemap?.Dispose();
            map.Basemap = null;
            map.Dispose();
        }

        (page as IDisposable)?.Dispose();
    }


    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState activationState)
    {
        // Before any page: a sample's constructor may open a shapefile, and the
        // samples are written in plain file paths like the desktop gallery's.
        //
        // Through Task.Run, not awaited inline. CreateWindow runs on the UI thread,
        // whose synchronization context posts every continuation back to that same
        // thread - so blocking on the copy there deadlocks it, and the app comes up
        // with a window and no content, no exception and nothing in the log.
        var copied = Task.Run(SampleData.EnsureCopiedAsync).GetAwaiter().GetResult();
        if (copied > 0)
        {
            Console.WriteLine($"[HowDoI] copied {copied} sample data files into the app's data directory");
        }

        var navigation = new NavigationPage(new MenuPage());

        // Released here rather than in a page's own override: a sample is only
        // sometimes a SamplePage - several build their own ContentPage - and the
        // ones that did not inherit it were the ones holding the most. What is
        // popped is what is finished with, whatever class it happens to be.
        navigation.Popped += (_, e) => ReleaseMaps(e.Page);

        // Opening one sample straight away, for a run that is driven rather than
        // tapped: `simctl launch` passes SIMCTL_CHILD_HOWDOI_SAMPLE through as this
        // variable, and `devicectl` passes DEVICECTL_CHILD_HOWDOI_SAMPLE. It is how
        // every sample gets screenshotted without a hand on the screen; without it
        // the gallery opens on its menu as a person would find it.

        var wanted = Environment.GetEnvironmentVariable("HOWDOI_SAMPLE");
        if (!string.IsNullOrWhiteSpace(wanted))
        {
            var entry = SampleCatalog.All.FirstOrDefault(
                e => string.Equals(e.Title, wanted, StringComparison.OrdinalIgnoreCase));

            if (entry == null)
            {
                Console.WriteLine($"[HowDoI] no sample titled '{wanted}'; the catalog has: "
                    + string.Join(", ", SampleCatalog.All.Select(e => e.Title)));
            }
            else
            {
                Console.WriteLine($"[HowDoI] opening '{entry.Title}'");
                navigation.PushAsync(entry.Create(), animated: false);

            }
        }

        return new Window(navigation);
    }
}
