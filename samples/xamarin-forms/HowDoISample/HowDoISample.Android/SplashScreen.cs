using System.Threading.Tasks;
using Android.App;
using Android.Content;
using AndroidX.AppCompat.App;

namespace HowDoISample.Droid
{
    [Activity(Theme = "@style/MyTheme.Splash", MainLauncher = true, NoHistory = true)]
    public class SplashScreen : AppCompatActivity
    {
        protected override void OnResume()
        {
            base.OnResume();
            var startupWork = new Task(() => { Startup(); });
            startupWork.Start();
        }

        public override void OnBackPressed()
        {
        }

        private async void Startup()
        {
            await Task.Delay(500);
            StartActivity(new Intent(Application.Context, typeof(MainActivity)));
        }
    }
}