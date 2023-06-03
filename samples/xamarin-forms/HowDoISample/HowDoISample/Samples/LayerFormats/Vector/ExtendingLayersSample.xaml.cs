using System;
using System.Collections.ObjectModel;
using ThinkGeo.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ExtendingLayersSample : ContentPage
    {
        public ExtendingLayersSample()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     ...
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            // Set the map's unit of measurement to meters(Spherical Mercator)
            mapView.MapUnit = GeographyUnit.Meter;

            // Add Cloud Maps as a background overlay
            var backgroundOverlay = new ThinkGeoCloudVectorMapsOverlay(
                "9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~",
                "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~", ThinkGeoCloudVectorMapsMapType.Light);
            backgroundOverlay.TileCache = new FileRasterTileCache(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ThinkGeoLightBackground");
            mapView.Overlays.Add(backgroundOverlay);

            var layerOverlay = new LayerOverlay();
            layerOverlay.TileType = TileType.SingleTile;
            mapView.Overlays.Add(layerOverlay);

            var radiusLayer = new RadiusLayer();
            radiusLayer.RingDistanceUnit = DistanceUnit.Mile;
            radiusLayer.RingGeography = GeographyUnit.Meter;
            radiusLayer.RingDistance = 5;

            //layerOverlay.Layers.Add(radiusLayer);
            mapView.AdornmentOverlay.Layers.Add(radiusLayer);
            mapView.CurrentExtent =
                new RectangleShape(-10812042.5236828, 3942445.36497713, -10748599.7905585, 3887792.89005685);
        }

        public void Dispose()
        {
            // Dispose of unmanaged resources.
            mapView.Dispose();
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }
    }

    // This layer overrides the DrawCore and draws circles every x miles based on the center point
    // of the screen.  You notice in the DrawCore we can draw directly on the canvas which gives us
    // allot of power.  This is similar to custom styles where we can also draw directly on the canvas
    // from the style.
    public class RadiusLayer : AdornmentLayer
    {
        public RadiusLayer()
        {
            RingDistanceUnit = DistanceUnit.Mile;
            RingGeography = GeographyUnit.Meter;
            RingDistance = 1;
            RingAreaStyle = new AreaStyle(new GeoPen(new GeoSolidBrush(GeoColor.FromArgb(50, GeoColors.Blue)), 4));
        }

        public double RingDistance { get; set; }

        public DistanceUnit RingDistanceUnit { get; set; }

        public GeographyUnit RingGeography { get; set; }

        public AreaStyle RingAreaStyle { get; set; }

        protected override void DrawCore(GeoCanvas canvas, Collection<SimpleCandidate> labelsInAllLayers)
        {
            var centerPoint = canvas.CurrentWorldExtent.GetCenterPoint();

            var currentRingDistance = RingDistance;
            MultipolygonShape circle = null;

            // Keep drawing rings until the only barley fit inside the current extent.
            do
            {
                circle = centerPoint.Buffer(currentRingDistance, RingGeography, RingDistanceUnit);

                canvas.DrawArea(circle, RingAreaStyle.OutlinePen, RingAreaStyle.FillBrush, DrawingLevel.LevelOne);
                currentRingDistance += RingDistance;
            } while (canvas.CurrentWorldExtent.Contains(circle));
        }
    }
}