using CoreGraphics;
using System;
using ThinkGeo.Core;
using UIKit;

namespace ThinkGeo.UI.iOS.HowDoI
{
    public class UseBingMapsOverlay : BaseViewController
    {
        public UseBingMapsOverlay()
            : base()
        { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            MapView.MapUnit = GeographyUnit.Meter;
            MapView.ZoomLevelSet = new BingMapsZoomLevelSet();

            BingMapsOverlay bingMapOverlay = new BingMapsOverlay();
            bingMapOverlay.TileWidth = 512;
            bingMapOverlay.TileHeight = 512;
            MapView.Overlays.Add(bingMapOverlay);

            string rootPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            bingMapOverlay.ApplicationId = "Your Application ID";
            bingMapOverlay.ApplicationId = "Amg9BxyuF81NEyxKm2ESMaoL03MTvaYBV3KOfxpHeXDsEt38DVwK4-SPFPg6qcBp";
            bingMapOverlay.MapStyle = BingMapsMapType.AerialWithLabels;
            bingMapOverlay.TileCache = new FileRasterTileCache(rootPath + "/CacheImages", "Road");
            bingMapOverlay.TileCache.ImageFormat = RasterTileFormat.Jpeg;

            MapView.CurrentExtent = new RectangleShape(-20000000, 20000000, 20000000, -20000000);

            MapView.Refresh();
            //InitialImageView();
        }

        private void InitialImageView()
        {
            UIImageView imageView = new UIImageView(View.Bounds);
            imageView.Image = UIImage.FromBundle("GoogleMap");

            UIView infoLabel = new UIView();
            infoLabel.Frame = new CGRect(10, View.Frame.Height / 2, View.Frame.Width - 20, 70);
            infoLabel.BackgroundColor = UIColor.Yellow;
            infoLabel.Layer.CornerRadius = 2;

            UILabel textInfoLable = new UILabel();
            textInfoLable.TextAlignment = UITextAlignment.Center;
            textInfoLable.Frame = new CGRect(25, 18, infoLabel.Frame.Width, 20);
            textInfoLable.Text = "Bing Maps require an API Key.";
            infoLabel.Add(textInfoLable);

            UILabel smallLable = new UILabel();
            smallLable.TextAlignment = UITextAlignment.Center;
            smallLable.Frame = new CGRect(25, 38, infoLabel.Frame.Width, 20);
            smallLable.Text = "please modify the source code to make it work.";
            smallLable.Font = UIFont.FromName("Helvetica-Bold", 11);
            infoLabel.Add(smallLable);

            View.Add(imageView);
            View.Add(infoLabel);
        }

        protected override void InitializeInstruction()
        {
            SampleUIHelper.InitializeInstruction(MapView, View, GetType(), isIphone => isIphone ? 100 : 80);
        }
    }
}