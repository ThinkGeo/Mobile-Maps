using Foundation;
using System;
using System.Collections.ObjectModel;
using ThinkGeo.MapSuite.iOS;
using UIKit;

namespace MapSuiteEarthquakeStatistics
{
    [Register("BaseMapTypeController")]
    public class BaseMapTypeController : UITableViewController
    {
        public Action DispalyBingMapKeyAlertView;
        public Action<UITableView, NSIndexPath> RowClick;

        public BaseMapTypeController()
        { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            UITableView baseMapTypeTableView = new UITableView(View.Frame);
            DataTableSource baseMapTypeSource = new DataTableSource();
            Collection<RowModel> baseMapTypeRows = new Collection<RowModel>();

            string[] baseMapTypeItems = { "ThinkGeo Cloud Maps Light", "ThinkGeo Cloud Maps Aerial", "ThinkGeo Cloud Maps Hybrid" };
            foreach (var nameItem in baseMapTypeItems)
            {
                RowModel row = new RowModel(nameItem)
                {
                    CellAccessory = UITableViewCellAccessory.Checkmark
                };
                baseMapTypeRows.Add(row);
            }
            baseMapTypeRows[0].IsChecked = true;

            SectionModel baseMapTypeSection = new SectionModel(string.Empty, baseMapTypeRows);

            baseMapTypeSource.Sections.Add(baseMapTypeSection);
            baseMapTypeSource.RowClick = BaseMapTypeRowClick;
            baseMapTypeTableView.Source = baseMapTypeSource;
            View = baseMapTypeTableView;
        }

        private void BaseMapTypeRowClick(UITableView tableView, NSIndexPath indexPath)
        {
            DataTableSource source = (DataTableSource)tableView.Source;
            string selectedItem = source.Sections[indexPath.Section].Rows[indexPath.Row].Name;
            Global.BaseMapTypeString = selectedItem;

            foreach (var row in source.Sections[0].Rows)
            {
                row.IsChecked = row.Name.Equals(selectedItem);
            }

            tableView.ReloadData();
            RefreshBaseMap();
            RowClick?.Invoke(tableView, indexPath);
        }

        private void RefreshBaseMap()
        {
            string baseMapTypeString = Global.BaseMapTypeString.Replace(" ", "");
            if (Enum.TryParse(baseMapTypeString, true, out BaseMapType mapType)) Global.BaseMapType = mapType;

            switch (Global.BaseMapType)
            {
                case BaseMapType.ThinkGeoCloudMapsLight:
                    Global.ThinkGeoCloudMapsOverlay.MapType = ThinkGeoCloudRasterMapsMapType.Light;
                    Global.MapView.Refresh();
                    break;
                case BaseMapType.ThinkGeoCloudMapsAerial:
                    Global.ThinkGeoCloudMapsOverlay.MapType = ThinkGeoCloudRasterMapsMapType.Aerial;
                    Global.MapView.Refresh();
                    break;
                case BaseMapType.ThinkGeoCloudMapsHybrid:
                    Global.ThinkGeoCloudMapsOverlay.MapType = ThinkGeoCloudRasterMapsMapType.Hybrid;
                    Global.MapView.Refresh();
                    break;
            }
        }
    }
}