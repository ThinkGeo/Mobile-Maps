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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.iOS;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;
using UIKit;

namespace MapSuiteEarthquakeStatistics
{
    public partial class MainFormViewController : UIViewController
    {
        private MapView iOSMap;
        private UIActivityIndicatorView loadingView;

        private UINavigationController optionNavigationController;
        private UIPopoverController optionsPopover;
        private MainOptionsFormViewController optionsController;
        private BaseMapTypeController baseTypeTableViewController;

        public MainFormViewController(IntPtr handle)
            : base(handle)
        { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            InitializeMap();
            InitializeComponent();
            InitializeSetting();
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
            string targetDictionary = @"AppData/SampleData";

            Proj4Projection proj4 = Global.GetWgs84ToMercatorProjection();
            string rootPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/CacheImages";

            // Please input your ThinkGeo Cloud Client ID / Client Secret to enable the background map. 
            ThinkGeoCloudRasterMapsOverlay thinkGeoCloudMapsOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud Client ID", "ThinkGeo Cloud Client Secret")
            {
                TileResolution = ThinkGeo.Cloud.TileResolution.High
            };

            // Earthquake points
            ShapeFileFeatureLayer earthquakePointLayer = new ShapeFileFeatureLayer(Path.Combine(targetDictionary, "usEarthquake.shp"));
            earthquakePointLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(PointStyles.CreateSimpleCircleStyle(GeoColor.SimpleColors.Red, 5, GeoColor.SimpleColors.White, 1));
            earthquakePointLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            earthquakePointLayer.FeatureSource.Projection = proj4;

            ShapeFileFeatureSource earthquakeHeatFeatureSource = new ShapeFileFeatureSource(Path.Combine(targetDictionary, "usEarthquake_Simplified.shp"))
            {
                Projection = proj4
            };

            HeatLayer earthquakeHeatLayer = new HeatLayer(earthquakeHeatFeatureSource)
            {
                HeatStyle = new HeatStyle(10, 75, DistanceUnit.Kilometer)
                {
                    Alpha = 180
                },
                IsVisible = false
            };

            DynamicIsoLineLayer earthquakeIsoLineLayer = CreateDynamicIsoLineLayer(earthquakeHeatFeatureSource);
            earthquakeIsoLineLayer.IsVisible = false;

            LayerOverlay highlightOverlay = new LayerOverlay();
            highlightOverlay.Layers.Add("EarthquakePointLayer", earthquakePointLayer);
            highlightOverlay.Layers.Add("EarthquakeHeatLayer", earthquakeHeatLayer);
            highlightOverlay.Layers.Add("EarthquakeIsoLineLayer", earthquakeIsoLineLayer);

            // Highlighted points
            InMemoryFeatureLayer selectedMarkerLayer = new InMemoryFeatureLayer();
            selectedMarkerLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = PointStyles.CreateSimpleCircleStyle(GeoColor.SimpleColors.Orange, 8, GeoColor.SimpleColors.White, 2);
            selectedMarkerLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            PointStyle highLightMarkerStyle = new PointStyle();
            highLightMarkerStyle.CustomPointStyles.Add(PointStyles.CreateSimpleCircleStyle(GeoColor.FromArgb(50, GeoColor.SimpleColors.Blue), 20, GeoColor.SimpleColors.LightBlue, 1));
            highLightMarkerStyle.CustomPointStyles.Add(PointStyles.CreateSimpleCircleStyle(GeoColor.FromArgb(255, 0, 122, 255), 10, GeoColor.SimpleColors.White, 2));

            InMemoryFeatureLayer highlightMarkerLayer = new InMemoryFeatureLayer();
            highlightMarkerLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = highLightMarkerStyle;
            highlightMarkerLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            highlightOverlay.Layers.Add("SelectMarkerLayer", selectedMarkerLayer);
            highlightOverlay.Layers.Add("HighlightMarkerLayer", highlightMarkerLayer);
            highlightOverlay.TileWidth = 512;
            highlightOverlay.TileHeight = 512;

            // Maps
            iOSMap = new MapView(View.Frame)
            {
                MapUnit = GeographyUnit.Meter,
                ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet(512),
                CurrentExtent = new RectangleShape(-19062735.6816748, 9273256.52450252, -5746827.16371793, 2673516.56066139),
                BackgroundColor = new UIColor(233, 229, 220, 200)
            };

            iOSMap.Overlays.Add(Global.ThinkGeoCloudMapsOverlayKey, thinkGeoCloudMapsOverlay);
            iOSMap.Overlays.Add(Global.HighLightOverlayKey, highlightOverlay);

            iOSMap.TrackOverlay.TrackShapeLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Clear();
            iOSMap.TrackOverlay.TrackShapeLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = PointStyles.CreateSimpleCircleStyle(GeoColor.FromArgb(80, GeoColor.SimpleColors.LightGreen), 8);
            iOSMap.TrackOverlay.TrackShapeLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = LineStyles.CreateSimpleLineStyle(GeoColor.SimpleColors.White, 3, true);
            iOSMap.TrackOverlay.TrackShapeLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.FromArgb(80, GeoColor.SimpleColors.LightGreen), GeoColor.SimpleColors.White, 2);
            iOSMap.TrackOverlay.TrackEnded += TrackInteractiveOverlayOnTrackEnded;
            Global.MapView = iOSMap;

