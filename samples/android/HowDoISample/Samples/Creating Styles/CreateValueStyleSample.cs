using System;
using System.Collections.ObjectModel;
using Android.OS;
using Android.Widget;
using ThinkGeo.Core;

namespace ThinkGeo.UI.Android.HowDoI
{
    /// <summary>
    /// Learn how to ...
    /// </summary>
    public class CreateValueStyleSample : SampleFragment
    {
        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            SetupSample();

            SetupMap();
        }

        /// <summary>
        /// Sets up the sample's layout and controls
        /// </summary>
        private void SetupSample()
        {
            base.OnStart();

            //Button button = new Button(this.Context);
            //button.Text = "OneFeature";
            //button.Click += Button_Click;

            //LinearLayout linearLayout = new LinearLayout(this.Context);
            //linearLayout.Orientation = Orientation.Horizontal;

            //linearLayout.AddView(button);

            //SampleViewHelper.InitializeInstruction(this.Context, currentView.FindViewById<RelativeLayout>(Resource.Id.MainLayout), this.SampleInfo, new Collection<View>() { linearLayout });

            SampleViewHelper.InitializeInstruction(this.Context, currentView.FindViewById<RelativeLayout>(Resource.Id.MainLayout), base.SampleInfo);
        }

        /// <summary>
        /// Sets up the map layers and styles
        /// </summary>
        private void SetupMap()
        {
            // Set the map's unit of measurement to meters(Spherical Mercator)
            mapView.MapUnit = GeographyUnit.Meter;

            // Add Cloud Maps as a background overlay
            var thinkGeoCloudVectorMapsOverlay = new ThinkGeoCloudVectorMapsOverlay("USlbIyO5uIMja2y0qoM21RRM6NBXUad4hjK3NBD6pD0~", "f6OJsvCDDzmccnevX55nL7nXpPDXXKANe5cN6czVjCH0s8jhpCH-2A~~", ThinkGeoCloudVectorMapsMapType.Light);
            mapView.Overlays.Add(thinkGeoCloudVectorMapsOverlay);

            // Set the zoom levels to match cloud maps
            mapView.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();

            // Set the map extent
            mapView.CurrentExtent = new RectangleShape(-10786436, 3918518, -10769429, 3906002);
        }

        //private void Button_Click(object sender, EventArgs e)
        //{
        //    throw new NotImplementedException();
        //}
    }
}