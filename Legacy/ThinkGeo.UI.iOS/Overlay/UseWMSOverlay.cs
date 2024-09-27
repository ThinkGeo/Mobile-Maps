using System;
using ThinkGeo.Core;

namespace ThinkGeo.UI.iOS.HowDoI
{
    public class UseWMSOverlay : BaseViewController
    {
        public UseWMSOverlay()
            : base()
        { }

        public override async void ViewDidLoad()
        {
            base.ViewDidLoad();

            MapView.MapUnit = GeographyUnit.DecimalDegree;
            MapView.CurrentExtent = new RectangleShape(-143.4, 109.3, 116.7, -76.3);

            WmsOverlay wmsOverlay = new WmsOverlay();
            wmsOverlay.Uri = new Uri("http://ows.mundialis.de/services/service");
            wmsOverlay.Parameters.Add("layers", "OSM-WMS");
            wmsOverlay.Parameters.Add("STYLES", "default");
            wmsOverlay.Parameters.Add("CRS", "3857");
            MapView.Overlays.Add(wmsOverlay);

            await MapView.RefreshAsync();
        }

        protected override void InitializeInstruction()
        {
            SampleUIHelper.InitializeInstruction(MapView, View, GetType(), isIphone => isIphone ? 100 : 80);
        }
    }
}