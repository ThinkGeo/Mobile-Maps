
namespace LabelingStyle
{
    public abstract class StyleSettings
    {
        protected StyleSettings() { }

        public string Title { get; set; }

        /// <summary>
        /// This method parses the setting value to double.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>System.Double.</returns>
        protected static double ParseToDouble(string source, double defaultValue)
        {
            double temp;
            double result = defaultValue;
            if (double.TryParse(source, out temp))
            {
                result = temp;
            }

            return result;
        }

        /// <summary>
        /// This method parses the setting value to int.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>System.Int32.</returns>
        protected static int ParseToInt(string source, int defaultValue)
        {
            int temp;
            int result = defaultValue;
            if (int.TryParse(source, out temp))
            {
                result = temp;
            }

            return result;
        }

        /// <summary>
        /// This method parses the setting value to float.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>System.Single.</returns>
        protected static float ParseToFloat(string source, float defaultValue)
        {
            float temp;
            float result = defaultValue;
            if (float.TryParse(source, out temp))
            {
                result = temp;
            }

            return result;
        }
    }
}