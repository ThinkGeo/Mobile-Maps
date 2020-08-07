using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinkGeo.Core;
using ThinkGeo.UI.Forms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.Xamarin.HowDoI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ESRIGridLayerSample : ContentPage
    {
        public ESRIGridLayerSample()
        {
            InitializeComponent();
        }

        /// <summary>
        /// ...
        /// </summary>
        private void MapView_Loaded(object sender, EventArgs e)
        {
            // It is important to set the map unit first to either feet, meters or decimal degrees.
            mapView.MapUnit = GeographyUnit.Meter;

            // Create background world map with vector tile requested from ThinkGeo Cloud Service. 
            ThinkGeoCloudVectorMapsOverlay thinkGeoCloudVectorMapsOverlay = new ThinkGeoCloudVectorMapsOverlay("itZGOI8oafZwmtxP-XGiMvfWJPPc-dX35DmESmLlQIU~", "bcaCzPpmOG6le2pUz5EAaEKYI-KSMny_WxEAe7gMNQgGeN9sqL12OA~~", ThinkGeoCloudVectorMapsMapType.Light);
            mapView.Overlays.Add(thinkGeoCloudVectorMapsOverlay);

            // Create a new overlay that will hold our new layer and add it to the map.
            LayerOverlay staticOverlay = new LayerOverlay();
            mapView.Overlays.Add(staticOverlay);

            // Create the new layer and set the projection as the data is in srid 2276 and our background is srid 3857 (spherical mercator).
            GridFeatureLayer gridFeatureLayer = new GridFeatureLayer(@"..\..\..\data\GridFile\Mosquitos.grd");
            gridFeatureLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

            // Add the layer to the overlay we created earlier.
            staticOverlay.Layers.Add("GridFeatureLayer", gridFeatureLayer);

            // Create a class break style based on the cell values and set area styles based on the values
            ClassBreakStyle gridClassBreakStyle = new ClassBreakStyle("CellValue");
            gridClassBreakStyle.ClassBreaks.Add(new ClassBreak(double.MinValue, new AreaStyle(new GeoSolidBrush(GeoColor.FromArgb(100, 0, 255, 0)))));
            gridClassBreakStyle.ClassBreaks.Add(new ClassBreak(12, new AreaStyle(new GeoSolidBrush(GeoColor.FromArgb(100, 128, 255, 128)))));
            gridClassBreakStyle.ClassBreaks.Add(new ClassBreak(24, new AreaStyle(new GeoSolidBrush(GeoColor.FromArgb(100, 224, 251, 132)))));
            gridClassBreakStyle.ClassBreaks.Add(new ClassBreak(36, new AreaStyle(new GeoSolidBrush(GeoColor.FromArgb(100, 225, 255, 0)))));
            gridClassBreakStyle.ClassBreaks.Add(new ClassBreak(48, new AreaStyle(new GeoSolidBrush(GeoColor.FromArgb(100, 245, 210, 10)))));
            gridClassBreakStyle.ClassBreaks.Add(new ClassBreak(60, new AreaStyle(new GeoSolidBrush(GeoColor.FromArgb(100, 255, 128, 0)))));
            gridClassBreakStyle.ClassBreaks.Add(new ClassBreak(72, new AreaStyle(new GeoSolidBrush(GeoColor.FromArgb(100, 255, 0, 0)))));
            gridClassBreakStyle.ClassBreaks.Add(new ClassBreak(double.MaxValue, new AreaStyle(new GeoSolidBrush(GeoColors.Transparent))));

            // Take the class break style we created above and set it on zoom level 1 and then apply it to all zoom levels up to 20.
            gridFeatureLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(gridClassBreakStyle);
            gridFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Open the layer and set the map view current extent to the bounding box of the layer.  
            gridFeatureLayer.Open();
            mapView.CurrentExtent = gridFeatureLayer.GetBoundingBox();

            // Refresh the map.
            mapView.Refresh();
        }
    }
}