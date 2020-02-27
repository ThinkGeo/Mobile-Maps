using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;

namespace AnalyzingVisualization
{
    public class CustomGeoImageLineStyle : LineStyle
    {
        private Collection<GeoImage> geoImages;
        private int spacing;
        private LineStyle lineStyle;
        private SymbolSide side;

        public CustomGeoImageLineStyle(LineStyle lineStyle, GeoImage geoImage, int spacing, SymbolSide side)
            : this(lineStyle, new Collection<GeoImage> { geoImage }, spacing, side)
        { }

        public CustomGeoImageLineStyle(LineStyle lineStyle, IEnumerable<GeoImage> geoImages, int spacing, SymbolSide side)
        {
            this.side = side;
            this.spacing = spacing;
            this.lineStyle = lineStyle;
            this.geoImages = new Collection<GeoImage>();
            foreach (var geoImage in geoImages)
            {
                this.geoImages.Add(geoImage);
            }
        }

        protected override void DrawCore(IEnumerable<Feature> features, GeoCanvas canvas, Collection<SimpleCandidate> labelsInThisLayer, Collection<SimpleCandidate> labelsInAllLayers)
        {
            PointStyle[] pointStyles = geoImages.Select(geoImage => { return new PointStyle(geoImage) { DrawingLevel = DrawingLevel.LevelThree }; }).ToArray();
            foreach (Feature feature in features)
            {
                LineShape lineShape = (LineShape)feature.GetShape();
                lineStyle.Draw(new BaseShape[] { lineShape }, canvas, labelsInThisLayer, labelsInAllLayers);

                int index = 0;
                double totalDist = 0;
                for (int i = 0; i < lineShape.Vertices.Count - 1; i++)
                {
                    PointShape pointShape1 = new PointShape(lineShape.Vertices[i]);
                    PointShape pointShape2 = new PointShape(lineShape.Vertices[i + 1]);

                    LineShape tempLineShape = new LineShape();
                    tempLineShape.Vertices.Add(lineShape.Vertices[i]);
                    tempLineShape.Vertices.Add(lineShape.Vertices[i + 1]);

                    double angle = GetAngleFromTwoVertices(lineShape.Vertices[i], lineShape.Vertices[i + 1]);

                    //Left side
                    if (side == SymbolSide.Left)
                    {
                        if (angle >= 270) { angle = angle - 180; }
                    }
                    //Right side
                    else
                    {
                        if (angle <= 90) { angle = angle + 180; }
                    }
                    //pointStyle.RotationAngle = (float)angle;
                    foreach (var pointStyle in pointStyles) pointStyle.RotationAngle = (float)angle;
                    float screenDist = ExtentHelper.GetScreenDistanceBetweenTwoWorldPoints(canvas.CurrentWorldExtent, pointShape1,
                                                                                        pointShape2, canvas.Width, canvas.Height);
                    double currentDist = Math.Round(pointShape1.GetDistanceTo(pointShape2, canvas.MapUnit, DistanceUnit.Meter), 2);
                    double worldInterval = (currentDist * spacing) / screenDist;

                    while (totalDist <= currentDist)
                    {
                        PointStyle pointStyle = pointStyles[index % pointStyles.Length];
                        PointShape tempPointShape = tempLineShape.GetPointOnALine(StartingPoint.FirstPoint, totalDist, canvas.MapUnit, DistanceUnit.Meter);
                        pointStyle.Draw(new BaseShape[] { tempPointShape }, canvas, labelsInThisLayer, labelsInAllLayers);
                        totalDist = totalDist + worldInterval;
                        index++;
                    }

                    totalDist = totalDist - currentDist;
                }
            }
        }

        private double GetAngleFromTwoVertices(Vertex b, Vertex c)
        {
            double alpha = 0;
            double tangentAlpha = (c.Y - b.Y) / (c.X - b.X);
            double Peta = Math.Atan(tangentAlpha);

            if (c.X > b.X)
            {
                alpha = 90 + (Peta * (180 / Math.PI));
            }
            else if (c.X < b.X)
            {
                alpha = 270 + (Peta * (180 / Math.PI));
            }
            else
            {
                if (c.Y > b.Y) alpha = 0;
                if (c.Y < b.Y) alpha = 180;
            }

            double offset;
            if (b.X > c.X)
            { offset = 90; }
            else { offset = -90; }

            return alpha + offset;
        }

        public enum SymbolSide
        {
            Right = 0,
            Left = 1
        }
    }
}