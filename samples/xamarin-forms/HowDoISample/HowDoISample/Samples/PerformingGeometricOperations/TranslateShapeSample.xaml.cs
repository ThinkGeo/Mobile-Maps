using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinkGeo.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HowDoISample.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TranslateShapeSample : ContentPage
    {
        public TranslateShapeSample()
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
            //ShapeFileFeatureLayer.BuildIndexFile(@"../../../Data/Shapefile/FriscoCityLimits.shp");

            //// Add Cloud Maps as a background overlay
            //var thinkGeoCloudVectorMapsOverlay = new ThinkGeoCloudVectorMapsOverlay("itZGOI8oafZwmtxP-XGiMvfWJPPc-dX35DmESmLlQIU~", "bcaCzPpmOG6le2pUz5EAaEKYI-KSMny_WxEAe7gMNQgGeN9sqL12OA~~", ThinkGeoCloudVectorMapsMapType.Light);
            //mapView.Overlays.Add(thinkGeoCloudVectorMapsOverlay);

            //ShapeFileFeatureLayer cityLimits = new ShapeFileFeatureLayer(@"../../../Data/Shapefile/FriscoCityLimits.shp");
            //InMemoryFeatureLayer translatedLayer = new InMemoryFeatureLayer();
            //LayerOverlay layerOverlay = new LayerOverlay();

            //// Project cityLimits layer to Spherical Mercator to match the map projection
            //cityLimits.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

            //// Style cityLimits layer
            //cityLimits.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(new GeoColor(32, GeoColors.Orange), GeoColors.DimGray);
            //cityLimits.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            //// Style translatedLayer
            //translatedLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(new GeoColor(32, GeoColors.Green), GeoColors.DimGray);
            //translatedLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            //// Add cityLimits layer to a LayerOverlay
            //layerOverlay.Layers.Add("cityLimits", cityLimits);

            //// Add translatedLayer to the layerOverlay
            //layerOverlay.Layers.Add("translatedLayer", translatedLayer);

            //// Set the map extent to the cityLimits layer bounding box
            //cityLimits.Open();
            //mapView.CurrentExtent = cityLimits.GetBoundingBox();
            //cityLimits.Close();

            //// Add LayerOverlay to Map
            //mapView.Overlays.Add("layerOverlay", layerOverlay);
        }

        private void OffsetTranslateShape_OnClick(object sender, EventArgs e)
        {
            //LayerOverlay layerOverlay = (LayerOverlay)mapView.Overlays["layerOverlay"];

            //ShapeFileFeatureLayer cityLimits = (ShapeFileFeatureLayer)layerOverlay.Layers["cityLimits"];
            //InMemoryFeatureLayer translatedLayer = (InMemoryFeatureLayer)layerOverlay.Layers["translatedLayer"];

            //// Query the cityLimits layer to get all the features
            //cityLimits.Open();
            //var features = cityLimits.QueryTools.GetAllFeatures(ReturningColumnsType.NoColumns);
            //cityLimits.Close();

            //// Translate the first feature's shape by the X and Y values on the UI in meters
            //var translate = AreaBaseShape.TranslateByOffset(features[0].GetShape(), Convert.ToDouble(translateX.Text), Convert.ToDouble(translateY.Text), GeographyUnit.Meter, DistanceUnit.Meter);

            //// Add the translated shape into translatedLayer to display the result.
            //// If this were to be a permanent change to the cityLimits FeatureSource, you would modify the
            //// underlying data using BeginTransaction and CommitTransaction instead.
            //translatedLayer.InternalFeatures.Clear();
            //translatedLayer.InternalFeatures.Add(new Feature(translate));

            //// Redraw the layerOverlay to see the translated feature on the map
            //layerOverlay.Refresh();
        }

        private void DegreeTranslateShape_OnClick(object sender, EventArgs e)
        {
            //LayerOverlay layerOverlay = (LayerOverlay)mapView.Overlays["layerOverlay"];

            //ShapeFileFeatureLayer cityLimits = (ShapeFileFeatureLayer)layerOverlay.Layers["cityLimits"];
            //InMemoryFeatureLayer translatedLayer = (InMemoryFeatureLayer)layerOverlay.Layers["translatedLayer"];

            //// Query the cityLimits layer to get all the features
            //cityLimits.Open();
            //var features = cityLimits.QueryTools.GetAllFeatures(ReturningColumnsType.NoColumns);
            //cityLimits.Close();

            //// Translate the first feature's shape by the X and Y values on the UI in meters
            //var translate = AreaBaseShape.TranslateByDegree(features[0].GetShape(), Convert.ToDouble(translateDistance.Text), Convert.ToDouble(translateAngle.Text), GeographyUnit.Meter, DistanceUnit.Meter);

            //// Add the translated shape into translatedLayer to display the result.
            //// If this were to be a permanent change to the cityLimits FeatureSource, you would modify the
            //// underlying data using BeginTransaction and CommitTransaction instead.
            //translatedLayer.InternalFeatures.Clear();
            //translatedLayer.InternalFeatures.Add(new Feature(translate));

            //// Redraw the layerOverlay to see the translated feature on the map
            //layerOverlay.Refresh();
        }
    }
}