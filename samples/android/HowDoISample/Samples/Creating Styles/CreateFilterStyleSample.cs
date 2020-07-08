using System;
using System.Collections.ObjectModel;
using Android.OS;
using Android.Widget;
using ThinkGeo.Core;

namespace ThinkGeo.UI.Android.HowDoI
{
    /// <summary>
    /// Learn how to selectively style features using a FilterStyle
    /// </summary>
    public class CreateFilterStyleSample : SampleFragment
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
            mapView.CurrentExtent = new RectangleShape(-10780196.9469504, 3916119.49665258, -10776231.7761301, 3912703.71697007);

            ShapeFileFeatureLayer friscoCrimeLayer = new ShapeFileFeatureLayer(@"mnt/sdcard/MapSuiteSampleData/HowDoISamples/AppData/SampleData/Shapefile/Frisco_Crime.shp");

            // Project the layer's data to match the projection of the map
            friscoCrimeLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

            // Add friscoCrimeLayer to a LayerOverlay
            var layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(friscoCrimeLayer);

            AddFilterStyle(friscoCrimeLayer);

            // Add layerOverlay to the mapView
            mapView.Overlays.Add(layerOverlay);
        }

        /// <summary>
        /// Adds a filter style to various categories of the Frisco Crime layer
        /// </summary>
        private void AddFilterStyle(ShapeFileFeatureLayer layer)
        {
            // Create a filter style based on the "Drugs" Offense Group 
            var drugFilterStyle = new FilterStyle()
            {
                Conditions = { new FilterCondition("OffenseGro", "Drugs") },
                Styles = {
                    new PointStyle(PointSymbolType.Circle, 28, GeoBrushes.White,GeoPens.Red),
                    new PointStyle(new GeoImage(@"../../../Resources/drugs_icon.png")) { ImageScale = .60 }
                }
            };

            // Create a filter style based on the "Weapons" Offense Group 
            var weaponFilterStyle = new FilterStyle()
            {
                Conditions = { new FilterCondition("OffenseGro", "Weapons") },
                Styles = {
                    new PointStyle(PointSymbolType.Circle, 28, GeoBrushes.White,GeoPens.Red),
                    new PointStyle(new GeoImage(@"../../../Resources/weapon_icon.png")) { ImageScale = .25 }
                }
            };

            // Create a filter style based on the "Vandalism" Offense Group 
            var vandalismFilterStyle = new FilterStyle()
            {
                Conditions = { new FilterCondition("OffenseGro", "Vandalism") },
                Styles = {
                    new PointStyle(PointSymbolType.Circle, 28, GeoBrushes.White,GeoPens.Red),
                    new PointStyle(new GeoImage(@"../../../Resources/vandalism_icon.png")) { ImageScale = .25 }
                }
            };

            // Add the filter styles to the CustomStyles collection
            layer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(drugFilterStyle);
            layer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(weaponFilterStyle);
            layer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(vandalismFilterStyle);
            layer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
        }
    }
}