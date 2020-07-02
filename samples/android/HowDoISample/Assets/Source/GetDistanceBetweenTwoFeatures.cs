using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.ObjectModel;
using ThinkGeo.Core;

namespace ThinkGeo.UI.Android.HowDoI
{ 
    public class GetDistanceBetweenTwoFeatures : SampleFragment
    {
        private MapView androidMap;
        private TextView distanceTextView;

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnStart();

            ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer(SampleHelper.GetDataPath(@"SampleData/Countries02.shp"));
            worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 233, 232, 214), GeoColor.FromArgb(255, 118, 138, 69));
            worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            Marker usMarker = new Marker(this.Context);
            usMarker.Position = new PointShape(-98.58, 39.57);
            usMarker.YOffset = -22;
            usMarker.SetImageBitmap(BitmapFactory.DecodeResource(Resources, Resource.Drawable.Pin));

            Marker chinaMarker = new Marker(this.Context);
            chinaMarker.Position = new PointShape(104.72, 34.45);
            chinaMarker.YOffset = -22;
            chinaMarker.SetImageBitmap(BitmapFactory.DecodeResource(Resources, Resource.Drawable.Pin));

            MarkerOverlay markerOverlay = new MarkerOverlay();
            markerOverlay.Markers.Add(usMarker);
            markerOverlay.Markers.Add(chinaMarker);

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(worldLayer);

            
            androidMap.MapUnit = GeographyUnit.DecimalDegree;
            androidMap.CurrentExtent = new RectangleShape(-133.2515625, 89.2484375, 126.9046875, -88.290625);
            androidMap.Overlays.Add(layerOverlay);
            androidMap.Overlays.Add("Marker", markerOverlay);

            Button getDistanceButton = new Button(this.Context);
            getDistanceButton.Click += GetDistanceButtonClick;
            getDistanceButton.Text = "Get Distance";
            getDistanceButton.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);

            distanceTextView = new TextView(this.Context);

            LinearLayout linearLayout = new LinearLayout(this.Context);
            linearLayout.Orientation = Orientation.Horizontal;
            linearLayout.AddView(getDistanceButton);
            linearLayout.AddView(distanceTextView);

            SampleViewHelper.InitializeInstruction(this.Context, currentView.FindViewById<RelativeLayout>(Resource.Id.MainLayout), base.SampleInfo, new Collection<View>() { linearLayout });
        }

        private void GetDistanceButtonClick(object sender, EventArgs e)
        {
            MarkerOverlay markerOverlayr = androidMap.Overlays["Marker"] as MarkerOverlay;
            Marker usMarker = markerOverlayr.Markers[0];
            Marker chinaMarker = markerOverlayr.Markers[1];
            double distance = usMarker.Position.GetDistanceTo(chinaMarker.Position, GeographyUnit.DecimalDegree, DistanceUnit.Kilometer);
            distanceTextView.Text = string.Format("{0:N4} Km", distance);
        }
    }
}