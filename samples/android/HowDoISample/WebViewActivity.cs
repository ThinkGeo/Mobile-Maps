using Android.App;
using Android.OS;
using Android.Views;
using Android.Webkit;
using System;
using System.Globalization;
using System.IO;
using Id = Android.Resource.Id;

namespace ThinkGeo.UI.Android.HowDoI
{
    [Activity(Label = "View Source")]
    public class WebViewActivity : Activity
    {
        private WebView webView;
        private static string sourceFolder;
        private static string htmlFolder;
        private static string startHtml = "<body oncontextmenu='return false;'><div class='divbody'><pre name='code' class='c-sharp:nocontrols'>";
        private static string endHtml = "</pre></div><link type='text/css' rel='stylesheet' href='../SyntaxHighlighter.css'></link><script language='javascript' src='../shCore.js'></script><script language='javascript' src='../shBrushCSharp.js'></script><script language='javascript' src='../shBrushXml.js'></script><script language='javascript'>dp.SyntaxHighlighter.HighlightAll('code');</script></body>";

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.WebView);

            sourceFolder = "Source";
            htmlFolder = FilesDir.AbsolutePath + "/htmlFolder";

            CopyStyleFiles();

            webView = FindViewById<WebView>(Resource.Id.localWebView);
            webView.Settings.JavaScriptEnabled = true;
            webView.LoadUrl("file:///" + GetSourceFullName(Intent.GetStringExtra("SampleName")));
        }

        private void CopyStyleFiles()
        {
            string syntaxHighlighterPath = Path.Combine(FilesDir.AbsolutePath, "SyntaxHighlighter.css");
            string shBrushCSharpPath = Path.Combine(FilesDir.AbsolutePath, "shBrushCSharp.js");
            string shBrushXmlPath = Path.Combine(FilesDir.AbsolutePath, "shBrushXml.js");
            string shCorePath = Path.Combine(FilesDir.AbsolutePath, "shCore.js");

            if (!File.Exists(syntaxHighlighterPath))
            {
                Stream sourceStream = Assets.Open("SyntaxHighlighter/SyntaxHighlighter.css");
                Stream targetStream = File.Create(syntaxHighlighterPath);
                sourceStream.CopyTo(targetStream);
                sourceStream.Close();
                targetStream.Close();
            }
            if (!File.Exists(shBrushCSharpPath))
            {
                Stream sourceStream = Assets.Open("SyntaxHighlighter/shBrushCSharp.js");
                Stream targetStream = File.Create(shBrushCSharpPath);
                sourceStream.CopyTo(targetStream);
                sourceStream.Close();
                targetStream.Close();
            }
            if (!File.Exists(shBrushXmlPath))
            {
                Stream sourceStream = Assets.Open("SyntaxHighlighter/shBrushXml.js");
                Stream targetStream = File.Create(shBrushXmlPath);
                sourceStream.CopyTo(targetStream);
                sourceStream.Close();
                targetStream.Close();
            }
            if (!File.Exists(shCorePath))
            {
                Stream sourceStream = Assets.Open("SyntaxHighlighter/shCore.js");
                Stream targetStream = File.Create(shCorePath);
                sourceStream.CopyTo(targetStream);
                sourceStream.Close();
                targetStream.Close();
            }
        }

        private string GetSourceFullName(string sampleName)
        {
            string htmlFullName = String.Format(CultureInfo.InvariantCulture, "{0}/{1}.html", htmlFolder, sampleName);

            if (!File.Exists(htmlFullName))
            {
                StreamReader reader = null;
                StreamWriter writer = null;
                try
                {
                    reader = new StreamReader(Assets.Open(string.Format("{0}/{1}.cs", sourceFolder, sampleName)));

                    if (!Directory.Exists(htmlFolder))
                        Directory.CreateDirectory(htmlFolder);

                    writer = new StreamWriter(File.Create(htmlFullName));
                    writer.Write(startHtml);
                    writer.Write(reader.ReadToEnd());
                    writer.Write(endHtml);
                }
                finally
                {
                    if (reader != null) { reader.Close(); reader.Dispose(); }
                    if (writer != null) { writer.Close(); writer.Dispose(); }
                }
            }
            return htmlFullName;
        }

        protected override void OnStart()
        {
            base.OnStart();
            ActionBar.SetHomeButtonEnabled(true);
            ActionBar.SetDisplayHomeAsUpEnabled(true);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Id.Home:
                    Finish();
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }


    }
}