using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using ThinkGeo.Core;

namespace ThinkGeo.UI.Android.HowDoI
{
    public class WorldCoordinatesToScreenCoordinates : SampleFragment
    {
        private MapView androidMap;
        private TextView resultView;
        private EditText longitudeTextView;
        private EditText latitudeTextView;

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnStart();

            ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer(SampleHelper.GetDataPath(@"SampleData/Countries02.shp"));
            worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 233, 232, 214), GeoColor.FromArgb(255, 118, 138, 69));
            worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            Marker thinkGeoMarker = new Marker(this.Context);
            thinkGeoMarker.Position = new PointShape(-95.2806, 38.9554);
            thinkGeoMarker.YOffset = -22;
            thinkGeoMarker.SetImageBitmap(BitmapFactory.DecodeResource(Resources, Resource.Drawable.Pin));

            MarkerOverlay markerOverlay = new MarkerOverlay();
            markerOverlay.Markers.Add(thinkGeoMarker);

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(worldLayer);

            
            androidMap.MapUnit = GeographyUnit.DecimalDegree;
            androidMap.CurrentExtent = new RectangleShape(-133.2515625, 89.2484375, 126.9046875, -88.290625);
            androidMap.Overlays.Add(layerOverlay);
            androidMap.Overlays.Add(markerOverlay);

            Button convertButton = new Button(this.Context);
            convertButton.Click += ConvertButtonClick;
            convertButton.Text = "Convert";
            convertButton.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.MatchParent);

            longitudeTextView = new EditText(this.Context);
            longitudeTextView.Text = "-95.2806";
            latitudeTextView = new EditText(this.Context);
            latitudeTextView.Text = "38.9554";

            resultView = new TextView(this.Context);

            LinearLayout verticalLinearLayout = new LinearLayout(this.Context);
            verticalLinearLayout.Orientation = Orientation.Vertical;
            verticalLinearLayout.AddView(longitudeTextView);
            verticalLinearLayout.AddView(latitudeTextView);

            LinearLayout horizontalLinearLayout = new LinearLayout(this.Context);
            horizontalLinearLayout.Orientation = Orientation.Horizontal;
            horizontalLinearLayout.AddView(verticalLinearLayout);
            horizontalLinearLayout.AddView(convertButton);
            horizontalLinearLayout.AddView(resultView);

            SampleViewHelper.InitializeInstruction(this.Context, currentView.FindViewById<RelativeLayout>(Resource.Id.MainLayout), base.SampleInfo, new Collection<View>() { horizontalLinearLayout });
        }


        private void ConvertButtonClick(object sender, EventArgs e)
        {
            ScreenPointF screenPoint = MapUtil.ToScreenCoordinate(androidMap.CurrentExtent, new PointShape(Double.Parse(longitudeTextView.Text, CultureInfo.InvariantCulture), Double.Parse(latitudeTextView.Text, CultureInfo.InvariantCulture)), androidMap.Width, androidMap.Height);
            resultView.Text = string.Format("Screen Position:({0:N4},{1:N4})", screenPoint.X, screenPoint.Y);
        }
    }
}