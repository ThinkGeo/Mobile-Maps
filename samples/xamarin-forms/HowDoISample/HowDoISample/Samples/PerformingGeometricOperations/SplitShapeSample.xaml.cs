﻿using System;
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
    public partial class SplitShapeSample : ContentPage
    {
        public SplitShapeSample()
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

            ShapeFileFeatureLayer cityLimits = new ShapeFileFeatureLayer(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Data/Shapefile/FriscoCityLimits.shp"));
            ShapeFileFeatureLayer adminBoundaries = new ShapeFileFeatureLayer(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Data/FriscoMunBnd/FriscoAdminBoundaries.shp"));
            InMemoryFeatureLayer splitLayer = new InMemoryFeatureLayer();
            LayerOverlay layerOverlay = new LayerOverlay();

            // Project cityLimits layer to Spherical Mercator to match the map projection
            cityLimits.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

            // Project adminBoundaries layer to Spherical Mercator to match the map projection
            adminBoundaries.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

            // Style cityLimits layer
            cityLimits.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(new GeoColor(32, GeoColors.Orange), GeoColors.DimGray);
            cityLimits.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Style adminBoundaries layer
            adminBoundaries.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = LineStyle.CreateSimpleLineStyle(GeoColors.Red, 2, false);
            adminBoundaries.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Style the splitLayer
            splitLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(new GeoColor(32, GeoColors.Green), GeoColors.DimGray);
            splitLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Add cityLimits to a LayerOverlay
            layerOverlay.Layers.Add("cityLimits", cityLimits);

            // Add splitLayer to the layerOverlay
            layerOverlay.Layers.Add("splitLayer", splitLayer);

            // Set the map extent to the cityLimits layer bounding box
            cityLimits.Open();
            mapView.CurrentExtent = cityLimits.GetBoundingBox();
            cityLimits.Close();

            // Add LayerOverlay to Map
            mapView.Overlays.Add("layerOverlay", layerOverlay);

            mapView.Refresh();
        }

        private void SplitShape_OnClick(object sender, EventArgs e)
        {
            LayerOverlay layerOverlay = (LayerOverlay)mapView.Overlays["layerOverlay"];

            ShapeFileFeatureLayer cityLimits = (ShapeFileFeatureLayer)layerOverlay.Layers["cityLimits"];
            InMemoryFeatureLayer splitLayer = (InMemoryFeatureLayer)layerOverlay.Layers["splitLayer"];

            // Query the cityLimits layer to get all the features
            cityLimits.Open();
            var feature = cityLimits.QueryTools.GetAllFeatures(ReturningColumnsType.NoColumns).First();
            cityLimits.Close();

            // Split the polygon using a line that crosses through it
            // TODO: We do not have an API for splitting a polygon using a line shape. This sample will be more involved than normal
            var split = feature;

            // Add the split shape into an InMemoryFeatureLayer to display the result.
            // If this were to be a permanent change to the cityLimits FeatureSource, you would modify the underlying data using BeginTransaction and CommitTransaction instead.
            splitLayer.InternalFeatures.Clear();
            splitLayer.InternalFeatures.Add(split);

            // Redraw the layerOverlay to see the split features on the map
            layerOverlay.Refresh();
        }
    }
}
