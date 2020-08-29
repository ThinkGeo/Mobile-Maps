using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinkGeo.Core;
using ThinkGeo.UI.XamarinForms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UsingMarkersSample : ContentPage
    {
        public UsingMarkersSample()
        {
            InitializeComponent();
        }

        /// <summary>
        /// ...
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            // Set the map's unit of measurement to meters(Spherical Mercator)
            mapView.MapUnit = GeographyUnit.Meter;
            
            // Add Cloud Maps as a background overlay
            var thinkGeoCloudVectorMapsOverlay = new ThinkGeoCloudVectorMapsOverlay("9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~", "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~", ThinkGeoCloudVectorMapsMapType.Light);
            thinkGeoCloudVectorMapsOverlay.VectorTileCache = new FileVectorTileCache(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "cache"), "CloudMapsVector");
            mapView.Overlays.Add(thinkGeoCloudVectorMapsOverlay);

            // Set the map extent
            mapView.CurrentExtent = new RectangleShape(-10778329.017082, 3909598.36751101, -10776250.8853871, 3907890.47766975);

            AddSimpleMarkers();

            mapView.Refresh();
        }

        /// <summary>
        /// Add a SimpleMarkerOverlay to the map
        /// </summary>
        private void AddSimpleMarkers()
        {
            SimpleMarkerOverlay simpleMarkerOverlay = new SimpleMarkerOverlay();
            mapView.Overlays.Add("simpleMarkerOverlay", simpleMarkerOverlay);
        }

        /// <summary>
        /// Adds a marker to the simpleMarkerOverlay where the map click event occurred.
        /// </summary>
        private void MapView_OnMapTouch(object sender, TouchMapViewEventArgs e)
        {
            SimpleMarkerOverlay simpleMarkerOverlay = (SimpleMarkerOverlay)mapView.Overlays["simpleMarkerOverlay"];

            // Create a marker at the position the mouse was clicked
            var marker = new Marker()
            {
                Position = e.PointInWorldCoordinate,
                ImageSource = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Resources/AQUA.png"),
                YOffset = -17
            };

            // Add the marker to the simpleMarkerOverlay and refresh the map
            simpleMarkerOverlay.Markers.Add(marker);
            simpleMarkerOverlay.Refresh();
        }

        /// <summary>
        /// Sets the simpleMarkerOverlay's drag mode to none, meaning that the markers cannot be moved or manipulated.
        /// </summary>
        private void StaticMode_OnClick(object sender, EventArgs e)
        {
            //MarkerOverlay simpleMarkerOverlay = (MarkerOverlay)mapView.Overlays["simpleMarkerOverlay"];
            //simpleMarkerOverlay.DragMode = MarkerDragMode.None;
        }

        /// <summary>
        /// Sets the simpleMarkerOverlay's drag mode to drag, which allows the user to click and drag on an icon to move it.
        /// </summary>
        private void DragMode_OnClick(object sender, EventArgs e)
        {
            // MarkerOverlay simpleMarkerOverlay = (MarkerOverlay)mapView.Overlays["simpleMarkerOverlay"];
            // simpleMarkerOverlay.DragMode = MarkerDragMode.Drag;
        }

        /// <summary>
        /// Sets the simpleMarkerOverlay's drag mode to copy, which allows the user to copy an existing marker by shift-clicking and dragging it.
        /// </summary>
        private void CopyMode_OnClick(object sender, EventArgs e)
        {
            //MarkerOverlay simpleMarkerOverlay = (MarkerOverlay)mapView.Overlays["simpleMarkerOverlay"];
            //simpleMarkerOverlay.DragMode = MarkerDragMode.CopyWithShiftKey;
        }
    }
}
