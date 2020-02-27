using System;
using ThinkGeo.MapSuite.Shapes;

namespace MapSuiteSiteSelection
{
    public class ZoomToFeatureEventArgs : EventArgs
    {
        public ZoomToFeatureEventArgs()
            : this(null)
        { }

        public ZoomToFeatureEventArgs(Feature feature)
            : base()
        {
            ZoomToFeature = feature;
        }

        public Feature ZoomToFeature { get; set; }
    }
}