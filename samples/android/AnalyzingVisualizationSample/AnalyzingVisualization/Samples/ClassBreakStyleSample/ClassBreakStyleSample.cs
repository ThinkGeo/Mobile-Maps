using Android.App;
using Android.Content;
using Android.Widget;
using System.Collections.ObjectModel;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Android;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;

namespace AnalyzingVisualization
{
    public class ClassBreakStyleSample : BaseSample
    {
        private ClassBreakStyle classBreakStyle;

        public ClassBreakStyleSample(Context context)
            : base(context)
        {
        }

        protected override void InitializeMap()
        {
            MapView.CurrentExtent = new RectangleShape(-14299615, 20037508, -7255178, -1015546);

            ShapeFileFeatureLayer usLayer = new ShapeFileFeatureLayer(SampleHelper.GetDataPath("usStatesCensus2010.shp"));
            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(usLayer);
            MapView.Overlays.Add(layerOverlay);

            classBreakStyle = GetClassBreakStyle();
            usLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Clear();
            usLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(classBreakStyle);
            usLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
        }

        protected override void UpdateSampleLayoutCore()
        {
            FrameLayout mapContainerView = SampleView.FindViewById<FrameLayout>(Resource.Id.MapContainerView);
            ClassBreakChartView classBreakChartView = new ClassBreakChartView(Application.Context, classBreakStyle);
            mapContainerView.AddView(classBreakChartView);
        }

        private static ClassBreakStyle GetClassBreakStyle()
        {
            double[] classBreakValues = { 0, 814180.0, 1328361.0, 2059179.0, 2967297.0, 4339367.0, 5303925.0, 6392017.0, 8791894.0 };

            GeoColor outlineColor = GeoColor.FromHtml("#f05133");
            GeoColor fromColor = GeoColor.FromArgb(255, 116, 160, 255);
            GeoColor toColor = GeoColor.FromArgb(255, 220, 52, 56);
            Collection<GeoColor> familyColors = GeoColor.GetColorsInQualityFamily(fromColor, toColor, 10, ColorWheelDirection.CounterClockwise);
            ClassBreakStyle style = new ClassBreakStyle("Population", BreakValueInclusion.IncludeValue);
            for (int i = 0; i < classBreakValues.Length; i++)
            {
                style.ClassBreaks.Add(new ClassBreak(classBreakValues[i], AreaStyles.CreateSimpleAreaStyle(new GeoColor(200, familyColors[i]), outlineColor, 1)));
            }

            return style;
        }
    }
}