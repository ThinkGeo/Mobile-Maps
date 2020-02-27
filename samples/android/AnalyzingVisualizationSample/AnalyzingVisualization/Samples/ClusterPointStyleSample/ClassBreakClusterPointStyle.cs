using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;

namespace AnalyzingVisualization
{
    [Serializable]
    public class ClassBreakClusterPointStyle : Style
    {
        [Obfuscation(Exclude = true)]
        private Dictionary<int, PointStyle> classBreakPoint;

        [Obfuscation(Exclude = true)]
        private int cellSize = 100;

        [Obfuscation(Exclude = true)]
        private TextStyle textSytle = new TextStyle();

        public ClassBreakClusterPointStyle()
            : base()
        {
            classBreakPoint = new Dictionary<int, PointStyle>();
        }

        public Dictionary<int, PointStyle> ClassBreakPoint
        {
            get { return classBreakPoint; }
        }

        public TextStyle TextStyle
        {
            get { return textSytle; }
            set { textSytle = value; }
        }

        public int CellSize
        {
            get { return cellSize; }
            set { cellSize = value; }
        }

        protected override void DrawCore(IEnumerable<Feature> features, GeoCanvas canvas, Collection<SimpleCandidate> labelsInThisLayer, Collection<SimpleCandidate> labelsInAllLayers)
        {
            double scale = ExtentHelper.GetScale(canvas.CurrentWorldExtent, canvas.Width, canvas.MapUnit);
            MapSuiteTileMatrix mapSuiteTileMatrix = new MapSuiteTileMatrix(scale, cellSize, cellSize, canvas.MapUnit);
            IEnumerable<TileMatrixCell> tileMatricCells = mapSuiteTileMatrix.GetContainedCells(canvas.CurrentWorldExtent);
            Dictionary<string, string> unusedFeatures = new Dictionary<string, string>();

            foreach (Feature feature in features)
            {
                if (feature.GetWellKnownType() != WellKnownType.Point && feature.GetWellKnownType() != WellKnownType.Multipoint)
                {
                    continue;
                }
                unusedFeatures.Add(feature.Id, feature.Id);
            }

            foreach (TileMatrixCell cell in tileMatricCells)
            {
                int featureCount = 0;
                MultipointShape tempMultiPointShape = new MultipointShape();
                foreach (Feature feature in features)
                {
                    // Make sure the feature has not been used in another cluster
                    if (unusedFeatures.ContainsKey(feature.Id))
                    {
                        // Check if the cell contains the feature
                        if (cell.BoundingBox.Contains(feature.GetBoundingBox()))
                        {
                            featureCount++;
                            unusedFeatures.Remove(feature.Id);
                            if (feature.GetWellKnownType() == WellKnownType.Multipoint)
                            {
                                MultipointShape multipointShape = feature.GetShape() as MultipointShape;
                                foreach (var item in multipointShape.Points)
                                {
                                    tempMultiPointShape.Points.Add(item);
                                }
                            }
                            else
                            {
                                tempMultiPointShape.Points.Add(feature.GetShape() as PointShape);
                            }
                        }
                    }
                }
                if (featureCount > 0)
                {
                    // Add the feature count to the new feature we created.  The feature will be placed
                    // at the center of gravity of all the clustered features of the cell we created.
                    Dictionary<string, string> featureValues = new Dictionary<string, string>();
                    featureValues.Add("FeatureCount", featureCount.ToString(CultureInfo.InvariantCulture));

                    bool isMatch = false;

                    for (int i = 0; i < classBreakPoint.Count - 1; i++)
                    {
                        var startItem = classBreakPoint.ElementAt(i);
                        var endItem = classBreakPoint.ElementAt(i + 1);
                        if (featureCount >= startItem.Key && featureCount < endItem.Key)
                        {
                            //Draw the point shape
                            startItem.Value.Draw(new[] { new Feature(tempMultiPointShape.GetCenterPoint(), featureValues) }, canvas, labelsInThisLayer, labelsInAllLayers);
                            isMatch = true;
                            break;
                        }
                    }
                    if (!isMatch && featureCount >= classBreakPoint.LastOrDefault().Key)
                    {
                        classBreakPoint.LastOrDefault().Value.Draw(new[] { new Feature(tempMultiPointShape.GetCenterPoint(), featureValues) }, canvas, labelsInThisLayer, labelsInAllLayers);
                    }

                    if (featureCount != 1)
                    {
                        // Draw the text style to show how many feaures are consolidated in the cluster
                        textSytle.Draw(new[] { new Feature(tempMultiPointShape.GetCenterPoint(), featureValues) }, canvas, labelsInThisLayer, labelsInAllLayers);
                    }
                }
            }
        }
    }
}