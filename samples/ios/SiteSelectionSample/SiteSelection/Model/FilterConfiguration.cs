using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;

namespace MapSuiteSiteSelection
{
    public class FilterConfiguration
    {
        public double BufferValue { get; set; }

        public DistanceUnit BufferDistanceUnit { get; set; }

        public string QueryColumnName { get; set; }

        public string QueryColumnValue { get; set; }

        public FeatureLayer QueryFeatureLayer { get; set; }

        public FilterConfiguration()
        {
            BufferValue = 2;
            QueryColumnName = "ROOMS";
            QueryColumnValue = Global.AllFeatureKey;
            BufferDistanceUnit = DistanceUnit.Mile;
        }
    }
}
