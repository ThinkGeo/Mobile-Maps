using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.ObjectModel;
using ThinkGeo.Core;

namespace ThinkGeo.UI.Android.HowDoI
{ 
    public class ConvertToFromWellKnownBinary : SampleFragment
    {
        private TextView wkbTextView;
        private TextView wktTextView;

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnStart();

            ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer(SampleHelper.GetDataPath(@"SampleData/Countries02.shp"));
            worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 233, 232, 214), GeoColor.FromArgb(255, 118, 138, 69));
            worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(worldLayer);

            androidMap.MapUnit = GeographyUnit.DecimalDegree;
            androidMap.CurrentExtent = new RectangleShape(-133.2515625, 89.2484375, 126.9046875, -88.290625);
            androidMap.Overlays.Add(layerOverlay);

            Button convertButton = new Button(this.Context);
            convertButton.Text = "Convert";
            convertButton.Click += ConvertButtonClick;

            wkbTextView = new TextView(this.Context);
            wkbTextView.Text = "AQEAAAAAAAAAAAAkQAAAAAAAADRA";

            wktTextView = new TextView(this.Context);

            LinearLayout linearLayout = new LinearLayout(this.Context);
            linearLayout.Orientation = Orientation.Horizontal;

            linearLayout.AddView(wkbTextView);
            linearLayout.AddView(convertButton);
            linearLayout.AddView(wktTextView);

            SampleViewHelper.InitializeInstruction(this.Context,currentView. FindViewById<RelativeLayout>(Resource.Id.MainLayout), base.SampleInfo, new Collection<View>() { linearLayout });
        }

        private void ConvertButtonClick(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(wkbTextView.Text))
            {
                byte[] wellKnownBinary = Convert.FromBase64String(wkbTextView.Text);
                Feature feature = new Feature(wellKnownBinary);

                wktTextView.Text = feature.GetWellKnownText();
                wkbTextView.Text = string.Empty;
            }
            else if (!string.IsNullOrEmpty(wktTextView.Text))
            {
                Feature feature = new Feature(wktTextView.Text);
                byte[] wellKnownBinary = feature.GetWellKnownBinary();

                wkbTextView.Text = Convert.ToBase64String(wellKnownBinary);
                wktTextView.Text = string.Empty;
            }
        }
    }
}