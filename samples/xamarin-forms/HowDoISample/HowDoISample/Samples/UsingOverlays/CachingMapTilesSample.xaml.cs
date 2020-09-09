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
    /// <summary>
    /// Learn how to improve performance by locally caching map tiles
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CachingMapTilesSample : ContentPage
    {
        ThinkGeoCloudVectorMapsOverlay thinkGeoCloudVectorMapsOverlay;

        public CachingMapTilesSample()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Setup the map with the ThinkGeo Cloud Maps overlay to show a basic map
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            // Set the map's unit of measurement to meters(Spherical Mercator)
            mapView.MapUnit = GeographyUnit.Meter;            

            // Add Cloud Maps as a background overlay
            thinkGeoCloudVectorMapsOverlay = new ThinkGeoCloudVectorMapsOverlay("9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~", "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~", ThinkGeoCloudVectorMapsMapType.Light);
            mapView.Overlays.Add(thinkGeoCloudVectorMapsOverlay);

            // Set the map extent
            mapView.CurrentExtent = new RectangleShape(-10786436, 3918518, -10769429, 3906002);

            mapView.Refresh();
        }

        /// <summary>
        /// Toggles the use of a local tile cache
        /// </summary>
        private void UseCache_OnCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (useCache.IsChecked)
            {
                // Create a new tile cache on the Cloud Maps overlay. Cached images will be saved on the file system in the local application data directory.
                thinkGeoCloudVectorMapsOverlay.TileCache = new FileRasterTileCache(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "cache"), "CloudMapsImages", RasterTileFormat.Png);
            }
            else
            {
                // Remove the tile cache by setting it to null. Note that this does not remove the cached images on the file system.
                thinkGeoCloudVectorMapsOverlay.TileCache = null;
            }
        }
    }
}
