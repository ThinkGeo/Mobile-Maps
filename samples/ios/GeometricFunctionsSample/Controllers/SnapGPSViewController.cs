using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ThinkGeo.Core;
using ThinkGeo.UI.iOS;

namespace GeometricFunctions
{
    public class SnapGPSViewController : DetailViewController
    {
        private InMemoryFeatureLayer resultLayer;
        private InMemoryFeatureLayer inProcessLayer;

        public SnapGPSViewController()
        { }

        protected override void InitializeMap()
        {
            MapView.MapUnit = GeographyUnit.Meter;
            MapView.CurrentExtent = GetBoundingBox();

            InitializeLayers();

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.TileWidth = 512;
            layerOverlay.TileHeight = 512;
            layerOverlay.TileType = TileType.SingleTile;
            layerOverlay.Layers.Add("SourceLayer", inProcessLayer);
            layerOverlay.Layers.Add("PointLayer", resultLayer);
            MapView.Overlays.Add("LayerOverlay", layerOverlay);

            MapView.Refresh();
        }

        protected override void Execute()
        {
            LayerOverlay layerOverlay = (LayerOverlay)MapView.Overlays["LayerOverlay"];
            resultLayer.InternalFeatures.Clear();
            if (resultLayer.InternalFeatures.Count == 0)
            {
                inProcessLayer.IsVisible = false;

                MultilineShape multilineShape = inProcessLayer.InternalFeatures[0].GetShape() as MultilineShape;

                Feature[] allFeatures = { inProcessLayer.InternalFeatures[1] };

                Collection<Feature> matchFeatures = new Collection<Feature>();
                double tolerance = 100;

                foreach (var item in allFeatures)
                {
                    double tempDistance = multilineShape.GetShortestLineTo(item, GeographyUnit.Meter).GetLength(GeographyUnit.Meter, DistanceUnit.Feet);
                    if (tempDistance < tolerance)
                    {
                        matchFeatures.Add(item);
                    }
                }

                resultLayer.InternalFeatures.Add(new Feature(multilineShape));
                foreach (var feature in matchFeatures)
                {
                    Collection<Vertex> vertices = new Collection<Vertex>();

                    PointShape resultShape = multilineShape.GetClosestPointTo(feature, GeographyUnit.Meter);
                    MultilineShape tempMultilineShape = feature.GetShape() as MultilineShape;
                    if (tempMultilineShape != null)
                    {
                        vertices.Add(new Vertex(resultShape));

                        double offsetX = resultShape.X - tempMultilineShape.Lines[0].Vertices[0].X;
                        double offsetY = resultShape.Y - tempMultilineShape.Lines[0].Vertices[0].Y;
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

        private void InitializeLayers()
        {
            inProcessLayer = new InMemoryFeatureLayer();
            inProcessLayer.Open();
            inProcessLayer.Columns.Add(new FeatureSourceColumn("Name", "character", 100));

            resultLayer = new InMemoryFeatureLayer();
            resultLayer.Open();
            resultLayer.Columns.Add(new FeatureSourceColumn("Name", "character", 100));

            GeoColor semiTransparentOrange = GeoColor.FromArgb(140, 255, 155, 13);
            inProcessLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColors.Transparent, GeoColors.Black);
            inProcessLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = LineStyle.CreateSimpleLineStyle(semiTransparentOrange, 3, false);
            inProcessLayer.ZoomLevelSet.ZoomLevel01.DefaultTextStyle = TextStyle.CreateSimpleTextStyle("Name", "Arial", 9, DrawingFontStyles.Regular, GeoColors.Black);
            inProcessLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            foreach (var feature in GeometrySource)
            {
                inProcessLayer.InternalFeatures.Add(feature);
            }

            resultLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = inProcessLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle;
            resultLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = LineStyle.CreateSimpleLineStyle(semiTransparentOrange, 4, false);
            resultLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            CollectVertices(inProcessLayer.InternalFeatures.Skip(1).ToArray(), resultLayer);
        }

        private static void CollectVertices(IEnumerable<Feature> features, InMemoryFeatureLayer targetFeatureLayer)
        {
            foreach (var multilineShape in features.Select(f => f.GetShape()).OfType<MultilineShape>().Where(s => s != null))
            {
                foreach (var vertex in multilineShape.Lines.SelectMany(l => l.Vertices))
                {
                    EllipseShape ellipseShape = new EllipseShape(new PointShape(vertex), 30);
                    targetFeatureLayer.InternalFeatures.Add(new Feature(ellipseShape));
                }
            }
        }
    }
}