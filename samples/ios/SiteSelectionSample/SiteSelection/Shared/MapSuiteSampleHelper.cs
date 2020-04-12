using CoreGraphics;
using System.Collections.ObjectModel;
using ThinkGeo.Core;
using ThinkGeo.UI.iOS;
using UIKit;

namespace MapSuiteSiteSelection
{
    public static class MapSuiteSampleHelper
    {
        public static void AnimatedShow(this UIView view)
        {
            UIView.Animate(0.3, () =>
            {
                view.Transform = CGAffineTransform.MakeTranslation(0, 0);
                view.Hidden = false;
            }, () =>
            {
                view.Hidden = false;
            });
        }

        public static void AnimatedHide(this UIView view)
        {
            UIView.Animate(0.3, () =>
            {
                view.Transform = CGAffineTransform.MakeTranslation(0, view.Frame.Height);
                view.Hidden = true;
            });
        }

        public static void UpdateHighlightOverlay()
        {
            LayerOverlay highlightOverlay = Global.HighLightOverlay;

            InMemoryFeatureLayer highlightAreaLayer = (InMemoryFeatureLayer)highlightOverlay.Layers[Global.HighlightAreaLayerKey];
            InMemoryFeatureLayer highlightMarkerLayer = (InMemoryFeatureLayer)highlightOverlay.Layers[Global.HighlightMarkerLayerKey];
            InMemoryFeatureLayer highlightCenterMarkerLayer = (InMemoryFeatureLayer)highlightOverlay.Layers[Global.HighlightCenterMarkerLayerKey];

            if (highlightCenterMarkerLayer.InternalFeatures.Count > 0)
            {
                highlightAreaLayer.InternalFeatures.Clear();
                highlightMarkerLayer.InternalFeatures.Clear();

                MultipolygonShape bufferResultShape = highlightCenterMarkerLayer.InternalFeatures[0].GetShape().Buffer(Global.FilterConfiguration.BufferValue, Global.MapView.MapUnit, Global.FilterConfiguration.BufferDistanceUnit);
                Global.FilterConfiguration.QueryFeatureLayer.Open();
                Collection<Feature> filterResultFeatures = Global.FilterConfiguration.QueryFeatureLayer.FeatureSource.GetFeaturesWithinDistanceOf(bufferResultShape, Global.MapView.MapUnit, DistanceUnit.Meter, 0.1, ReturningColumnsType.AllColumns);

                highlightAreaLayer.InternalFeatures.Add(new Feature(bufferResultShape));
                foreach (Feature feature in FilterFeaturesByColumnValue(filterResultFeatures))
                {
                    highlightMarkerLayer.InternalFeatures.Add(feature);
                }

                highlightOverlay.Refresh();
            }
        }

        private static Collection<Feature> FilterFeaturesByColumnValue(Collection<Feature> filterResultFeatures)
        {
            Collection<Feature> resultFeatures = new Collection<Feature>();

            foreach (Feature feature in filterResultFeatures)
            {
                if (Global.FilterConfiguration.QueryColumnValue.Equals(Global.AllFeatureKey))
                {
                    resultFeatures.Add(feature);
                }
                else if (Global.FilterConfiguration.QueryColumnValue.Equals(feature.ColumnValues[Global.FilterConfiguration.QueryColumnName]))
                {
                    resultFeatures.Add(feature);
                }
                else if (Global.FilterConfiguration.QueryFeatureLayer.Name.Equals(Global.HotelsLayerKey))
                {
                    string[] values = Global.FilterConfiguration.QueryColumnValue.Split('~');
                    double min = 0, max = 0, value = 0;
                    if (double.TryParse(values[0], out min) && double.TryParse(values[1], out max)
                        && double.TryParse(feature.ColumnValues[Global.FilterConfiguration.QueryColumnName], out value))
                    {
                        if (min < value && value < max)
                        {
                            resultFeatures.Add(feature);
                        }
                    }
                }
            }

            return resultFeatures;
        }
    }
}