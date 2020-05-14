using Foundation;
using UIKit;
using System;
using System.Globalization;
using System.IO;

namespace ThinkGeo.UI.iOS.HowDoI
{
    public class WebViewController : UIViewController
    {
        private UIWebView webView;
        private string sampleName;
        private static string sourceFolder = "AppData/Source";
        private static string myDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private static string htmlFolder = Path.Combine(myDocuments, "SourceCode");
        private static string startHtml = "<body oncontextmenu='return false;'><div class='divbody'><pre name='code' class='c-sharp:nocontrols'>";
        private static string endHtml = "</pre></div><link type='text/css' rel='stylesheet' href='../SyntaxHighlighter.css'></link><script language='javascript' src='../shCore.js'></script><script language='javascript' src='../shBrushCSharp.js'></script><script language='javascript' src='../shBrushXml.js'></script><script language='javascript'>dp.SyntaxHighlighter.HighlightAll('code');</script></body>";

        public WebViewController()
            : base()
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            webView = new UIWebView(View.Frame);

            View.BackgroundColor = UIColor.White;
            View.Frame = UIScreen.MainScreen.Bounds;
            View.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
            View.AddSubview(webView);

            CopyStyleFiles();
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            webView.LoadRequest(new NSUrlRequest(new NSUrl(GetSourceFullName(sampleName), false)));
        }

        public string SampleName
        {
            get { return sampleName; }
            set { sampleName = value; }
        }

        private void CopyStyleFiles()
        {
            string syntaxHighlighterPath = Path.Combine(myDocuments, "SyntaxHighlighter.css");
            string shBrushCSharpPath = Path.Combine(myDocuments, "shBrushCSharp.js");
            string shBrushXmlPath = Path.Combine(myDocuments, "shBrushXml.js");
            string shCorePath = Path.Combine(myDocuments, "shCore.js");

            if (!File.Exists(syntaxHighlighterPath))
                File.Copy("AppData/SyntaxHighlighter/SyntaxHighlighter.css", syntaxHighlighterPath);
            if (!File.Exists(shBrushCSharpPath))
                File.Copy("AppData/SyntaxHighlighter/shBrushCSharp.js", shBrushCSharpPath);
            if (!File.Exists(shBrushXmlPath))
                File.Copy("AppData/SyntaxHighlighter/shBrushXml.js", shBrushXmlPath);
            if (!File.Exists(shCorePath))
                File.Copy("AppData/SyntaxHighlighter/shCore.js", shCorePath);
        }

        private string GetSourceFullName(string sampleName)
        {
            string sourceFullName = String.Format(CultureInfo.InvariantCulture, "{0}/{1}.cs", sourceFolder, sampleName);
            string htmlFullName = String.Format(CultureInfo.InvariantCulture, "{0}/{1}.html", htmlFolder, sampleName);

            if (!File.Exists(htmlFullName))
            {
                StreamReader reader = null;
                StreamWriter writer = null;
                try
                {
                    reader = new StreamReader(sourceFullName);

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
    }
}