namespace HowDoISampleGpu;

public partial class App
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState activationState)
        => new Window(new AppShell());
}
