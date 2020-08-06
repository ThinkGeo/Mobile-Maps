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
    public partial class CachingMapTilesSample : ContentPage
    {
        public CachingMapTilesSample()
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
            //thinkGeoCloudVectorMapsOverlay = new ThinkGeoCloudVectorMapsOverlay("9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~", "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~", ThinkGeoCloudVectorMapsMapType.Light);
            //mapView.Overlays.Add(thinkGeoCloudVectorMapsOverlay);

            //// Set the map extent
            //mapView.CurrentExtent = new RectangleShape(-10786436, 3918518, -10769429, 3906002);
        }


        /// <summary>
        /// Create a new tile cache on the Cloud Maps overlay. Cached images will be saved on the file system in the bin folder.
        /// </summary>
        private void UseCache_Checked(object sender, EventArgs e)
        {
            //thinkGeoCloudVectorMapsOverlay.TileCache = new FileRasterTileCache("cache", "CloudMapsImages", RasterTileFormat.Png);
        }

        /// <summary>
        /// Remove the tile cache by setting it to null. Note that this does not remove the cached images on the file system.
        /// </summary>
        private void UseCache_Unchecked(object sender, EventArgs e)
        {
           // thinkGeoCloudVectorMapsOverlay.TileCache = null;
        }
    }
}