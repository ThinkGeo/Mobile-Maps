using Android.App;
using Android.Content.PM;
using Android.OS;

namespace HowDoISampleGpu;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop,
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode
        | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    /// <summary>
    /// Carries the driven-run sample name from the launch intent into the environment
    /// the shared App reads.
    /// </summary>
    /// <remarks>
    /// iOS has `simctl launch`, which sets a child process's environment directly, so
    /// App just reads HOWDOI_SAMPLE. Android has no such thing: `am start --es` passes
    /// an intent extra instead. Putting it into the environment here is what lets the
    /// same one line in App drive both platforms, and so the same sweep script
    /// screenshot every sample on either.
    /// </remarks>
    protected override void OnCreate(Bundle savedInstanceState)
    {
        var wanted = Intent?.GetStringExtra("HOWDOI_SAMPLE");
        if (!string.IsNullOrWhiteSpace(wanted))
        {
            System.Environment.SetEnvironmentVariable("HOWDOI_SAMPLE", wanted);
        }

        base.OnCreate(savedInstanceState);
    }
}
