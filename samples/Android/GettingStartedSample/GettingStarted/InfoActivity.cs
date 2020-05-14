using Android.App;
using Android.OS;
using Android.Webkit;

namespace GettingStartedSample
{
    /// <summary>
    /// This class represents the info window after clicking on the info button.
    /// </summary>
    [Activity(Label = "Help")]
    public class InfoActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.WebView);

            WebView webView = FindViewById<WebView>(Resource.Id.localWebView);
            webView.LoadUrl("http://thinkgeo.com/map-suite-developer-gis/android-edition/");
        }
    }
}