            View.Add(iOSMap);
            iOSMap.Refresh();
        }

        private void InitializeComponent()
        {
            EarthquakeToolBar toolBar = EarthquakeToolBar.Instance;
            toolBar.ToolBarButtonClick += ToolbarButtonClick;
            OperationToolbar.SetItems(toolBar.GetToolBarItems().ToArray(), true);
            OperationToolbar.TintColor = UIColor.FromRGB(103, 103, 103);
            View.BringSubviewToFront(OperationToolbar);

            QueryResultView.Hidden = true;
            QueryResultView.Layer.Opacity = 0.85f;
            View.BringSubviewToFront(QueryResultView);

            QueryResultTableView.Layer.Opacity = 0.85f;
            QueryResultTableView.Layer.BorderColor = UIColor.Gray.CGColor;
            QueryResultTableView.Layer.BorderWidth = 1;
            QueryResultTableView.Layer.ShadowColor = UIColor.Red.CGColor;

            loadingView = new UIActivityIndicatorView(View.Frame)
            {
                Center = View.Center,
                ActivityIndicatorViewStyle = UIActivityIndicatorViewStyle.Gray
            };
            View.AddSubview(loadingView);
            View.BringSubviewToFront(loadingView);
        }

        private void InitializeSetting()
        {
            optionsController = (MainOptionsFormViewController)Global.FindViewController("MainOptionsFormViewController");
            optionsController.QueryEarthquakeResult = QueryEarthquakeResult;
            optionsController.OptionRowClick = OptionRowClick;
            optionsController.PreferredContentSize = new SizeF(420, 410);

            optionNavigationController = new UINavigationController(optionsController)
            {
                PreferredContentSize = new SizeF(420, 410)
            };

            baseTypeTableViewController = new BaseMapTypeController
            {
                RowClick = (view, path) =>
                {
                    optionNavigationController.PopToRootViewController(true);
                    DismissOptionController();
                }
            };
        }

        private DynamicIsoLineLayer CreateDynamicIsoLineLayer(FeatureSource earthquakeHeatFeatureSource)
        {
            Collection<GeoColor> levelAreaColors = new Collection<GeoColor>
            {
                GeoColor.FromHtml("#FFFFBE"),
                GeoColor.FromHtml("#FDFF9E"),
                GeoColor.FromHtml("#FDFF37"),
                GeoColor.FromHtml("#FDDA04"),
                GeoColor.FromHtml("#FFA701"),
                GeoColor.FromHtml("#FF6F02"),
                GeoColor.FromHtml("#EC0000"),
                GeoColor.FromHtml("#B90000"),
                GeoColor.FromHtml("#850100"),
                GeoColor.FromHtml("#620001"),
                GeoColor.FromHtml("#450005"),
                GeoColor.FromHtml("#2B0804")
            };

            earthquakeHeatFeatureSource.Open();
            Dictionary<PointShape, double> dataPoints = GetDataPoints(earthquakeHeatFeatureSource);
            GridInterpolationModel interpolationModel = new InverseDistanceWeightedGridInterpolationModel(3, double.MaxValue);
            DynamicIsoLineLayer earthquakeIsoLineLayer = new DynamicIsoLineLayer(dataPoints, GetClassBreakValues(dataPoints.Values, 12), interpolationModel, IsoLineType.ClosedLinesAsPolygons)
            {
                CellWidthInPixel = 32,
                CellHeightInPixel = 32
            };

            ClassBreakStyle levelClassBreakStyle = new ClassBreakStyle(earthquakeIsoLineLayer.DataValueColumnName);
            levelClassBreakStyle.ClassBreaks.Add(new ClassBreak(double.MinValue, new AreaStyle(new GeoPen(GeoColor.FromHtml("#FE6B06"), 1), new GeoSolidBrush(new GeoColor(100, levelAreaColors[0])))));
            for (int i = 0; i < earthquakeIsoLineLayer.IsoLineLevels.Count - 1; i++)
            {
                if (!levelClassBreakStyle.ClassBreaks.Any(c => c.Value == earthquakeIsoLineLayer.IsoLineLevels[i + 1]))
                {
                    levelClassBreakStyle.ClassBreaks.Add(new ClassBreak(earthquakeIsoLineLayer.IsoLineLevels[i + 1],
                        new AreaStyle(new GeoPen(GeoColor.FromHtml("#FE6B06"), 1),
                            new GeoSolidBrush(new GeoColor(100, levelAreaColors[i + 1])))));
                }
            }
            earthquakeIsoLineLayer.CustomStyles.Add(levelClassBreakStyle);
            return earthquakeIsoLineLayer;
        }

