using System.IO;
using ThinkGeo.Core;
using UIKit;

namespace ThinkGeo.UI.iOS.HowDoI
{
    public class AddAMarker : BaseViewController
    {
        public AddAMarker()
            : base()
        { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            MapView.MapUnit = GeographyUnit.DecimalDegree;
            MapView.CurrentExtent = new RectangleShape(-96.814230546875, 33.137237109375, -96.805205546875, 33.123699609375);

            LayerOverlay layerOverlay = new LayerOverlay();
            MapView.Overlays.Add(layerOverlay);

            string targetDirectory = "AppData/Frisco";
            ShapeFileFeatureLayer txwatFeatureLayer = new ShapeFileFeatureLayer(Path.Combine(targetDirectory, "TXwat.shp"));
            txwatFeatureLayer.ZoomLevelSet.ZoomLevel12.DefaultAreaStyle.FillBrush = new GeoSolidBrush(GeoColor.FromArgb(255, 153, 179, 204));
            txwatFeatureLayer.ZoomLevelSet.ZoomLevel12.DefaultTextStyle = TextStyle.CreateSimpleTextStyle("LandName", "Arial", 9, DrawingFontStyles.Italic, GeoColors.Navy);
            txwatFeatureLayer.ZoomLevelSet.ZoomLevel12.DefaultTextStyle.SuppressPartialLabels = true;
            txwatFeatureLayer.ZoomLevelSet.ZoomLevel12.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            layerOverlay.Layers.Add("TXwat", txwatFeatureLayer);

            ShapeFileFeatureLayer txlka40FeatureLayer = new ShapeFileFeatureLayer(Path.Combine(targetDirectory, "TXlkaA40.shp"));
            txlka40FeatureLayer.ZoomLevelSet.ZoomLevel14.DefaultLineStyle = LineStyle.CreateSimpleLineStyle(GeoColors.DarkGray, 1F, false);
            txlka40FeatureLayer.ZoomLevelSet.ZoomLevel15.DefaultLineStyle = LineStyle.CreateSimpleLineStyle(GeoColors.White, 3F, GeoColors.DarkGray, 5F, true);
            txlka40FeatureLayer.ZoomLevelSet.ZoomLevel16.DefaultLineStyle = LineStyle.CreateSimpleLineStyle(GeoColors.White, 8F, GeoColors.DarkGray, 10F, true);
            txlka40FeatureLayer.ZoomLevelSet.ZoomLevel16.DefaultTextStyle = TextStyle.CreateSimpleTextStyle("[fedirp] [fename] [fetype] [fedirs]", "Arial", 10f, DrawingFontStyles.Regular, GeoColors.Black, 0, -1);
            txlka40FeatureLayer.ZoomLevelSet.ZoomLevel16.DefaultTextStyle.SuppressPartialLabels = true;
            txlka40FeatureLayer.ZoomLevelSet.ZoomLevel16.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            txlka40FeatureLayer.DrawingMarginInPixel = 128;
            layerOverlay.Layers.Add("TXlkaA40", txlka40FeatureLayer);

            ShapeFileFeatureLayer txlkaA20FeatureLayer = new ShapeFileFeatureLayer(Path.Combine(targetDirectory, "TXlkaA20.shp"));
            txlkaA20FeatureLayer.ZoomLevelSet.ZoomLevel15.DefaultLineStyle = LineStyle.CreateSimpleLineStyle(GeoColor.FromArgb(255, 255, 255, 128), 6, GeoColors.LightGray, 9, true);
            txlkaA20FeatureLayer.ZoomLevelSet.ZoomLevel16.DefaultLineStyle = LineStyle.CreateSimpleLineStyle(GeoColor.FromArgb(255, 255, 255, 128), 9, GeoColors.LightGray, 12, true);
            txlkaA20FeatureLayer.ZoomLevelSet.ZoomLevel16.DefaultTextStyle = TextStyle.CreateSimpleTextStyle("[fedirp] [fename] [fetype] [fedirs]", "Arial", 12, DrawingFontStyles.Regular, GeoColors.Black, 0, -1);
            txlkaA20FeatureLayer.ZoomLevelSet.ZoomLevel16.DefaultTextStyle.SuppressPartialLabels = true;
            txlkaA20FeatureLayer.ZoomLevelSet.ZoomLevel16.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            layerOverlay.Layers.Add("TXlkaA20", txlkaA20FeatureLayer);

            Marker thinkGeoMarker = new Marker();
            thinkGeoMarker.Position = new PointShape(-96.809523, 33.128675);
            thinkGeoMarker.YOffset = -22;
            thinkGeoMarker.SetImage(UIImage.FromBundle("Pin"), UIControlState.Normal);
            SimpleMarkerOverlay markerOverlay = new SimpleMarkerOverlay();
            markerOverlay.Markers.Add(thinkGeoMarker);
            MapView.Overlays.Add(markerOverlay);

            MapView.Refresh();
        }

        protected override void InitializeInstruction()
        {
            SampleUIHelper.InitializeInstruction(MapView, View, GetType(), isIphone => isIphone ? 100 : 80);
        }
    }
}