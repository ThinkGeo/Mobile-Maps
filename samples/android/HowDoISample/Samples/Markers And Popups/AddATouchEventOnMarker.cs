using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Util;
using Android.Widget;
using ThinkGeo.Core;

namespace ThinkGeo.UI.Android.HowDoI
{
    public class AddATouchEventOnMarker : SampleFragment
    {
        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnStart();

            ShapeFileFeatureLayer txwatFeatureLayer = new ShapeFileFeatureLayer(SampleHelper.GetDataPath(@"Frisco/TXwat.shp"));
            txwatFeatureLayer.ZoomLevelSet.ZoomLevel12.DefaultAreaStyle.FillBrush = new GeoSolidBrush(GeoColor.FromArgb(255, 153, 179, 204));
            txwatFeatureLayer.ZoomLevelSet.ZoomLevel12.DefaultTextStyle = TextStyle.CreateSimpleTextStyle("LandName", "Arial", 9, DrawingFontStyles.Italic, GeoColors.Navy);
            txwatFeatureLayer.ZoomLevelSet.ZoomLevel12.DefaultTextStyle.SuppressPartialLabels = true;
            txwatFeatureLayer.ZoomLevelSet.ZoomLevel12.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            ShapeFileFeatureLayer txlkaA40FeatureLayer = new ShapeFileFeatureLayer(SampleHelper.GetDataPath(@"Frisco/TXlkaA40.shp"));
            txlkaA40FeatureLayer.ZoomLevelSet.ZoomLevel14.DefaultLineStyle = LineStyle.CreateSimpleLineStyle(GeoColors.DarkGray, 1F, false);
            txlkaA40FeatureLayer.ZoomLevelSet.ZoomLevel15.DefaultLineStyle = LineStyle.CreateSimpleLineStyle(GeoColors.White, 3F, GeoColors.DarkGray, 5F, true);
            txlkaA40FeatureLayer.ZoomLevelSet.ZoomLevel16.DefaultLineStyle = LineStyle.CreateSimpleLineStyle(GeoColors.White, 8F, GeoColors.DarkGray, 10F, true);
            txlkaA40FeatureLayer.ZoomLevelSet.ZoomLevel16.DefaultTextStyle = TextStyle.CreateSimpleTextStyle("[fedirp] [fename] [fetype] [fedirs]", "Arial", 10f, DrawingFontStyles.Regular, GeoColors.Black, 0, -1);
            txlkaA40FeatureLayer.ZoomLevelSet.ZoomLevel16.DefaultTextStyle.SuppressPartialLabels = true;
            txlkaA40FeatureLayer.ZoomLevelSet.ZoomLevel16.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            txlkaA40FeatureLayer.DrawingMarginInPixel = 200;

            ShapeFileFeatureLayer txlkaA20FeatureLayer = new ShapeFileFeatureLayer(SampleHelper.GetDataPath(@"Frisco/TXlkaA20.shp"));
            txlkaA20FeatureLayer.ZoomLevelSet.ZoomLevel15.DefaultLineStyle = LineStyle.CreateSimpleLineStyle(GeoColor.FromArgb(255, 255, 255, 128), 6, GeoColors.LightGray, 9, true);
            txlkaA20FeatureLayer.ZoomLevelSet.ZoomLevel16.DefaultLineStyle = LineStyle.CreateSimpleLineStyle(GeoColor.FromArgb(255, 255, 255, 128), 9, GeoColors.LightGray, 12, true);
            txlkaA20FeatureLayer.ZoomLevelSet.ZoomLevel16.DefaultTextStyle = TextStyle.CreateSimpleTextStyle("[fedirp] [fename] [fetype] [fedirs]", "Arial", 12, DrawingFontStyles.Regular, GeoColors.Black, 0, -1);
            txlkaA20FeatureLayer.ZoomLevelSet.ZoomLevel16.DefaultTextStyle.SuppressPartialLabels = true;
            txlkaA20FeatureLayer.ZoomLevelSet.ZoomLevel16.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            ThinkGeoCloudRasterMapsOverlay ThinkGeoCloudRasterMapsOverlay = new ThinkGeoCloudRasterMapsOverlay(SampleHelper.ThinkGeoCloudId, SampleHelper.ThinkGeoCloudSecret);

            Marker thinkGeoMarker = new Marker(this.Context);
            thinkGeoMarker.Position = new PointShape(-96.809523, 33.128675);
            thinkGeoMarker.YOffset = -(int)(22 * Resources.DisplayMetrics.Density);
            thinkGeoMarker.SetImageBitmap(BitmapFactory.DecodeResource(Resources, Resource.Drawable.Pin));
            thinkGeoMarker.Click += ThinkGeoMarkerClick;

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(txlkaA40FeatureLayer);
            layerOverlay.Layers.Add(txwatFeatureLayer);
            layerOverlay.Layers.Add(txlkaA20FeatureLayer);

            MarkerOverlay markerOverlay = new MarkerOverlay();
            markerOverlay.Markers.Add(thinkGeoMarker);

            
            mapView.MapUnit = GeographyUnit.DecimalDegree;
            mapView.CurrentExtent = new RectangleShape(-96.8172, 33.1299, -96.8050, 33.1226);
            mapView.Overlays.Add(ThinkGeoCloudRasterMapsOverlay);
            mapView.Overlays.Add(layerOverlay);
            mapView.Overlays.Add(markerOverlay);
            mapView.Overlays.Add("PopupOverlay", new PopupOverlay());

            SampleViewHelper.InitializeInstruction(this.Context, currentView.FindViewById<RelativeLayout>(Resource.Id.MainLayout), base.SampleInfo);
        }

        void ThinkGeoMarkerClick(object sender, System.EventArgs e)
        {
            Marker thinkGeoMarker = (Marker)sender;
            PopupOverlay popupOverlay = mapView.Overlays["PopupOverlay"] as PopupOverlay;

            if (popupOverlay.Popups.Count > 0)
            {
                popupOverlay.Popups.Clear();
            }
            else
            {

                ImageView imageView = new ImageView(this.Context);
                imageView.SetImageResource(Resource.Drawable.ThinkGeoLogo);

                TextView textView = new TextView(this.Context);
                textView.Text = string.Format("Longitude : {0:N4}" + "\r\n" + "Latitude : {1:N4}", thinkGeoMarker.Position.X, thinkGeoMarker.Position.Y);
                textView.SetTextColor(Color.Black);
                textView.SetTextSize(ComplexUnitType.Px, 22);

                LinearLayout linearLayout = new LinearLayout(this.Context);
                linearLayout.SetPadding(10, 10, 10, 10);
                linearLayout.Orientation = Orientation.Vertical;
                linearLayout.AddView(imageView);
                linearLayout.AddView(textView);

                Popup popup = new Popup(this.Context);
                popup.Position = thinkGeoMarker.Position;
                popup.YOffset = (int)(-44 * Resources.DisplayMetrics.Density);
                popup.XOffset = (int)(4 * Resources.DisplayMetrics.Density);
                popup.AddView(linearLayout);

                popupOverlay.Popups.Add(popup);
            }
            popupOverlay.Refresh();
        }
    }
}