using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;

namespace MapSuiteSiteSelection
{
    public class FilterConfiguration
    {
        private double bufferValue;
        private DistanceUnit bufferDistanceUnit;
        private string queryColumnName;
        private string queryColumnValue;
        private FeatureLayer queryFeatureLayer;

        public FilterConfiguration()
        {
            bufferValue = 2;
            queryColumnName = "ROOMS";
            queryColumnValue = SettingKey.AllFeature;
            bufferDistanceUnit = DistanceUnit.Mile;
        }

        public double BufferValue
        {
            get { return bufferValue; }
            set { bufferValue = value; }
        }

        public DistanceUnit BufferDistanceUnit
        {
            get { return bufferDistanceUnit; }
            set { bufferDistanceUnit = value; }
        }

        public string QueryColumnName
        {
            get { return queryColumnName; }
            set { queryColumnName = value; }
        }

        public string QueryColumnValue
        {
            get { return queryColumnValue; }
            set { queryColumnValue = value; }
        }

        public FeatureLayer QueryFeatureLayer
        {
            get { return queryFeatureLayer; }
            set { queryFeatureLayer = value; }
        }
    }
}
