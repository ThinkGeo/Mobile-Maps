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
    public partial class RotateShapeSample : ContentPage
    {
        public RotateShapeSample()
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
            mapView.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();

            // Add Cloud Maps as a background overlay
            var thinkGeoCloudVectorMapsOverlay = new ThinkGeoCloudVectorMapsOverlay("9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~", "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~", ThinkGeoCloudVectorMapsMapType.Light);
            mapView.Overlays.Add(thinkGeoCloudVectorMapsOverlay);

            ShapeFileFeatureLayer cityLimits = new ShapeFileFeatureLayer(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Data/Shapefile/FriscoCityLimits.shp"));
            InMemoryFeatureLayer rotatedLayer = new InMemoryFeatureLayer();
            LayerOverlay layerOverlay = new LayerOverlay();

            // Project cityLimits layer to Spherical Mercator to match the map projection
            cityLimits.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

            // Style cityLimits layer
            cityLimits.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(new GeoColor(32, GeoColors.Orange), GeoColors.DimGray);
            cityLimits.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Style rotatedLayer
            rotatedLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(new GeoColor(32, GeoColors.Green), GeoColors.DimGray);
            rotatedLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Add cityLimits layer to a LayerOverlay
            layerOverlay.Layers.Add("cityLimits", cityLimits);

            // Add rotatedLayer to the layerOverlay
            layerOverlay.Layers.Add("rotatedLayer", rotatedLayer);

            // Set the map extent to the cityLimits layer bounding box
            cityLimits.Open();
            mapView.CurrentExtent = cityLimits.GetBoundingBox();
            cityLimits.Close();

            // Add LayerOverlay to Map
            mapView.Overlays.Add("layerOverlay", layerOverlay);
        }

        private void RotateShape_OnClick(object sender, EventArgs e)
        {
            LayerOverlay layerOverlay = (LayerOverlay)mapView.Overlays["layerOverlay"];

            ShapeFileFeatureLayer cityLimits = (ShapeFileFeatureLayer)layerOverlay.Layers["cityLimits"];
            InMemoryFeatureLayer rotatedLayer = (InMemoryFeatureLayer)layerOverlay.Layers["rotatedLayer"];

            // Query the cityLimits layer to get all the features
            cityLimits.Open();
            var features = cityLimits.QueryTools.GetAllFeatures(ReturningColumnsType.NoColumns);
            cityLimits.Close();

            // Rotate the first feature using it's center point as the anchor
            var rotate = AreaBaseShape.Rotate(features[0], features[0].GetShape().GetCenterPoint(), Convert.ToSingle(rotateDegree.Text));

            // Add the rotated shape into rotatedLayer to display the result.
            // If this were to be a permanent change to the cityLimits FeatureSource, you would modify the underlying data using BeginTransaction and CommitTransaction instead.
            rotatedLayer.InternalFeatures.Clear();
            rotatedLayer.InternalFeatures.Add(new Feature(rotate));

            // Redraw the layerOverlay to see the rotated feature on the map
            layerOverlay.Refresh();
        }
    }
}
