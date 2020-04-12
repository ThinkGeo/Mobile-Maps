using CoreGraphics;
using System;
using System.Globalization;
using ThinkGeo.UI.iOS;
using UIKit;

namespace GettingStartedSample
{
    /// <summary>
    /// This class represents the Scale/ZoomLevel we displayed on the map. 
    /// </summary>
    public class ScaleZoomLevelMapTool : MapTool
    {
        private UILabel contentLabel;

        public ScaleZoomLevelMapTool()
        {
            IsEnabled = true;
            contentLabel = new UILabel();
        }

        protected override void InitializeCore(MapView mapView)
        {
            base.InitializeCore(mapView);
            Frame = new CGRect(5, mapView.Frame.Bottom - 30, mapView.Frame.Width - 10, 30);
            contentLabel.LineBreakMode = UILineBreakMode.WordWrap;
            contentLabel.Lines = 2;
            contentLabel.Font = UIFont.FromName("Arial", 12);
            contentLabel.Frame = new CGRect(0, 0, Frame.Width, Frame.Height);
            contentLabel.TextAlignment = UITextAlignment.Right;
            AddSubview(contentLabel);

            UpdateScaleContent(mapView.CurrentScale);
            mapView.CurrentScaleChanged += MapView_CurrentExtentChanged;
            mapView.AddSubview(this);
        }

        private void MapView_CurrentExtentChanged(object sender, CurrentScaleChangedMapViewEventArgs e)
        {
            UpdateScaleContent(e.NewScale);
        }

        private void UpdateScaleContent(double newScale = double.NaN)
        {
            if (!double.IsNaN(newScale))
            {
                int zoomLevelIndex = CurrentMap.GetSnappedZoomLevelIndex(newScale);
                string textFormat = "Scale 1:{1:N0}  Zoom Level {0:N0}";
                contentLabel.Text = String.Format(CultureInfo.InvariantCulture, textFormat, zoomLevelIndex, newScale);
            }
            else
            {
                contentLabel.Text = "Scale 1:--  Zoom Level --";
            }
        }
    }
}