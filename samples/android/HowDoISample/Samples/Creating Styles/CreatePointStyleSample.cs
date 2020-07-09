using System;
using System.Collections.ObjectModel;
using Android.OS;
using Android.Views;
using Android.Widget;
using ThinkGeo.Core;

namespace ThinkGeo.UI.Android.HowDoI
{
    /// <summary>
    /// Learn how to style point data using a PointStyle
    /// </summary>
    public class CreatePointStyleSample : SampleFragment
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

            RadioButton symbolButton = new RadioButton(this.Context);
            symbolButton.Text = "Symbols";
            symbolButton.Click += SymbolButton_Click;

            RadioButton iconButton = new RadioButton(this.Context);
            iconButton.Text = "Icons";
            iconButton.Click += IconButton_Click;

            RadioButton textButton = new RadioButton(this.Context);
            textButton.Text = "Text";
            textButton.Click += TextButton_Click;

            RadioGroup radioGroup = new RadioGroup(this.Context);
            radioGroup.AddView(symbolButton);
            radioGroup.AddView(iconButton);
            radioGroup.AddView(textButton);

            LinearLayout linearLayout = new LinearLayout(this.Context);
            linearLayout.Orientation = Orientation.Horizontal;

            linearLayout.AddView(radioGroup);

            SampleViewHelper.InitializeInstruction(this.Context, currentView.FindViewById<RelativeLayout>(Resource.Id.MainLayout), this.SampleInfo, new Collection<View>() { linearLayout });
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
            mapView.CurrentExtent = new RectangleShape(-10778329.017082, 3909598.36751101, -10776250.8853871, 3907890.47766975);

            ShapeFileFeatureLayer hotelsLayer = new ShapeFileFeatureLayer(@"mnt/sdcard/MapSuiteSampleData/HowDoISamples/AppData/SampleData/Shapefile/Hotels.shp");
            LayerOverlay layerOverlay = new LayerOverlay();

            // Project the layer's data to match the projection of the map
            hotelsLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

            // Add the layer to a layer overlay
            layerOverlay.Layers.Add("hotels", hotelsLayer);

            // Add the overlay to the map
            mapView.Overlays.Add("layerOverlay", layerOverlay);
        }

        /// <summary>
        /// Create a pointStyle using a PointSymbol and add it to the Hotels layer
        /// </summary>
        private void SymbolButton_Click(object sender, EventArgs e)
        {
            if (mapView.Overlays.Count > 0)
            {
                LayerOverlay layerOverlay = (LayerOverlay)mapView.Overlays["layerOverlay"];
                ShapeFileFeatureLayer hotelsLayer = (ShapeFileFeatureLayer)layerOverlay.Layers["hotels"];

                // Create a point style
                var pointStyle = new PointStyle(PointSymbolType.Circle, 12, GeoBrushes.Blue, new GeoPen(GeoBrushes.White, 2));

                // Add the point style to the collection of custom styles for ZoomLevel 1.
                hotelsLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Clear();
                hotelsLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(pointStyle);

                // Apply the styles for ZoomLevel 1 down to ZoomLevel 20. This effectively applies the point style on every zoom level on the map. 
                hotelsLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

                // Refresh the layerOverlay to show the new style
                layerOverlay.Refresh();
            }
        }

        /// <summary>
        /// Create a pointStyle using an icon image and add it to the Hotels layer
        /// </summary>
        private void IconButton_Click(object sender, EventArgs e)
        {
            LayerOverlay layerOverlay = (LayerOverlay)mapView.Overlays["layerOverlay"];
            ShapeFileFeatureLayer hotelsLayer = (ShapeFileFeatureLayer)layerOverlay.Layers["hotels"];

            // Create a point style
            var pointStyle = new PointStyle(new GeoImage(@"mnt/sdcard/MapSuiteSampleData/HowDoISamples/AppData/Resources/hotel_icon.png"))
            {
                ImageScale = .25
            };

            // Add the point style to the collection of custom styles for ZoomLevel 1.
            hotelsLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Clear();
            hotelsLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(pointStyle);

            // Apply the styles for ZoomLevel 1 down to ZoomLevel 20. This effectively applies the point style on every zoom level on the map. 
            hotelsLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Refresh the layerOverlay to show the new style
            layerOverlay.Refresh();
        }


        /// <summary>
        /// Create a pointStyle using a font symbol and add it to the Hotels layer
        /// </summary>
        private void TextButton_Click(object sender, EventArgs e)
        {
            LayerOverlay layerOverlay = (LayerOverlay)mapView.Overlays["layerOverlay"];
            ShapeFileFeatureLayer hotelsLayer = (ShapeFileFeatureLayer)layerOverlay.Layers["hotels"];

            // Create a point style
            var symbolPointStyle = new PointStyle(new GeoFont("Verdana", 16, DrawingFontStyles.Bold), "@", GeoBrushes.Black)
            {
                Mask = new AreaStyle(GeoBrushes.White),
                MaskType = MaskType.Circle
            };

            // Add the point style to the collection of custom styles for ZoomLevel 1.
            hotelsLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Clear();
            hotelsLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(symbolPointStyle);

            // Apply the styles for ZoomLevel 1 down to ZoomLevel 20. This effectively applies the point style on every zoom level on the map. 
            hotelsLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Refresh the layerOverlay to show the new style
            layerOverlay.Refresh();
        }
    }
}