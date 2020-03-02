using System;
using System.Collections.Generic;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;

namespace AnalyzingVisualization
{
    public class PressureValueStyle : ValueStyle
    {
        public PressureValueStyle()
        {
            SquareTextPointStyle highPressurePointStyle = new SquareTextPointStyle();
            highPressurePointStyle.Text = "H";
            highPressurePointStyle.SymbolSolidBrush = new GeoSolidBrush(GeoColor.FromArgb(255, 39, 39, 245));

            SquareTextPointStyle lowPressurePointStyle = new SquareTextPointStyle();
            lowPressurePointStyle.Text = "L";
            lowPressurePointStyle.SymbolSolidBrush = new GeoSolidBrush(GeoColor.StandardColors.Red);

            ValueItems.Add(new ValueItem("L", lowPressurePointStyle));
            ValueItems.Add(new ValueItem("H", highPressurePointStyle));
        }

        public class SquareTextPointStyle : PointStyle
        {
            private string text;
            private GeoFont font;
            private GeoBrush textBrush;

            public SquareTextPointStyle()
            {
                SymbolType = PointSymbolType.Square;
                SymbolSize = 30;
                PointType = PointType.Symbol;
                font = new GeoFont("Verdana", 14);
                textBrush = new GeoSolidBrush(GeoColor.StandardColors.White);
                SymbolPen = new GeoPen(GeoColor.StandardColors.White, 1);
            }

            public string Text
            {
                get { return text; }
                set { text = value; }
            }

            protected override void DrawCore(IEnumerable<Feature> features, GeoCanvas canvas, System.Collections.ObjectModel.Collection<SimpleCandidate> labelsInThisLayer, System.Collections.ObjectModel.Collection<SimpleCandidate> labelsInAllLayers)
            {
                base.DrawCore(features, canvas, labelsInThisLayer, labelsInAllLayers);

                double resolution = Math.Max(canvas.CurrentWorldExtent.Width / canvas.Width, canvas.CurrentWorldExtent.Height / canvas.Height);
                foreach (Feature feature in features)
                {
                    PointShape pointShape = feature.GetShape() as PointShape;
                    if (pointShape != null)
                    {
                        float screenOffsetX = (float)((pointShape.X - canvas.CurrentWorldExtent.UpperLeftPoint.X) / resolution);
                        float screenOffsetY = (float)((canvas.CurrentWorldExtent.UpperLeftPoint.Y - pointShape.Y) / resolution);
                        canvas.DrawTextWithScreenCoordinate(Text, font, textBrush, screenOffsetX, screenOffsetY, DrawingLevel.LabelLevel);
                    }
                }
            }
        }
    }
}