using Android.App;
using Android.Content.PM;
using Android.OS;

namespace HowDoISample;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop,
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode
        | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    /// <summary>
    /// Carries the name of a sample to open from the launch intent into the
    /// environment variable the shared App reads: `am start --es HOWDOI_SAMPLE` is
    /// Android's counterpart of an environment variable passed at launch on iOS.
    /// </summary>
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
