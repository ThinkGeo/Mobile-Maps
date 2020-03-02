/*===========================================
    Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    a Client ID and Secret. These were sent to you via email when you signed up
    with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/

using CoreGraphics;
using Foundation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.iOS;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;
using UIKit;

namespace MapSuiteSiteSelection
{
    public partial class MainFormViewController : UIViewController
    {
        private MapView iOSMap;
        private UIPopoverController optionsPopover;
        private MainOptionFormViewController optionsController;
        private UINavigationController optionNavigationController;
        private UITableViewController filterTypeTableViewController;
        private UITableViewController pointSubTypeTableViewController;
        private UITableViewController unitTypeTableViewController;
        private BaseMapTypeController baseTypeTableViewController;

        public MainFormViewController(IntPtr handle)
            : base(handle)
        { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            InitializeMap();
            InitializeComponent();
            InitializeSiteSelectionSetting();
        }

        public override void WillAnimateRotation(UIInterfaceOrientation toInterfaceOrientation, double duration)
        {
            base.WillAnimateRotation(toInterfaceOrientation, duration);

            double resolution = Math.Max(iOSMap.CurrentExtent.Width / iOSMap.Frame.Width, iOSMap.CurrentExtent.Height / iOSMap.Frame.Height);
            iOSMap.Frame = View.Frame;

            iOSMap.CurrentExtent = GetExtentRetainScale(iOSMap.CurrentExtent.GetCenterPoint(), resolution);
            iOSMap.Refresh();
        }

        private void InitializeMap()
        {
            string targetPoisDictionary = @"AppData/POIs";
            string tempFolderRootPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/CacheImages";

            //LimitPolygon
            ShapeFileFeatureLayer limitPolygonLayer = new ShapeFileFeatureLayer("AppData/SampleData/CityLimitPolygon.shp");
            limitPolygonLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(new AreaStyle(new GeoPen(GeoColor.SimpleColors.White, 5.5f), new GeoSolidBrush(GeoColor.SimpleColors.Transparent)));
            limitPolygonLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(new AreaStyle(new GeoPen(GeoColor.SimpleColors.Red, 1.5f) { DashStyle = LineDashStyle.Dash }, new GeoSolidBrush(GeoColor.SimpleColors.Transparent)));
            limitPolygonLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            limitPolygonLayer.FeatureSource.Projection = Global.GetWgs84ToMercatorProjection();

            // Please input your ThinkGeo Cloud Client ID / Client Secret to enable the background map.
            ThinkGeoCloudRasterMapsOverlay thinkGeoCloudMapsOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud Client ID", "ThinkGeo Cloud Client Secret");
            thinkGeoCloudMapsOverlay.TileResolution = ThinkGeo.Cloud.TileResolution.High;

            ShapeFileFeatureLayer hotelsLayer = new ShapeFileFeatureLayer(Path.Combine(targetPoisDictionary, "Hotels.shp"));
            hotelsLayer.Name = Global.HotelsLayerKey;
            hotelsLayer.Transparency = 120f;
            hotelsLayer.ZoomLevelSet.ZoomLevel10.DefaultPointStyle = new PointStyle(GetGeoImage("hotel"));
            hotelsLayer.ZoomLevelSet.ZoomLevel10.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            hotelsLayer.FeatureSource.Projection = Global.GetWgs84ToMercatorProjection();

            ShapeFileFeatureLayer medicalFacilitesLayer = new ShapeFileFeatureLayer(Path.Combine(targetPoisDictionary, "Medical_Facilities.shp"));
            medicalFacilitesLayer.Name = Global.MedicalFacilitiesLayerKey;
            medicalFacilitesLayer.Transparency = 120f;
            medicalFacilitesLayer.ZoomLevelSet.ZoomLevel10.DefaultPointStyle = new PointStyle(GetGeoImage("store"));
            medicalFacilitesLayer.ZoomLevelSet.ZoomLevel10.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            medicalFacilitesLayer.FeatureSource.Projection = Global.GetWgs84ToMercatorProjection();

            ShapeFileFeatureLayer publicFacilitesLayer = new ShapeFileFeatureLayer(Path.Combine(targetPoisDictionary, "Public_Facilities.shp"));
            publicFacilitesLayer.Name = Global.PublicFacilitiesLayerKey;
            publicFacilitesLayer.Transparency = 120f;
            publicFacilitesLayer.ZoomLevelSet.ZoomLevel10.DefaultPointStyle = new PointStyle(GetGeoImage("public-facility"));
            publicFacilitesLayer.ZoomLevelSet.ZoomLevel10.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            publicFacilitesLayer.FeatureSource.Projection = Global.GetWgs84ToMercatorProjection();

            ShapeFileFeatureLayer restaurantsLayer = new ShapeFileFeatureLayer(Path.Combine(targetPoisDictionary, "Restaurants.shp"));
            restaurantsLayer.Name = Global.RestaurantsLayerKey;
            restaurantsLayer.Transparency = 120f;
            restaurantsLayer.ZoomLevelSet.ZoomLevel10.DefaultPointStyle = new PointStyle(GetGeoImage("restaurant"));
            restaurantsLayer.ZoomLevelSet.ZoomLevel10.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            restaurantsLayer.FeatureSource.Projection = Global.GetWgs84ToMercatorProjection();

            ShapeFileFeatureLayer schoolsLayer = new ShapeFileFeatureLayer(Path.Combine(targetPoisDictionary, "Schools.shp"));
            schoolsLayer.Name = Global.SchoolsLayerKey;
            schoolsLayer.Transparency = 120f;
            schoolsLayer.ZoomLevelSet.ZoomLevel10.DefaultPointStyle = new PointStyle(GetGeoImage("school"));
            schoolsLayer.ZoomLevelSet.ZoomLevel10.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            schoolsLayer.FeatureSource.Projection = Global.GetWgs84ToMercatorProjection();

            //Highlight Overlay
            GeoImage pinImage = GetGeoImage("map-pin");
            InMemoryFeatureLayer highlightCenterMarkerLayer = new InMemoryFeatureLayer();
            highlightCenterMarkerLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = new PointStyle(pinImage);
            highlightCenterMarkerLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle.YOffsetInPixel = -(pinImage.Height / 2f);
            highlightCenterMarkerLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            InMemoryFeatureLayer highlightMarkerLayer = new InMemoryFeatureLayer();
            highlightMarkerLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = new PointStyle(GetGeoImage("selected-halo"));
            highlightMarkerLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            InMemoryFeatureLayer highlightAreaLayer = new InMemoryFeatureLayer();
            highlightAreaLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(new GeoColor(120, GeoColor.FromHtml("#1749c9")), GeoColor.FromHtml("#fefec1"), 3, LineDashStyle.Solid);
            highlightAreaLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            LayerOverlay highlightOverlay = new LayerOverlay();
            highlightOverlay.TileType = TileType.SingleTile;
            highlightOverlay.TileWidth = 512;
            highlightOverlay.TileHeight = 512;

            highlightOverlay.Layers.Add(limitPolygonLayer);
            highlightOverlay.Layers.Add(Global.HotelsLayerKey, hotelsLayer);
            highlightOverlay.Layers.Add(Global.MedicalFacilitiesLayerKey, medicalFacilitesLayer);
            highlightOverlay.Layers.Add(Global.PublicFacilitiesLayerKey, publicFacilitesLayer);
            highlightOverlay.Layers.Add(Global.RestaurantsLayerKey, restaurantsLayer);
            highlightOverlay.Layers.Add(Global.SchoolsLayerKey, schoolsLayer);
            highlightOverlay.Layers.Add(Global.HighlightAreaLayerKey, highlightAreaLayer);
            highlightOverlay.Layers.Add(Global.HighlightMarkerLayerKey, highlightMarkerLayer);
            highlightOverlay.Layers.Add(Global.HighlightCenterMarkerLayerKey, highlightCenterMarkerLayer);

            // Maps
            iOSMap = new MapView(View.Frame);
            iOSMap.MapUnit = GeographyUnit.Meter;
            iOSMap.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            iOSMap.BackgroundColor = UIColor.FromRGB(244, 242, 238);
            iOSMap.CurrentExtent = new RectangleShape(-10789390.0630888, 3924457.19413373, -10768237.5787263, 3906066.41190523);
            iOSMap.TrackOverlay.TrackEnded += TrackInteractiveOverlayOnTrackEnded;

            iOSMap.Overlays.Add(Global.ThinkGeoCloudMapsOverlayKey, thinkGeoCloudMapsOverlay);
            iOSMap.Overlays.Add(Global.HighLightOverlayKey, highlightOverlay);

            Global.MapView = iOSMap;
            Global.FilterConfiguration.QueryFeatureLayer = hotelsLayer;

            View.Add(iOSMap);
            iOSMap.Refresh();
        }

        private void InitializeComponent()
        {
            ToolbarCandidates toolBar = ToolbarCandidates.Instance;
            toolBar.ToolBarButtonClick += ToolbarButtonClick;
            OperationToolbar.SetItems(toolBar.GetToolBarItems().ToArray(), true);
            View.BringSubviewToFront(OperationToolbar);

            QueryResultView.Hidden = true;
            QueryResultView.Layer.Opacity = 0.85f;
            View.BringSubviewToFront(QueryResultView);

            QueryResultTableView.Layer.Opacity = 0.85f;
            QueryResultTableView.Layer.BorderWidth = 1;
            QueryResultTableView.Layer.BorderColor = UIColor.Black.CGColor;
            View.BringSubviewToFront(QueryResultTableView);
        }

        private void InitializeSiteSelectionSetting()
        {
            optionsController = (MainOptionFormViewController)Global.FindViewController("MainOptionFormViewController");
            optionsController.OptionRowClick = OptionRowClick;
            optionsController.QueryEarthquakeResult = RefreshQueryResultData;
            optionsController.PreferredContentSize = new SizeF(420, 280);
            optionNavigationController = new UINavigationController(optionsController);
            optionNavigationController.PreferredContentSize = new SizeF(420, 280);

            filterTypeTableViewController = new UITableViewController();
            UITableView pointTypeTableView = new UITableView(View.Frame);
            DataTableSource pointTypeSource = new DataTableSource();
            Collection<RowModel> rows = new Collection<RowModel>();

            string[] PoiLayerNameItems = { "Hotels", "Medical Facilites", "Restaurants", "Schools", "Public Facilites" };
            foreach (var nameItem in PoiLayerNameItems)
            {
                RowModel row = new RowModel(nameItem);
                row.CellAccessory = UITableViewCellAccessory.DisclosureIndicator;
                rows.Add(row);
            }

            SectionModel section = new SectionModel(string.Empty, rows);

            pointTypeSource.Sections.Add(section);
            pointTypeSource.RowClick = PointTypeRowClick;
            pointTypeTableView.Source = pointTypeSource;
            filterTypeTableViewController.View = pointTypeTableView;

            pointSubTypeTableViewController = new UITableViewController();
            pointSubTypeTableViewController.View = new UITableView(View.Frame);

            unitTypeTableViewController = new UITableViewController();
            UITableView unitTypeTableView = new UITableView(View.Frame);
            DataTableSource unitTypeSource = new DataTableSource();
            Collection<RowModel> unitTypeRows = new Collection<RowModel>();

            //Create Unit Type table view controller.
            string[] unitTypeItems = { "Miles", "Kilometers" };
            foreach (var nameItem in unitTypeItems)
            {
                RowModel row = new RowModel(nameItem);
                row.CellAccessory = UITableViewCellAccessory.Checkmark;
                unitTypeRows.Add(row);
            }

            SectionModel unitTypeSection = new SectionModel(string.Empty, unitTypeRows);

            unitTypeSource.Sections.Add(unitTypeSection);
            unitTypeSource.RowClick = UnitTypeRowClick;
            unitTypeTableView.Source = unitTypeSource;
            unitTypeTableViewController.View = unitTypeTableView;

            baseTypeTableViewController = new BaseMapTypeController();
            baseTypeTableViewController.RowClick = (view, path) =>
            {
                optionNavigationController.PopToRootViewController(true);
                DismissOptionController();
            };
        }

        private void DismissOptionController()
        {
            if (Global.UserInterfaceIdiomIsPhone)
            {
                optionsController.DismissViewController(true, null);
            }
            else
            {
                optionsPopover.Dismiss(true);
            }
        }

        private void TrackInteractiveOverlayOnTrackEnded(object sender, TrackEndedTrackInteractiveOverlayEventArgs args)
        {
            Feature centerFeature = iOSMap.TrackOverlay.TrackShapeLayer.InternalFeatures.FirstOrDefault();

            InMemoryFeatureLayer highlightCenterMarkerLayer = (InMemoryFeatureLayer)Global.HighLightOverlay.Layers[Global.HighlightCenterMarkerLayerKey];

            highlightCenterMarkerLayer.InternalFeatures.Clear();
            highlightCenterMarkerLayer.InternalFeatures.Add(centerFeature);

            MapSuiteSampleHelper.UpdateHighlightOverlay();
            RefreshQueryResultData();

            iOSMap.TrackOverlay.TrackShapeLayer.InternalFeatures.Clear();
        }

        private void ToolbarButtonClick(object sender, EventArgs e)
        {
            UIBarButtonItem buttonItem = (UIBarButtonItem)sender;
            QueryResultView.AnimatedHide();
            if (buttonItem != null)
            {
                switch (buttonItem.Title)
                {
                    case SiteSelectionConstant.Pan:
                        iOSMap.TrackOverlay.TrackMode = TrackMode.None;
                        break;

                    case SiteSelectionConstant.Pin:
                        iOSMap.TrackOverlay.TrackMode = TrackMode.Point;
                        break;

                    case SiteSelectionConstant.Clear:
                        iOSMap.TrackOverlay.TrackMode = TrackMode.None;
                        ClearQueryResult();
                        break;

                    case SiteSelectionConstant.Search:
                        iOSMap.TrackOverlay.TrackMode = TrackMode.None;
                        RefreshQueryResultData();
                        break;

                    case SiteSelectionConstant.Options:
                        iOSMap.TrackOverlay.TrackMode = TrackMode.None;
                        ShowOptionsPopover(optionNavigationController);
                        break;

                    default:
                        iOSMap.TrackOverlay.TrackMode = TrackMode.None;
                        break;
                }
                RefreshToolbarItem(OperationToolbar, buttonItem);
            }
        }

        private void ClearQueryResult()
        {
            LayerOverlay highlightOverlay = Global.HighLightOverlay;

            InMemoryFeatureLayer highlightAreaLayer = (InMemoryFeatureLayer)highlightOverlay.Layers[Global.HighlightAreaLayerKey];
            InMemoryFeatureLayer highlightMarkerLayer = (InMemoryFeatureLayer)highlightOverlay.Layers[Global.HighlightMarkerLayerKey];
            InMemoryFeatureLayer highlightCenterMarkerLayer = (InMemoryFeatureLayer)highlightOverlay.Layers[Global.HighlightCenterMarkerLayerKey];

            highlightAreaLayer.InternalFeatures.Clear();
            highlightMarkerLayer.InternalFeatures.Clear();
            highlightCenterMarkerLayer.InternalFeatures.Clear();

            highlightOverlay.Refresh();

            QueryResultTableView.Source = null;
            QueryResultTableView.ReloadData();
        }

        private void ShowOptionsPopover(UIViewController popoverContentController)
        {
            if (Global.UserInterfaceIdiomIsPhone)
            {
                popoverContentController.ModalPresentationStyle = UIModalPresentationStyle.CurrentContext;
                popoverContentController.ModalTransitionStyle = UIModalTransitionStyle.CrossDissolve;
                PresentViewController(optionNavigationController, true, null);
            }
            else
            {
                if (optionsPopover == null) optionsPopover = new UIPopoverController(popoverContentController);
                optionsPopover.PresentFromRect(new CGRect(View.Frame.Width - 25, View.Frame.Height - 40, 40, 40), View, UIPopoverArrowDirection.Down, true);
            }
        }

        private void OptionRowClick(string itemName)
        {
            if (itemName.Contains("/"))
                optionNavigationController.PushViewController(filterTypeTableViewController, true);
            else if (itemName.Equals("UnitType"))
                optionNavigationController.PushViewController(unitTypeTableViewController, true);
            else if (itemName.Equals("Base Map"))
                optionNavigationController.PushViewController(baseTypeTableViewController, true);
            else
                DismissOptionController();
        }

        private void PointTypeRowClick(UITableView tableView, NSIndexPath indexPath)
        {
            DataTableSource source = (DataTableSource)tableView.Source;
            string pointType = source.Sections[indexPath.Section].Rows[indexPath.Row].Name;

            Global.FilterConfiguration.QueryFeatureLayer = GetQueryLayer(pointType);

            UITableView pointSubTypeTableView = (UITableView)pointSubTypeTableViewController.View;
            DataTableSource subTypeSource = new DataTableSource();
            subTypeSource.RowClick = PointSubTypeRowClick;

            string selectColumnName = GetDefaultColumnNameByPoiType(pointType);
            Global.FilterConfiguration.QueryColumnName = selectColumnName;

            IEnumerable<string> columns = GetColumnValueCandidates(selectColumnName);
            Collection<RowModel> rows = new Collection<RowModel>();
            foreach (var nameItem in columns)
            {
                RowModel row = new RowModel(nameItem);
                row.CellAccessory = UITableViewCellAccessory.Checkmark;
                rows.Add(row);
            }
            subTypeSource.Sections.Add(new SectionModel(string.Empty, rows));
            pointSubTypeTableView.Source = subTypeSource;

            optionNavigationController.PushViewController(pointSubTypeTableViewController, true);
        }

        private void PointSubTypeRowClick(UITableView tableView, NSIndexPath indexPath)
        {
            DataTableSource source = (DataTableSource)tableView.Source;
            string subPointType = source.Sections[indexPath.Section].Rows[indexPath.Row].Name;
            Global.FilterConfiguration.QueryColumnValue = subPointType;

            foreach (var row in source.Sections[0].Rows)
            {
                row.IsChecked = row.Name.Equals(subPointType);
            }
            tableView.ReloadData();
            RefreshQueryResultData();

            optionNavigationController.PopToRootViewController(true);
            DismissOptionController();
            iOSMap.Refresh();
        }

        private void UnitTypeRowClick(UITableView tableView, NSIndexPath indexPath)
        {
            DataTableSource source = (DataTableSource)tableView.Source;
            string unitType = source.Sections[indexPath.Section].Rows[indexPath.Row].Name;

            DistanceUnit distanceUnit;
            if (Enum.TryParse(unitType.TrimEnd('s'), true, out distanceUnit))
                Global.FilterConfiguration.BufferDistanceUnit = distanceUnit;

            foreach (var row in source.Sections[0].Rows)
            {
                row.IsChecked = row.Name.Equals(unitType);
            }
            tableView.ReloadData();
            RefreshQueryResultData();

            optionNavigationController.PopToRootViewController(true);
            DismissOptionController();
            iOSMap.Refresh();
        }

        private FeatureLayer GetQueryLayer(string pointType)
        {
            FeatureLayer resultLayer = null;
            LayerOverlay poiOverlay = Global.HighLightOverlay;
            switch (pointType)
            {
                case "Hotels":
                    resultLayer = (FeatureLayer)poiOverlay.Layers[Global.HotelsLayerKey];
                    break;
                case "Medical Facilites":
                    resultLayer = (FeatureLayer)poiOverlay.Layers[Global.MedicalFacilitiesLayerKey];
                    break;
                case "Restaurants":
                    resultLayer = (FeatureLayer)poiOverlay.Layers[Global.RestaurantsLayerKey];
                    break;
                case "Schools":
                    resultLayer = (FeatureLayer)poiOverlay.Layers[Global.SchoolsLayerKey];
                    break;
                case "Public Facilites":
                    resultLayer = (FeatureLayer)poiOverlay.Layers[Global.PublicFacilitiesLayerKey];
                    break;
            }
            return resultLayer;
        }

        private IEnumerable<string> GetColumnValueCandidates(string selectColumnName)
        {
            Collection<string> candidates = new Collection<string>();
            candidates.Add(Global.AllFeatureKey);
            if (selectColumnName.Equals("ROOMS"))
            {
                candidates.Add("1 ~ 50");
                candidates.Add("50 ~ 100");
                candidates.Add("100 ~ 150");
                candidates.Add("150 ~ 200");
                candidates.Add("200 ~ 300");
                candidates.Add("300 ~ 400");
                candidates.Add("400 ~ 500");
                candidates.Add(">= 500");
            }
            else
            {
                Global.FilterConfiguration.QueryFeatureLayer.Open();
                IEnumerable<string> distinctColumnValues = Global.FilterConfiguration.QueryFeatureLayer.FeatureSource.GetDistinctColumnValues(selectColumnName).Select(v => v.ColumnValue);
                foreach (var distinctColumnValue in distinctColumnValues)
                {
                    candidates.Add(distinctColumnValue);
                }
            }
            candidates.Remove(string.Empty);

            return candidates;
        }

        private string GetDefaultColumnNameByPoiType(string poiType)
        {
            string result = string.Empty;
            if (poiType.Equals("Restaurants", StringComparison.OrdinalIgnoreCase))
            {
                result = "FoodType";
            }
            else if (poiType.Equals("Medical Facilites", StringComparison.OrdinalIgnoreCase)
                || poiType.Equals("Schools", StringComparison.OrdinalIgnoreCase))
            {
                result = "TYPE";
            }
            else if (poiType.Equals("Public Facilites", StringComparison.OrdinalIgnoreCase))
            {
                result = "AGENCY";
            }
            else if (poiType.Equals("Hotels", StringComparison.OrdinalIgnoreCase))
            {
                result = "ROOMS";
            }
            return result;
        }

        private void RefreshToolbarItem(UIToolbar toolbar, UIBarButtonItem buttonItem)
        {
            UIColor defaultColor = UIColor.FromRGB(103, 103, 103);
            UIColor highlightColor = UIColor.FromRGB(27, 119, 222);
            // Set all item color to default.
            foreach (var item in toolbar.Items)
            {
                item.TintColor = defaultColor;
            }

            if (buttonItem.Title.Equals(SiteSelectionConstant.Pin))
            {
                buttonItem.TintColor = highlightColor;
            }
        }

        private void RefreshQueryResultData()
        {
            if (QueryResultView.Hidden) QueryResultView.AnimatedShow();
            DataTableSource similarSiteSourceSource;
            if (QueryResultTableView.Source == null)
            {
                similarSiteSourceSource = new DataTableSource();
                similarSiteSourceSource.RowClick = SimilarSiteSourceRowClicked;
                QueryResultTableView.Source = similarSiteSourceSource;
            }
            else
            {
                similarSiteSourceSource = (DataTableSource)QueryResultTableView.Source;
            }

            similarSiteSourceSource.Sections.Clear();

            InMemoryFeatureLayer highlightMarkerLayer = (InMemoryFeatureLayer)Global.HighLightOverlay.Layers[Global.HighlightMarkerLayerKey];

            MapSuiteSampleHelper.UpdateHighlightOverlay();

            GeoCollection<Feature> selectionFeatures = highlightMarkerLayer.InternalFeatures;

            SectionModel sectionModel = new SectionModel("Queried Count: " + selectionFeatures.Count);
            sectionModel.HeaderHeight = 50;
            foreach (var feature in highlightMarkerLayer.InternalFeatures)
            {
                sectionModel.Rows.Add(new RowModel(feature.ColumnValues["Name"], new UIImageView(UIImage.FromBundle("location"))));
            }

            similarSiteSourceSource.Sections.Add(sectionModel);

            QueryResultTableView.ReloadData();
        }

        private void SimilarSiteSourceRowClicked(UITableView tableView, NSIndexPath indexPath)
        {
            InMemoryFeatureLayer selectMarkerLayer = (InMemoryFeatureLayer)Global.HighLightOverlay.Layers[Global.HighlightMarkerLayerKey];
            Feature queryFeature = selectMarkerLayer.InternalFeatures[indexPath.Row];

            iOSMap.ZoomTo(queryFeature.GetBoundingBox().GetCenterPoint(), iOSMap.ZoomLevelSet.ZoomLevel15.Scale);
        }

        private GeoImage GetGeoImage(string name)
        {
            return new GeoImage(UIImage.FromBundle(name).AsPNG().AsStream());
        }

        private RectangleShape GetExtentRetainScale(PointShape currentLocationInMecator, double resolution = double.NaN)
        {
            if (double.IsNaN(resolution))
            {
                resolution = Math.Max(iOSMap.CurrentExtent.Width / iOSMap.Frame.Width, iOSMap.CurrentExtent.Height / iOSMap.Frame.Height);
            }

            double left = currentLocationInMecator.X - resolution * iOSMap.Frame.Width * .5;
            double right = currentLocationInMecator.X + resolution * iOSMap.Frame.Width * .5;
            double top = currentLocationInMecator.Y + resolution * iOSMap.Frame.Height * .5;
            double bottom = currentLocationInMecator.Y - resolution * iOSMap.Frame.Height * .5;
            return new RectangleShape(left, top, right, bottom);
        }
    }
}