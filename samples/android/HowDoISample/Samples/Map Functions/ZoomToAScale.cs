using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using ThinkGeo.Core;

namespace ThinkGeo.UI.Android.HowDoI
{
    public class ZoomToAScale : SampleFragment
    {
        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnStart();

            ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer(SampleHelper.GetDataPath(@"SampleData/Countries02.shp"));
            worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 233, 232, 214), GeoColor.FromArgb(255, 118, 138, 69));
            worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(worldLayer);

            //
            androidMap.MapUnit = GeographyUnit.DecimalDegree;
            androidMap.CurrentExtent = new RectangleShape(-133.2515625, 89.2484375, 126.9046875, -88.290625);
            androidMap.Overlays.Add(layerOverlay);

            LinearLayout linearLayout = new LinearLayout(this.Context);
            linearLayout.Orientation = Orientation.Horizontal;

            Button zoomToButton = new Button(this.Context);
            zoomToButton.Text = "1:1,000,000";
            zoomToButton.Click += ZoomToButtonClick;

            Button zoomToFiveButton = new Button(this.Context);
            zoomToFiveButton.Text = "1:5,000,000";
            zoomToFiveButton.Click += ZoomToButtonClick;

            linearLayout.AddView(zoomToButton);
            linearLayout.AddView(zoomToFiveButton);

            SampleViewHelper.InitializeInstruction(this.Context, currentView.FindViewById<RelativeLayout>(Resource.Id.MainLayout), this.SampleInfo, new Collection<View>() { linearLayout });
        }

        private void ZoomToButtonClick(object sender, EventArgs e)
        {
            var button = sender as Button;
            string zoomLevelScale = button.Text;
            double scale = Convert.ToDouble(zoomLevelScale.Split(':')[1], CultureInfo.InvariantCulture);
            androidMap.ZoomToScale(scale);
        }
    }
}