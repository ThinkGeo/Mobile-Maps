using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinkGeo.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.Xamarin.HowDoI
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
        private void MapView_Loaded(object sender, EventArgs e)
        {
            //// Set the map's unit of measurement to meters(Spherical Mercator)
            //mapView.MapUnit = GeographyUnit.Meter;

            //// Add Cloud Maps as a background overlay
            //var thinkGeoCloudVectorMapsOverlay = new ThinkGeoCloudVectorMapsOverlay("itZGOI8oafZwmtxP-XGiMvfWJPPc-dX35DmESmLlQIU~", "bcaCzPpmOG6le2pUz5EAaEKYI-KSMny_WxEAe7gMNQgGeN9sqL12OA~~", ThinkGeoCloudVectorMapsMapType.Light);
            //mapView.Overlays.Add(thinkGeoCloudVectorMapsOverlay);

            //// Set the map extent
            //mapView.CurrentExtent = new RectangleShape(-10778329.017082, 3909598.36751101, -10776250.8853871, 3907890.47766975);

            //AddSimpleMarkers();
        }

        /// <summary>
        /// Add a SimpleMarkerOverlay to the map
        /// </summary>
        private void AddSimpleMarkers()
        {
            //SimpleMarkerOverlay simpleMarkerOverlay = new SimpleMarkerOverlay();
            //mapView.Overlays.Add("simpleMarkerOverlay", simpleMarkerOverlay);
        }

        /// <summary>
        /// Adds a marker to the simpleMarkerOverlay where the map click event occurred.
        /// </summary>
        //private void MapView_OnMapClick(object sender, MapClickMapViewEventArgs e)
        //{
        //    SimpleMarkerOverlay simpleMarkerOverlay = (SimpleMarkerOverlay)mapView.Overlays["simpleMarkerOverlay"];

        //    // Create a marker at the position the mouse was clicked
        //    var marker = new Marker(e.WorldLocation)
        //    {
        //        ImageSource = new BitmapImage(new Uri("/Resources/AQUA.png", UriKind.RelativeOrAbsolute)),
        //        Width = 20,
        //        Height = 34,
        //        YOffset = -17
        //    };

        //    // Add the marker to the simpleMarkerOverlay and refresh the map
        //    simpleMarkerOverlay.Markers.Add(marker);
        //    simpleMarkerOverlay.Refresh();
        //}

        /// <summary>
        /// Sets the simpleMarkerOverlay's drag mode to none, meaning that the markers cannot be moved or manipulated.
        /// </summary>
        private void StaticMode_OnClick(object sender, EventArgs e)
        {
            //SimpleMarkerOverlay simpleMarkerOverlay = (SimpleMarkerOverlay)mapView.Overlays["simpleMarkerOverlay"];
            //simpleMarkerOverlay.DragMode = MarkerDragMode.None;
        }

        /// <summary>
        /// Sets the simpleMarkerOverlay's drag mode to drag, which allows the user to click and drag on an icon to move it.
        /// </summary>
        private void DragMode_OnClick(object sender, EventArgs e)
        {
            //SimpleMarkerOverlay simpleMarkerOverlay = (SimpleMarkerOverlay)mapView.Overlays["simpleMarkerOverlay"];
            //simpleMarkerOverlay.DragMode = MarkerDragMode.Drag;
        }

        /// <summary>
        /// Sets the simpleMarkerOverlay's drag mode to copy, which allows the user to copy an existing marker by shift-clicking and dragging it.
        /// </summary>
        private void CopyMode_OnClick(object sender, EventArgs e)
        {
            //SimpleMarkerOverlay simpleMarkerOverlay = (SimpleMarkerOverlay)mapView.Overlays["simpleMarkerOverlay"];
            //simpleMarkerOverlay.DragMode = MarkerDragMode.CopyWithShiftKey;
        }
    }
}