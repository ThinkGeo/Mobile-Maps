namespace HowDoISample;

public partial class AppShell
{
    public static readonly BindableProperty CurrentPageTitleProperty =
        BindableProperty.Create(nameof(CurrentPageTitle), typeof(string), typeof(AppShell), string.Empty);

    public string CurrentPageTitle
    {
        get => (string)GetValue(CurrentPageTitleProperty);
        set => SetValue(CurrentPageTitleProperty, value);
    }

    public AppShell()
    {
        InitializeComponent();
    }

    public async Task NavigateFromMenu(SampleInfo sampleInfo)
    {
        // Set the title and description
        if (Current is AppShell shell)
        {
            shell.CurrentPageTitle = sampleInfo.Title;
        }

        // Add the route of your page (e.g., "YourPageRoute") to navigate to that page
        await Current.GoToAsync($"{sampleInfo.Id}");
        var currentPage = Current.CurrentPage;

        if (currentPage is SamplePage samplePage)
        {
            samplePage.Description = sampleInfo.Description;
            samplePage.Title = sampleInfo.Title;
        }

        FlyoutIsPresented = false;
    }
}