using Android.Widget;
using ThinkGeo.Core;
using ThinkGeo.UI.Android;

namespace MapSuiteSiteSelection
{
    internal class PotentialSimilarSiteResultHolder : Java.Lang.Object
    {
        public TextView NameValue { get; set; }
        public ImageButton IconValue { get; set; }
        public Feature SelectedFeature { get; set; }
    }
}