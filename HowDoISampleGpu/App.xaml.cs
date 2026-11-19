namespace HowDoISampleGpu;

public partial class App
{
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
