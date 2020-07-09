using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using System.Collections.ObjectModel;
using System.Globalization;
using ThinkGeo.Core;

namespace ThinkGeo.UI.Android.HowDoI
{ 
    public class LengthOfAFeature : SampleFragment
    {
        private TextView messageLabel;

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnStart();

            ShapeFileFeatureLayer txlka40FeatureLayer = new ShapeFileFeatureLayer(SampleHelper.GetDataPath(@"Frisco/TXlkaA40.shp"));
            txlka40FeatureLayer.ZoomLevelSet.ZoomLevel14.DefaultLineStyle = LineStyle.CreateSimpleLineStyle(GeoColors.DarkGray, 1F, false);
            txlka40FeatureLayer.ZoomLevelSet.ZoomLevel15.DefaultLineStyle = LineStyle.CreateSimpleLineStyle(GeoColors.White, 3F, GeoColors.DarkGray, 5F, true);
            txlka40FeatureLayer.ZoomLevelSet.ZoomLevel16.DefaultLineStyle = LineStyle.CreateSimpleLineStyle(GeoColors.White, 8F, GeoColors.DarkGray, 10F, true);
            txlka40FeatureLayer.ZoomLevelSet.ZoomLevel16.DefaultTextStyle = TextStyle.CreateSimpleTextStyle("[fedirp] [fename] [fetype] [fedirs]", "Arial", 10f, DrawingFontStyles.Regular, GeoColors.Black, 0, -1);
            txlka40FeatureLayer.ZoomLevelSet.ZoomLevel16.DefaultTextStyle.SuppressPartialLabels = true;
            txlka40FeatureLayer.ZoomLevelSet.ZoomLevel16.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            txlka40FeatureLayer.DrawingMarginInPixel = 200;

            InMemoryFeatureLayer highlightLayer = new InMemoryFeatureLayer();
            highlightLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = LineStyle.CreateSimpleLineStyle(GeoColors.White, 9.2F, GeoColors.DarkGray, 12.2F, true);
            highlightLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle.InnerPen.Brush = new GeoSolidBrush(GeoColor.FromArgb(150, GeoColors.Blue));
            highlightLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add("TXlkaA40", txlka40FeatureLayer);

            LayerOverlay highlightOverlay = new LayerOverlay();
            highlightOverlay.Layers.Add("HighlightLayer", highlightLayer);

            
            mapView.MapUnit = GeographyUnit.DecimalDegree;
            mapView.CurrentExtent = new RectangleShape(-96.8172, 33.1299, -96.8050, 33.1226);
            mapView.Overlays.Add("RoadOverlay", layerOverlay);
            mapView.Overlays.Add("HighlightOverlay", highlightOverlay);
            mapView.SingleTap += mapView_SingleTap;

            messageLabel = new TextView(this.Context);
            SampleViewHelper.InitializeInstruction(this.Context, currentView.FindViewById<RelativeLayout>(Resource.Id.MainLayout), base.SampleInfo, new Collection<View>() { messageLabel });
        }

        private void mapView_SingleTap(object sender, SingleTapMapViewEventArgs e)
        {
            LayerOverlay overlay = mapView.Overlays["RoadOverlay"] as LayerOverlay;
            FeatureLayer roadLayer = overlay.Layers["TXlkaA40"] as FeatureLayer;

            LayerOverlay highlightOverlay = mapView.Overlays["HighlightOverlay"] as LayerOverlay;
            InMemoryFeatureLayer highlightLayer = (InMemoryFeatureLayer)highlightOverlay.Layers["HighlightLayer"];

            roadLayer.Open();
            Collection<Feature> selectedFeatures = roadLayer.QueryTools.GetFeaturesNearestTo(e.WorldPoint, GeographyUnit.DecimalDegree, 1, new string[1] { "fename" });
            roadLayer.Close();

            if (selectedFeatures.Count > 0)
            {
                LineBaseShape lineShape = (LineBaseShape)selectedFeatures[0].GetShape();
                highlightLayer.Open();
                highlightLayer.InternalFeatures.Clear();
                highlightLayer.InternalFeatures.Add(new Feature(lineShape));
                highlightLayer.Close();

                double length = lineShape.GetLength(GeographyUnit.DecimalDegree, DistanceUnit.Meter);
                string lengthMessage = string.Format(CultureInfo.InvariantCulture, "{0} has a length of {1:F2} meters.", selectedFeatures[0].ColumnValues["fename"].Trim(), length);

                messageLabel.Text = lengthMessage;
                highlightOverlay.Refresh();
            }
        }
    }
}