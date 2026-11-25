using ThinkGeo.UI.Maui;

namespace HowDoISample;

public partial class App
{
    /// <summary>
    /// Gives back what a finished sample was holding. A MapView keeps GL textures and
    /// a tile cache, which are native memory the collector does not account for, so
    /// an unreferenced map is not a freed one - it has to be disposed.
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
        // samples read their data by plain file path.
        //
        // Through Task.Run, not awaited inline. CreateWindow runs on the UI thread,
        // whose synchronization context posts every continuation back to that same
        // thread, so blocking on the copy there would deadlock it.
        var copied = Task.Run(SampleData.EnsureCopiedAsync).GetAwaiter().GetResult();
        if (copied > 0)
        {
            Console.WriteLine($"[HowDoI] copied {copied} sample data files into the app's data directory");
        }

        var navigation = new NavigationPage(new MenuPage());

        // Released here as well as by SamplePage: not every sample is a SamplePage,
        // and what is popped is finished with whatever class it is.
        navigation.Popped += (_, e) => ReleaseMaps(e.Page);

        // Opens one sample straight away, for a run that is driven rather than
        // tapped: `simctl launch` passes SIMCTL_CHILD_HOWDOI_SAMPLE through as this
        // variable, and `devicectl` passes DEVICECTL_CHILD_HOWDOI_SAMPLE. Without it
        // the gallery opens on its menu.
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
                navigation.PushAsync(entry.CreateTitledPage(), animated: false);
            }
        }

        return new Window(navigation);
    }
}
