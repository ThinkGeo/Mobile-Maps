using System.IO;

namespace ThinkGeo.UI.Android.HowDoI
{
    internal class SampleHelper
    {
        // This ThinkGeo Cloud test key is exclusively for demonstration purposes and is limited to requesting base map 
        // tiles only. Do not use it in your own applications, as it may be restricted or disabled without prior notice. 
        // Please visit https://cloud.thinkgeo.com to create a free key for your own use.
        public static string ThinkGeoCloudId { get; } = "USlbIyO5uIMja2y0qoM21RRM6NBXUad4hjK3NBD6pD0~";
        public static string ThinkGeoCloudSecret { get; } = "f6OJsvCDDzmccnevX55nL7nXpPDXXKANe5cN6czVjCH0s8jhpCH-2A~~";
        public readonly static string AssetsDataDictionary = @"AppData";
        public readonly static string SampleDataDictionary = @"mnt/sdcard/MapSuiteSampleData/HowDoISamples";

        public static string GetDataPath(string fileName)
        {
            return Path.Combine(SampleDataDictionary, AssetsDataDictionary, fileName);
        }
    }
}