using Android.Content;
using Android.Util;
using Android.Widget;
using System;
using System.Globalization;
using ThinkGeo.MapSuite.Android;

namespace GettingStartedSample
{
    /// <summary>
    /// This class represents the Scale/ZoomLevel we displayed on the map.
    /// </summary>
    public class ScaleZoomLevelMapTool : MapTool
    {
        private TextView contentLabel;
        private MapView currentMap;

        public ScaleZoomLevelMapTool(MapView mapView, Context context)
            : base(context)
        {
            Init(mapView, context, null);
        }

        public ScaleZoomLevelMapTool(MapView mapView, Context context, IAttributeSet attrs)
            : base(context)
        {
            Init(mapView, context, attrs);
        }

        private void Init(MapView mapView, Context context, IAttributeSet attrs)
        {
            currentMap = mapView;

            contentLabel = new TextView(context);
            contentLabel.SetTextColor(Android.Graphics.Color.Black);
            AddView(contentLabel);

            mapView.CurrentScaleChanged += MapView_CurrentScaleChanged;
        }

        private void MapView_CurrentScaleChanged(object sender, CurrentScaleChangedMapViewEventArgs e)
        {
            UpdateScaleContent(e.NewScale);
        }

        private void UpdateScaleContent(double newScale = double.NaN)
        {
            if (!double.IsNaN(newScale))
            {
                int zoomLevelIndex = currentMap.GetSnappedZoomLevelIndex(newScale);
                contentLabel.Text = String.Format(CultureInfo.InvariantCulture, "Scale 1 : {0:N0}\r\nZoomLevel : {1:N0}", newScale, zoomLevelIndex);
            }
            else
            {
                contentLabel.Text = "Scale 1 : --\r\nZoomLevel : --";
            }
        }

        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                contentLabel.Dispose();
                GC.SuppressFinalize(this);
            }
        }
    }
}