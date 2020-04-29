using Android.Content;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ThinkGeo.Core;
using ThinkGeo.UI.Android;

namespace GeometricFunctions
{
    public class SnapGPSSample : BaseSample
    {
        private InMemoryFeatureLayer inProcessLayer;
        private InMemoryFeatureLayer resultLayer;

        public SnapGPSSample(Context context, SliderView sliderView)
            : base(context, sliderView)
        { }

        protected override void Execute()
        {
            LayerOverlay layerOverlay = (LayerOverlay)MapView.Overlays["LayerOverlay"];
            resultLayer.InternalFeatures.Clear();
            if (resultLayer.InternalFeatures.Count == 0)
            {
                inProcessLayer.IsVisible = false;

                Feature multilineFeature = inProcessLayer.InternalFeatures[0];
                MultilineShape multilineShape = (MultilineShape)multilineFeature.GetShape();
                Feature[] allFeatures = { inProcessLayer.InternalFeatures[1] };
                double tolerance = 100;

                Collection<Feature> matchFeatures = new Collection<Feature>();
                foreach (var item in allFeatures)
                {
                    double tempDistance = multilineShape.GetShortestLineTo(item, GeographyUnit.Meter).GetLength(GeographyUnit.Meter, DistanceUnit.Feet);
                    if (tempDistance < tolerance)
                    {
                        matchFeatures.Add(item);
                    }
                }

                resultLayer.InternalFeatures.Add(multilineFeature);
                foreach (var feature in matchFeatures)
                {
                    Collection<Vertex> vertices = new Collection<Vertex>();
                    PointShape resultShape = multilineShape.GetClosestPointTo(feature, GeographyUnit.Meter);
                    MultilineShape tempMultilineShape = feature.GetShape() as MultilineShape;
                    if (tempMultilineShape != null)
                    {
                        double offsetX = resultShape.X - tempMultilineShape.Lines[0].Vertices[0].X;
                        double offsetY = resultShape.Y - tempMultilineShape.Lines[0].Vertices[0].Y;
                        vertices.Add(new Vertex(resultShape));

                        double x = offsetX + tempMultilineShape.Lines[0].Vertices[1].X;
                        double y = offsetY + tempMultilineShape.Lines[0].Vertices[1].Y;
                        vertices.Add(new Vertex(x, y));
                    }

                    resultLayer.InternalFeatures.Add(new Feature(new MultilineShape(new[] { new LineShape(vertices) })));
                }

                foreach (var feature in allFeatures)
                {
                    if (!matchFeatures.Contains(feature))
                    {
                        resultLayer.InternalFeatures.Add(feature);
                    }
                }

                CollectVertices(resultLayer.InternalFeatures.Skip(1).ToArray(), resultLayer);
            }

            layerOverlay.Refresh();
        }

        protected override void InitalizeMap()
        {
            MapView.MapUnit = GeographyUnit.Meter;
            MapView.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            MapView.CurrentExtent = GetBoundingBox();

            inProcessLayer = new InMemoryFeatureLayer();
            resultLayer = new InMemoryFeatureLayer();

            inProcessLayer.Open();
            inProcessLayer.Columns.Add(new FeatureSourceColumn("Name", "character", 100));

            resultLayer.Open();
            resultLayer.Columns.Add(new FeatureSourceColumn("Name", "character", 100));

            InitializeStyle();

            foreach (var feature in GeometrySource)
            {
                inProcessLayer.InternalFeatures.Add(feature);
            }

            CollectVertices(inProcessLayer.InternalFeatures.Skip(1).ToArray(), resultLayer);

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add("SourceLayer", inProcessLayer);
            layerOverlay.Layers.Add("PointLayer", resultLayer);
            MapView.Overlays.Add("LayerOverlay", layerOverlay);
        }

        private static void CollectVertices(IEnumerable<Feature> features, InMemoryFeatureLayer featureLayer)
        {
            lock (features)
            {
                foreach (var multilineShape in features.Select(f => f.GetShape()).OfType<MultilineShape>())
                {
                    foreach (var vertex in multilineShape.Lines.SelectMany(l => l.Vertices))
                    {
                        EllipseShape ellipseShape = new EllipseShape(new PointShape(vertex), 30);
                        featureLayer.InternalFeatures.Add(new Feature(ellipseShape));
                    }
                }
            }
        }

        private void InitializeStyle()
        {
            GeoColor semiTransparentOrange = GeoColor.FromArgb(140, 255, 155, 13);

            inProcessLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColors.Transparent, GeoColors.Black);
            inProcessLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = LineStyle.CreateSimpleLineStyle(semiTransparentOrange, 3, false);
            inProcessLayer.ZoomLevelSet.ZoomLevel01.DefaultTextStyle = TextStyle.CreateSimpleTextStyle("Name", "Arial", 9, DrawingFontStyles.Regular, GeoColors.Black);
            inProcessLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            resultLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = inProcessLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle;
            resultLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = LineStyle.CreateSimpleLineStyle(semiTransparentOrange, 4, false);
            resultLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
        }
    }
}