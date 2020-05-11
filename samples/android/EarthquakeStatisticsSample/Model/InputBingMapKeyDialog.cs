using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Java.Net;
using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using ThinkGeo.Core;
using ThinkGeo.UI.Android;

namespace MapSuiteEarthquakeStatistics
{
    public class InputBingMapKeyDialog : AlertDialog
    {
        private Context context;
        private View inputBingMapsKeyView;
        private Button bingMapsKeyOkButton;
        private ISharedPreferences preferences;
        private SelectBaseMapTypeDialog selectBaseMapTypeDialog;

        public InputBingMapKeyDialog(Context context, SelectBaseMapTypeDialog selectBaseMapTypeDialog, ISharedPreferences preferences)
            : base(context)
        {
            inputBingMapsKeyView = View.Inflate(context, Resource.Layout.BingMapsKeyLayout, null);
            this.SetView(inputBingMapsKeyView);

            this.context = context;
            this.preferences = preferences;
            this.selectBaseMapTypeDialog = selectBaseMapTypeDialog;

            EditText bingMapsKeyEditText = inputBingMapsKeyView.FindViewById<EditText>(Resource.Id.BingMapsKeyEditText);
            Button bingMapsKeyCancelButton = inputBingMapsKeyView.FindViewById<Button>(Resource.Id.CancelButton);
            bingMapsKeyOkButton = inputBingMapsKeyView.FindViewById<Button>(Resource.Id.OkButton);

            bingMapsKeyOkButton.Click += BingMapsKeyOkButton_Click;
            bingMapsKeyCancelButton.Click += BingMapsKeyCancelButton_Click;
        }

        private void BingMapsKeyOkButton_Click(object sender, EventArgs e)
        {
            EditText bingMapsKeyEditText = inputBingMapsKeyView.FindViewById<EditText>(Resource.Id.BingMapsKeyEditText);
            string inputKey = bingMapsKeyEditText.Text;

            bingMapsKeyEditText.Enabled = false;
            bingMapsKeyOkButton.Enabled = false;

            Task.Factory.StartNew(() =>
            {
                bool isValid = Validate(inputKey, BingMapsMapType.Aerial);
                Global.MapView.Post(() =>
                {
                    if (isValid)
                    {
                        Cancel();

                        ISharedPreferencesEditor editor = preferences.Edit();
                        editor.PutString(Global.PREFS_BINGMAPKEY, bingMapsKeyEditText.Text);
                        editor.Commit();

                        ((BingMapsOverlay)Global.MapView.Overlays[Global.BingMapsAerialOverlayKey]).ApplicationId = bingMapsKeyEditText.Text;
                        ((BingMapsOverlay)Global.MapView.Overlays[Global.BingMapsRoadOverlayKey]).ApplicationId = bingMapsKeyEditText.Text;

                        Global.MapView.Overlays[Global.ThinkGeoCloudMapsOverlayKey].IsVisible = false;
                        Global.MapView.Overlays[Global.OpenStreetMapOverlayKey].IsVisible = false;
                        Global.MapView.Overlays[Global.BingMapsAerialOverlayKey].IsVisible = Global.BaseMapType == BaseMapType.BingMapsAerial;
                        Global.MapView.Overlays[Global.BingMapsRoadOverlayKey].IsVisible = Global.BaseMapType == BaseMapType.BingMapsRoad;

                        Global.MapView.Refresh();
                    }
                    else
                    {
                        bingMapsKeyEditText.Enabled = true;
                        bingMapsKeyOkButton.Enabled = true;
                        Toast.MakeText(context, "The input BingMapKey is not validate.", ToastLength.Long).Show();
                    }
                });
            });

        }

        private void BingMapsKeyCancelButton_Click(object sender, EventArgs e)
        {
            Cancel();
        }

        private bool Validate(string bingMapsKey, BingMapsMapType mapType)
        {
            bool result = false;

            URL url = null;
            Stream stream = null;
            URLConnection conn = null;

            string loginServiceTemplate = "http://dev.virtualearth.net/REST/v1/Imagery/Metadata/{0}?&incl=ImageryProviders&o=xml&key={1}";

            try
            {
                string loginServiceUri = string.Format(CultureInfo.InvariantCulture, loginServiceTemplate, mapType, bingMapsKey);

                url = new URL(loginServiceUri);
                conn = url.OpenConnection();
                stream = conn.InputStream;

                if (stream != null)
                {
                    XmlDocument xDoc = new XmlDocument();
                    xDoc.Load(stream);
                    XmlNamespaceManager nsmgr = new XmlNamespaceManager(xDoc.NameTable);
                    nsmgr.AddNamespace("bing", "http://schemas.microsoft.com/search/local/ws/rest/v1");

                    XmlNode root = xDoc.SelectSingleNode("bing:Response", nsmgr);
                    XmlNode imageUrlElement = root.SelectSingleNode("bing:ResourceSets/bing:ResourceSet/bing:Resources/bing:ImageryMetadata/bing:ImageUrl", nsmgr);
                    XmlNodeList subdomainsElement = root.SelectNodes("bing:ResourceSets/bing:ResourceSet/bing:Resources/bing:ImageryMetadata/bing:ImageUrlSubdomains/bing:string", nsmgr);
                    if (imageUrlElement != null && subdomainsElement != null)
                    {
                        result = true;
                    }
                }
            }
            catch
            { }
            finally
            {
                if (url != null) url.Dispose();
                if (conn != null) conn.Dispose();
                if (stream != null) stream.Dispose();
            }

            return result;
        }
    }
}