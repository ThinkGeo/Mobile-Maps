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
using ThinkGeo.MapSuite.Android;

namespace MapSuiteSiteSelection
{
    public class InputBingMapKeyDialog : AlertDialog
    {
        private Context context;
        private View inputBingMapsKeyView;
        private Button bingMapsKeyOkButton;

        public InputBingMapKeyDialog(Context context)
            : base(context)
        {
            inputBingMapsKeyView = View.Inflate(context, Resource.Layout.BingMapsKeyLayout, null);
            this.SetView(inputBingMapsKeyView);

            this.context = context;

            EditText bingMapsKeyEditText = inputBingMapsKeyView.FindViewById<EditText>(Resource.Id.BingMapsKeyEditText);
            bingMapsKeyOkButton = inputBingMapsKeyView.FindViewById<Button>(Resource.Id.OkButton);
            Button bingMapsKeyCancelButton = inputBingMapsKeyView.FindViewById<Button>(Resource.Id.CancelButton);

            bingMapsKeyOkButton.Click += BingMapsKeyOkButton_Click;
            bingMapsKeyCancelButton.Click += BingMapsKeyCancelButton_Click;
        }

        private void BingMapsKeyOkButton_Click(object sender, EventArgs e)
        {
            EditText bingMapsKeyEditText = inputBingMapsKeyView.FindViewById<EditText>(Resource.Id.BingMapsKeyEditText);
            bingMapsKeyEditText.Enabled = false;
            bingMapsKeyOkButton.Enabled = false;

            Task.Factory.StartNew(() =>
            {
                bool isValid = Validate(bingMapsKeyEditText.Text, ThinkGeo.MapSuite.Android.BingMapsMapType.Aerial);
                SampleMapView.Current.Post(() =>
                {
                    if (isValid)
                    {
                        Cancel();
                        ISharedPreferences preferences = context.GetSharedPreferences(SettingKey.PrefsFile, 0);
                        ISharedPreferencesEditor editor = preferences.Edit();
                        editor.PutString(SettingKey.PrefsBingMapKey, bingMapsKeyEditText.Text);
                        editor.Commit();

                        BingMapsOverlay bingMapsAerialOverlay = SampleMapView.Current.FindOverlay<BingMapsOverlay>(OverlayKey.BingMapsAerialOverlay);
                        BingMapsOverlay bingMapsRoadOverlay = SampleMapView.Current.FindOverlay<BingMapsOverlay>(OverlayKey.BingMapsRoadOverlay);
                        bingMapsAerialOverlay.ApplicationId = bingMapsKeyEditText.Text;
                        bingMapsRoadOverlay.ApplicationId = bingMapsKeyEditText.Text;

                        SampleMapView.Current.SwitchBaseMapTo(SampleMapView.Current.BaseMapType);
                        SampleMapView.Current.Refresh();
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

        private bool Validate(string bingMapsKey, ThinkGeo.MapSuite.Android.BingMapsMapType mapType)
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
                url?.Dispose();
                conn?.Dispose();
                stream?.Dispose();
            }

            return result;
        }
    }
}