        private void TrackInteractiveOverlayOnTrackEnded(object sender, TrackEndedTrackInteractiveOverlayEventArgs args)
        {
            loadingView.StartAnimating();
            Task.Factory.StartNew(() =>
            {
                MultipolygonShape resultShape = AreaBaseShape.Union(iOSMap.TrackOverlay.TrackShapeLayer.InternalFeatures);

                ShapeFileFeatureLayer earthquakePointLayer = (ShapeFileFeatureLayer)Global.HighLightOverlay.Layers["EarthquakePointLayer"];

                earthquakePointLayer.Open();
                Collection<Feature> features = earthquakePointLayer.FeatureSource.GetFeaturesWithinDistanceOf(new Feature(resultShape), iOSMap.MapUnit, DistanceUnit.Meter, 0.0001, ReturningColumnsType.AllColumns);

                Global.QueriedFeatures.Clear();

                foreach (Feature feature in features)
                {
                    Global.QueriedFeatures.Add(feature);
                }

                Global.FilterSelectedEarthquakeFeatures();
                InvokeOnMainThread(() =>
                {
                    Global.HighLightOverlay.Refresh();
                    loadingView.StopAnimating();
                });
            });
        }

        private void ToolbarButtonClick(object sender, EventArgs e)
        {
            QueryResultView.AnimatedHide();
            UIBarButtonItem buttonItem = (UIBarButtonItem)sender;

            if (buttonItem != null)
            {
                switch (buttonItem.Title)
                {
                    case EarthquakeConstant.Cursor:
                        iOSMap.TrackOverlay.TrackMode = TrackMode.None;
                        break;

                    case EarthquakeConstant.Polygon:
                        iOSMap.TrackOverlay.TrackMode = TrackMode.Polygon;
                        break;

                    case EarthquakeConstant.Rectangle:
                        iOSMap.TrackOverlay.TrackMode = TrackMode.Rectangle;
                        break;

                    case EarthquakeConstant.Clear:
                        iOSMap.TrackOverlay.TrackMode = TrackMode.None;
                        ClearQueryResult();
                        iOSMap.Refresh();
                        break;

                    case EarthquakeConstant.Search:
                        iOSMap.TrackOverlay.TrackMode = TrackMode.None;
                        RefreshQueryResultData();
                        break;

                    case EarthquakeConstant.Options:
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
            Global.QueriedFeatures.Clear();
            ((InMemoryFeatureLayer)Global.HighLightOverlay.Layers["SelectMarkerLayer"]).InternalFeatures.Clear();
            ((InMemoryFeatureLayer)Global.HighLightOverlay.Layers["HighlightMarkerLayer"]).InternalFeatures.Clear();
            Global.HighLightOverlay.Refresh();

            iOSMap.TrackOverlay.TrackShapeLayer.InternalFeatures.Clear();
            iOSMap.TrackOverlay.Refresh();

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
                optionsPopover.PresentFromRect(new CGRect(View.Frame.Width - 55, View.Frame.Height - 35, 50, 50), View, UIPopoverArrowDirection.Down, true);
            }
        }

        private void OptionRowClick(string itemName)
        {
            if (itemName.Equals("Base Map"))
                optionNavigationController.PushViewController(baseTypeTableViewController, true);
            else
                DismissOptionController();
        }

        private void DismissOptionController()
        {
            if (Global.UserInterfaceIdiomIsPhone)
                optionsController.DismissViewController(true, null);
            else
                optionsPopover.Dismiss(true);
        }

        private Dictionary<PointShape, double> GetDataPoints(FeatureSource featureSource)
        {
            return (from feature in featureSource.GetAllFeatures(GetReturningColumns())
                    where double.Parse(feature.ColumnValues["MAGNITUDE"]) > 0
                    select new PointShape
                    {
                        X = double.Parse(feature.ColumnValues["LONGITUDE"], CultureInfo.InvariantCulture),
                        Y = double.Parse(feature.ColumnValues["LATITIUDE"], CultureInfo.InvariantCulture),
                        Z = double.Parse(feature.ColumnValues["MAGNITUDE"], CultureInfo.InvariantCulture)
                    }).ToDictionary(point => point, point => point.Z);
        }

        private static IEnumerable<string> GetReturningColumns()
        {
            //LONGITUDE
            yield return "LONGITUDE";
            //LATITIUDE
            yield return "LATITIUDE";
            //MAGNITUDE
            yield return "MAGNITUDE";
        }

        private static IEnumerable<double> GetClassBreakValues(IEnumerable<double> values, int count)
        {
            Collection<double> result = new Collection<double>();
            double[] sortedValues = values.OrderBy(v => v).ToArray();
            int classCount = sortedValues.Length / count;
            for (int i = 1; i < count; i++)
            {
                result.Add(sortedValues[i * classCount]);
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

            if (buttonItem.Title.Equals(EarthquakeConstant.Rectangle) || buttonItem.Title.Equals(EarthquakeConstant.Polygon))
                buttonItem.TintColor = highlightColor;
        }

        private void QueryEarthquakeResult()
        {
            if (Global.UserInterfaceIdiomIsPhone)
                optionsController.DismissViewController(true, null);
            else
                optionsPopover.Dismiss(true);
            RefreshQueryResultData();
        }

        private void RefreshQueryResultData()
        {
            if (QueryResultView.Hidden) QueryResultView.AnimatedShow();
            DataTableSource earthquakeSource;
            if (QueryResultTableView.Source == null)
            {
                earthquakeSource = new DataTableSource
                {
                    RowClick = EarthquakeRowClicked
                };
            }
            else
            {
                earthquakeSource = (DataTableSource)QueryResultTableView.Source;
            }
            earthquakeSource.Sections.Clear();

            Proj4Projection mercatorToWgs84Projection = Global.GetWgs84ToMercatorProjection();
            mercatorToWgs84Projection.Open();

            try
            {
                Global.FilterSelectedEarthquakeFeatures();

                InMemoryFeatureLayer selectMarkerLayer = (InMemoryFeatureLayer)Global.HighLightOverlay.Layers["SelectMarkerLayer"];

                GeoCollection<Feature> selectFeatures = selectMarkerLayer.InternalFeatures;

                SectionModel detailSection = new SectionModel("Queried Count: " + selectFeatures.Count)
                {
                    HeaderHeight = 50
                };
                foreach (var feature in selectFeatures)
                {
                    double latitude = 0;

                    if (double.TryParse(feature.ColumnValues["LONGITUDE"], out double longitude) && double.TryParse(feature.ColumnValues["LATITIUDE"], out latitude))
                    {
                        PointShape point = new PointShape(longitude, latitude);
                        point = (PointShape)mercatorToWgs84Projection.ConvertToInternalProjection(point);
                        longitude = point.X;
                        latitude = point.Y;
                    }

                    double.TryParse(feature.ColumnValues["MAGNITUDE"], out double magnitude);
                    double.TryParse(feature.ColumnValues["DEPTH_KM"], out double depth);
                    double.TryParse(feature.ColumnValues["YEAR"], out double year);

                    EarthquakeRow result = new EarthquakeRow
                    {
                        YearValue = year != -9999 ? year.ToString(CultureInfo.InvariantCulture) : "Unknown",
                        LocationValue = longitude.ToString("f2", CultureInfo.InvariantCulture),
                        LatitudeValue = latitude.ToString("f2", CultureInfo.InvariantCulture),
                        DepthValue = depth != -9999 ? depth.ToString(CultureInfo.InvariantCulture) : "Unknown",
                        MagnitudeValue = magnitude != -9999 ? magnitude.ToString(CultureInfo.InvariantCulture) : "Unknown"
                    };
                    result.LocationValue = feature.ColumnValues["LOCATION"];

                    detailSection.Rows.Add(new RowModel(result.ToString(), new UIImageView(UIImage.FromBundle("location"))));
                }
                earthquakeSource.Sections.Add(detailSection);

                QueryResultTableView.Source = earthquakeSource;
                QueryResultTableView.ReloadData();
            }
            finally
            {
                mercatorToWgs84Projection.Close();
            }
        }

        private void EarthquakeRowClicked(UITableView tableView, NSIndexPath indexPath)
        {
            InMemoryFeatureLayer selectMarkerLayer = (InMemoryFeatureLayer)Global.HighLightOverlay.Layers["SelectMarkerLayer"];
            Feature queryFeature = selectMarkerLayer.InternalFeatures[indexPath.Row];

            iOSMap.ZoomTo(queryFeature.GetBoundingBox().GetCenterPoint(), iOSMap.ZoomLevelSet.ZoomLevel15.Scale);
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