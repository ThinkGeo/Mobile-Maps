﻿using System;
using System.Collections.ObjectModel;
using Android.OS;
using Android.Views;
using Android.Widget;
using ThinkGeo.Core;

namespace ThinkGeo.UI.Android.HowDoI
{
    /// <summary>
    /// This sample shows how to display a WMS layer. (This sample may be slow as we use a public WMS sever).
    /// </summary>
    public class WMSLayerSample : SampleFragment
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

            RadioButton overlayButton = new RadioButton(this.Context);
            overlayButton.Text = "Use WmsOverlay";
            overlayButton.Click += rbLayerOverlay_Checked;
            overlayButton.Checked = true;

            RadioButton rasterLayerButton = new RadioButton(this.Context);
            rasterLayerButton.Text = "Use WmsRasterLayer";
            rasterLayerButton.Click += rbLayerOverlay_Checked;

            RadioGroup radioGroup = new RadioGroup(this.Context);
            radioGroup.AddView(overlayButton);
            radioGroup.AddView(rasterLayerButton);

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
            // It is important to set the map unit first to either feet, meters or decimal degrees.
            mapView.MapUnit = GeographyUnit.DecimalDegree;

            // This code sets up the sample to use the overlay versus the layer.
            UseOverlay();

            // Set the current extent to a local area.
            mapView.CurrentExtent = new RectangleShape(-96.8538765269409, 33.1618647290098, -96.7987487018851, 33.1054126590461);

            // Refresh the map.
            mapView.Refresh();
        }

        private void rbLayerOverlay_Checked(object sender, EventArgs e)
        {
            // Based on the radio buttons we switch between using the overlay and layer.
            RadioButton button = (RadioButton)sender;
            if (button.Text != null)
            {
                switch (button.Text)
                {
                    case "Use WmsOverlay":
                        UseOverlay();
                        break;
                    case "Use WmsRasterLayer":
                        UseLayer();
                        break;
                    default:
                        break;
                }
                mapView.Refresh();
            }
        }
        private void UseOverlay()
        {
            // Clear out the overlays so we start fresh
            mapView.Overlays.Clear();

            // Create a WMS overlay using the WMS parameters below.
            // This is a public service and is very slow most of the time.
            WmsOverlay wmsOverlay = new WmsOverlay(new Uri("http://ows.mundialis.de/services/service"));
            wmsOverlay.Parameters.Add("layers", "OSM-WMS");
            wmsOverlay.Parameters.Add("STYLES", "default");

            // Add the overlay to the map.
            mapView.Overlays.Add(wmsOverlay);
        }

        private void UseLayer()
        {
            // Clear out the overlays so we start fresh
            mapView.Overlays.Clear();

            // Create an overlay that we will add the layer to.
            LayerOverlay staticOverlay = new LayerOverlay();
            mapView.Overlays.Add(staticOverlay);

            // Create the WMS layer using the parameters below.
            // This is a public service and is very slow most of the time.
            WmsRasterLayer wmsImageLayer = new WmsRasterLayer(new Uri("http://ows.mundialis.de/services/service"));
            wmsImageLayer.UpperThreshold = double.MaxValue;
            wmsImageLayer.LowerThreshold = 0;
            wmsImageLayer.ActiveLayerNames.Add("OSM-WMS");
            wmsImageLayer.ActiveStyleNames.Add("default");
            wmsImageLayer.Exceptions = "application/vnd.ogc.se_xml";

            // Add the layer to the overlay.
            staticOverlay.Layers.Add("wmsImageLayer", wmsImageLayer);
        }
    }
}