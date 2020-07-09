using Android.App;
using Android.OS;
using Android.Widget;
using System;
using ThinkGeo.Core;

namespace ThinkGeo.UI.Android.HowDoI
{
    public class UseWMSOverlay : SampleFragment
    {
        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnStart();

            
            mapView.MapUnit = GeographyUnit.DecimalDegree;
            mapView.CurrentExtent = new RectangleShape(-143.4, 109.3, 116.7, -76.3);

            WmsOverlay wmsOverlay = new WmsOverlay();
            wmsOverlay.ServerUris.Add(new Uri("http://ows.mundialis.de/services/service"));
            wmsOverlay.Parameters.Add("layers", "OSM-WMS");
            wmsOverlay.Parameters.Add("STYLES", "default");
            wmsOverlay.Parameters.Add("CRS", "3857");
            mapView.Overlays.Add(wmsOverlay);

            SampleViewHelper.InitializeInstruction(this.Context, currentView.FindViewById<RelativeLayout>(Resource.Id.MainLayout), base.SampleInfo);
        }
    }
}