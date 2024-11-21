using System;
using System.Drawing;
using System.IO;
using ThinkGeo.Core;
using ThinkGeo.UI.iOS;
using UIKit;

namespace CSHowDoISamples
{
    public class DetailViewController : UIViewController
    {
        private MapView iOSMap;

        public DetailViewController(RectangleF frame)
            : base()
        {
            //iOSEditionPclShareEnviroment.Current = new iOSEditionPclShareEnviroment();

            base.ViewDidLoad();

            // Set the main view frame
            View.Frame = frame;
            View.BackgroundColor = UIColor.Red;
            View.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;

            // Create MapView
            iOSMap = new MapView(View.Frame);
            iOSMap.MapUnit = GeographyUnit.DecimalDegree;
            iOSMap.BackgroundColor = new UIColor(233, 229, 220, 200);
            iOSMap.CurrentExtent = new RectangleShape(-96.8172, 33.1299, -96.8050, 33.1226);
            LayerOverlay layerOverlay = new LayerOverlay();

            string targetDirectory = "AppData/Frisco";
            ShapeFileFeatureLayer txwatFeatureLayer = new ShapeFileFeatureLayer(Path.Combine(targetDirectory, "TXwat.shp"));
            txwatFeatureLayer.ZoomLevelSet.ZoomLevel12.DefaultAreaStyle.FillBrush = new GeoSolidBrush(GeoColor.FromArgb(255, 153, 179, 204));
            txwatFeatureLayer.ZoomLevelSet.ZoomLevel12.DefaultTextStyle = TextStyle.CreateSimpleTextStyle("LandName", "Arial", 9, DrawingFontStyles.Italic, GeoColors.Navy);
            txwatFeatureLayer.ZoomLevelSet.ZoomLevel12.DefaultTextStyle.SuppressPartialLabels = true;
            txwatFeatureLayer.ZoomLevelSet.ZoomLevel12.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            layerOverlay.Layers.Add("TXwat", txwatFeatureLayer);

            ShapeFileFeatureLayer txlka40FeatureLayer = new ShapeFileFeatureLayer(Path.Combine(targetDirectory, "TXlkaA40.shp"));
            txlka40FeatureLayer.ZoomLevelSet.ZoomLevel14.DefaultLineStyle = LineStyle.CreateSimpleLineStyle(GeoColors.DarkGray, 1F, false);
            txlka40FeatureLayer.ZoomLevelSet.ZoomLevel15.DefaultLineStyle = LineStyle.CreateSimpleLineStyle(GeoColors.White, 3F, GeoColors.DarkGray, 5F, true);
            txlka40FeatureLayer.ZoomLevelSet.ZoomLevel16.DefaultLineStyle = LineStyle.CreateSimpleLineStyle(GeoColors.White, 8F, GeoColors.DarkGray, 10F, true);
            txlka40FeatureLayer.ZoomLevelSet.ZoomLevel16.DefaultTextStyle = TextStyle.CreateSimpleTextStyle("[fedirp] [fename] [fetype] [fedirs]", "Arial", 10f, DrawingFontStyles.Regular, GeoColors.Black, 0, -1);
            txlka40FeatureLayer.ZoomLevelSet.ZoomLevel16.DefaultTextStyle.SuppressPartialLabels = true;
            txlka40FeatureLayer.ZoomLevelSet.ZoomLevel16.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            //txlka40FeatureLayer.DrawingMarginPercentage = 80;
            layerOverlay.Layers.Add("TXlkaA40", txlka40FeatureLayer);

            ShapeFileFeatureLayer txlkaA20FeatureLayer = new ShapeFileFeatureLayer(Path.Combine(targetDirectory, "TXlkaA20.shp"));
            txlkaA20FeatureLayer.ZoomLevelSet.ZoomLevel15.DefaultLineStyle = LineStyle.CreateSimpleLineStyle(GeoColor.FromArgb(255, 255, 255, 128), 6, GeoColors.LightGray, 9, true);
            txlkaA20FeatureLayer.ZoomLevelSet.ZoomLevel16.DefaultLineStyle = LineStyle.CreateSimpleLineStyle(GeoColor.FromArgb(255, 255, 255, 128), 9, GeoColors.LightGray, 12, true);
            txlkaA20FeatureLayer.ZoomLevelSet.ZoomLevel16.DefaultTextStyle = TextStyle.CreateSimpleTextStyle("[fedirp] [fename] [fetype] [fedirs]", "Arial", 12, DrawingFontStyles.Regular, GeoColors.Black, 0, -1);
            txlkaA20FeatureLayer.ZoomLevelSet.ZoomLevel16.DefaultTextStyle.SuppressPartialLabels = true;
            txlkaA20FeatureLayer.ZoomLevelSet.ZoomLevel16.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            layerOverlay.Layers.Add("TXlkaA20", txlkaA20FeatureLayer);
            iOSMap.Overlays.Add(layerOverlay);
            View.AddSubview(iOSMap);

            InitializeFunctionButtons();
        }

        //public void Update(int row)
        //{
        //    string targetDirectory;

        //    switch (row)
        //    {
        //        case 1:
        //            targetDirectory = "AppData/Frisco";
        //            ShapeFileFeatureLayer txwatFeatureLayer = new ShapeFileFeatureLayer(Path.Combine(targetDirectory, "TXwat.shp"));
        //            txwatFeatureLayer.ZoomLevelSet.ZoomLevel12.DefaultAreaStyle.FillBrush = new GeoSolidBrush(GeoColor.FromArgb(255, 153, 179, 204);
        //            txwatFeatureLayer.ZoomLevelSet.ZoomLevel12.DefaultTextStyle = TextStyle.CreateSimpleTextStyle("LandName", "Arial", 9, DrawingFontStyles.Italic, GeoColors.Navy);
        //            txwatFeatureLayer.ZoomLevelSet.ZoomLevel12.DefaultTextStyle.SuppressPartialLabels = true;
        //            txwatFeatureLayer.ZoomLevelSet.ZoomLevel12.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
        //            layerOverlay.Layers.Add("TXwat", txwatFeatureLayer);

        //            ShapeFileFeatureLayer txlka40FeatureLayer = new ShapeFileFeatureLayer(Path.Combine(targetDirectory, "TXlkaA40.shp"));
        //            txlka40FeatureLayer.ZoomLevelSet.ZoomLevel14.DefaultLineStyle = LineStyle.CreateSimpleLineStyle(GeoColors.DarkGray, 1F, false);
        //            txlka40FeatureLayer.ZoomLevelSet.ZoomLevel15.DefaultLineStyle = LineStyle.CreateSimpleLineStyle(GeoColors.White, 3F, GeoColors.DarkGray, 5F, true);
        //            txlka40FeatureLayer.ZoomLevelSet.ZoomLevel16.DefaultLineStyle = LineStyle.CreateSimpleLineStyle(GeoColors.White, 8F, GeoColors.DarkGray, 10F, true);
        //            txlka40FeatureLayer.ZoomLevelSet.ZoomLevel16.DefaultTextStyle = TextStyle.CreateSimpleTextStyle("[fedirp] [fename] [fetype] [fedirs]", "Arial", 10f, DrawingFontStyles.Regular, GeoColors.Black, 0, -1);
        //            txlka40FeatureLayer.ZoomLevelSet.ZoomLevel16.DefaultTextStyle.SuppressPartialLabels = true;
        //            txlka40FeatureLayer.ZoomLevelSet.ZoomLevel16.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
        //            txlka40FeatureLayer.DrawingMarginPercentage = 80;
        //            layerOverlay.Layers.Add("TXlkaA40", txlka40FeatureLayer);

        //            ShapeFileFeatureLayer txlkaA20FeatureLayer = new ShapeFileFeatureLayer(Path.Combine(targetDirectory, "TXlkaA20.shp"));
        //            txlkaA20FeatureLayer.ZoomLevelSet.ZoomLevel15.DefaultLineStyle = LineStyle.CreateSimpleLineStyle(GeoColor.FromArgb(255, 255, 255, 128), 6, GeoColors.LightGray, 9, true);
        //            txlkaA20FeatureLayer.ZoomLevelSet.ZoomLevel16.DefaultLineStyle = LineStyle.CreateSimpleLineStyle(GeoColor.FromArgb(255, 255, 255, 128), 9, GeoColors.LightGray, 12, true);
        //            txlkaA20FeatureLayer.ZoomLevelSet.ZoomLevel16.DefaultTextStyle = TextStyle.CreateSimpleTextStyle("[fedirp] [fename] [fetype] [fedirs]", "Arial", 12, DrawingFontStyles.Regular, GeoColors.Black, 0, -1);
        //            txlkaA20FeatureLayer.ZoomLevelSet.ZoomLevel16.DefaultTextStyle.SuppressPartialLabels = true;
        //            txlkaA20FeatureLayer.ZoomLevelSet.ZoomLevel16.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
        //            layerOverlay.Layers.Add("TXlkaA20", txlkaA20FeatureLayer);
        //            iOSMap.CurrentExtent = new RectangleShape(-96.8172, 33.1299, -96.8050, 33.1226);

        //            break;
        //        case 2:
        //            targetDirectory = "AppData/Frisco";
        //            string path = "AppData/Frisco/test.tgeo";
        //            TinyGeoFeatureLayer.CreateTinyGeoFile(path, Path.Combine(targetDirectory, "TXlkaA40.shp"), GeographyUnit.DecimalDegree, ReturningColumnsType.AllColumns);

        //            TinyGeoFeatureLayer tinyGeoFeatureLayer = new TinyGeoFeatureLayer(path);
        //            tinyGeoFeatureLayer.ZoomLevelSet.ZoomLevel14.DefaultLineStyle = LineStyle.CreateSimpleLineStyle(GeoColors.DarkGray, 1F, false);
        //            tinyGeoFeatureLayer.ZoomLevelSet.ZoomLevel15.DefaultLineStyle = LineStyle.CreateSimpleLineStyle(GeoColors.White, 3F, GeoColors.DarkGray, 5F, true);
        //            tinyGeoFeatureLayer.ZoomLevelSet.ZoomLevel16.DefaultLineStyle = LineStyle.CreateSimpleLineStyle(GeoColors.White, 8F, GeoColors.DarkGray, 10F, true);
        //            tinyGeoFeatureLayer.ZoomLevelSet.ZoomLevel16.DefaultTextStyle = TextStyle.CreateSimpleTextStyle("[fedirp] [fename] [fetype] [fedirs]", "Arial", 10f, DrawingFontStyles.Regular, GeoColors.Black, 0, -1);
        //            tinyGeoFeatureLayer.ZoomLevelSet.ZoomLevel16.DefaultTextStyle.SuppressPartialLabels = true;
        //            tinyGeoFeatureLayer.ZoomLevelSet.ZoomLevel16.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
        //            layerOverlay.Layers.Add(tinyGeoFeatureLayer);

        //            tinyGeoFeatureLayer.Open();
        //            iOSMap.CurrentExtent = tinyGeoFeatureLayer.GetBoundingBox();
        //            break;
        //        case 3:
        //            targetDirectory = "AppData/Gpx";
        //            GpxFeatureLayer gpxFeatureLayer = new GpxFeatureLayer(Path.Combine(targetDirectory, "afoxboro.gpx"));
        //            ValueStyle pointStyle = new ValueStyle();
        //            pointStyle.ColumnName = "IsWayPoint";
        //            pointStyle.ValueItems.Add(new ValueItem("0", PointStyles.CreateSimplePointStyle(PointSymbolType.Circle, GeoColors.Red, 4)));
        //            pointStyle.ValueItems.Add(new ValueItem("1", PointStyles.CreateSimplePointStyle(PointSymbolType.Circle, GeoColors.Green, 8)));
        //            LineStyle roadstyle = LineStyle.CreateSimpleLineStyle(GeoColors.Black, 1, true);
        //            gpxFeatureLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(pointStyle);
        //            gpxFeatureLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(roadstyle);
        //            gpxFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        //            GpxFeatureLayer gpxTextLayer = new GpxFeatureLayer(Path.Combine(targetDirectory, "afoxboro.gpx"));
        //            TextStyle labelStyle = TextStyle.CreateSimpleTextStyle("name", "Arial", 8, DrawingFontStyles.Bold, GeoColors.Black);
        //            labelStyle.TextPlacement = TextPlacement.UpperCenter;
        //            labelStyle.OverlappingRule = LabelOverlappingRule.NoOverlapping;
        //            labelStyle.YOffsetInPixel = 8;
        //            gpxTextLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(labelStyle);
        //            gpxTextLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
        //            layerOverlay.Layers.Add(gpxFeatureLayer);
        //            layerOverlay.Layers.Add(gpxTextLayer);

        //            gpxFeatureLayer.Open();
        //            iOSMap.CurrentExtent = gpxFeatureLayer.GetBoundingBox();
        //            break;
        //        case 4:
        //            targetDirectory = "AppData/Gif";
        //            GdiPlusRasterLayer radarImageLayer = new GdiPlusRasterLayer(Path.Combine(targetDirectory, "EWX_N0R_0.gif"));
        //            radarImageLayer.UpperThreshold = double.MaxValue;
        //            radarImageLayer.LowerThreshold = 0;
        //            layerOverlay.Layers.Add(radarImageLayer);
        //            layerOverlay.TileType = TileType.SingleTile;

        //            radarImageLayer.Open();
        //            iOSMap.CurrentExtent = radarImageLayer.GetBoundingBox();
        //            break;
        //        case 5:
        //            targetDirectory = "AppData/Tiff";
        //            GeoTiffRasterLayer geoTiffRasterLayer = new GeoTiffRasterLayer(Path.Combine(targetDirectory, "world.tif"));
        //            geoTiffRasterLayer.UpperThreshold = double.MaxValue;
        //            geoTiffRasterLayer.LowerThreshold = 0;
        //            geoTiffRasterLayer.IsGrayscale = false;
        //            layerOverlay.Layers.Add(geoTiffRasterLayer);

        //            geoTiffRasterLayer.Open();
        //            iOSMap.CurrentExtent = geoTiffRasterLayer.GetBoundingBox();
        //            break;
        //        case 6:
        //            targetDirectory = "AppData/Tab";
        //            TabFeatureLayer tabFeatureLayer = new TabFeatureLayer(Path.Combine(targetDirectory, "HoustonMuniBdySamp_Boundary.TAB"));
        //            tabFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(100, GeoColors.Green), GeoColors.Green);
        //            tabFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
        //            layerOverlay.Layers.Add("WorldLayer", tabFeatureLayer);

        //            tabFeatureLayer.Open();
        //            iOSMap.CurrentExtent = tabFeatureLayer.GetBoundingBox();
        //            break;
        //        case 7:
        //            string rootPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        //            BingMapsTileOverlay bingMapOverlay = new BingMapsTileOverlay();
        //            bingMapOverlay.ApplicationId = "AhDBjJalvtRXvYe6BsKuj2DwHT9Atlkas7HNVU7rDvoUvIu0_L_GVtSc8y9gNP61";
        //            bingMapOverlay.MapType = BingMapsMapType.AerialWithLabels;
        //            bingMapOverlay.TileCache = new FileBitmapTileCache(rootPath + "/CacheImages", "Road");
        //            bingMapOverlay.TileCache.TileMatrix.BoundingBoxUnit = GeographyUnit.Meter;
        //            bingMapOverlay.TileCache.TileMatrix.BoundingBox = bingMapOverlay.GetBoundingBox();
        //            bingMapOverlay.TileCache.ImageFormat = TileImageFormat.Jpeg;
        //            iOSMap.Overlays.Add(bingMapOverlay);

        //            iOSMap.MapUnit = GeographyUnit.Meter;
        //            iOSMap.ZoomLevelSet = new BingMapsZoomLevelSet();
        //            iOSMap.CurrentExtent = bingMapOverlay.GetBoundingBox();
        //            break;
        //        case 8:
        //            GoogleMapsTileOverlay googleMapsOverlay = new GoogleMapsTileOverlay();
        //            googleMapsOverlay.TileType = TileType.SingleTile;

        //            ManagedProj4Projection proj4 = new ManagedProj4Projection();
        //            proj4.InternalProjectionParametersString = ManagedProj4Projection.GetDecimalDegreesParametersString();
        //            proj4.ExternalProjectionParametersString = ManagedProj4Projection.GetGoogleMapParametersString();
        //            proj4.Open();

        //            iOSMap.MapUnit = GeographyUnit.Meter;
        //            iOSMap.ZoomLevelSet = new GoogleMapsZoomLevelSet();
        //            iOSMap.Overlays.Add(googleMapsOverlay);

        //            iOSMap.CurrentExtent = proj4.ConvertToExternalProjection(new RectangleShape(-139.2, 92.4, 120.9, -93.2)) as RectangleShape;
        //            break;
        //        default:
        //            break;
        //    }
        //    iOSMap.Refresh();
        //}

        private void InitializeFunctionButtons()
        {
            UIButton zoomInButton = GetUIButton(0, "Plus", (sender, o) => iOSMap.ZoomIn(50));
            UIButton zoomOutButton = GetUIButton(40, "Minus", (sender, o) => iOSMap.ZoomOut(50));
            UIButton editButton = GetUIButton(330, "Edit", TrackButtonClick);
            UIButton lineButton = GetUIButton(130, "Line", TrackButtonClick);
            UIButton pointButton = GetUIButton(90, "Point", TrackButtonClick);
            UIButton clearButton = GetUIButton(10, "Clear", TrackButtonClick);
            UIButton cursorButton = GetUIButton(50, "Cursor", TrackButtonClick);
            UIButton circleButton = GetUIButton(210, "Circle", TrackButtonClick);
            UIButton polygonButton = GetUIButton(250, "Polygon", TrackButtonClick);
            UIButton ellipseButton = GetUIButton(290, "Ellipse", TrackButtonClick);
            UIButton rectangleButton = GetUIButton(170, "Rectangle", TrackButtonClick);

            UIView zoomButtonsView = new UIView(new RectangleF((int)View.Frame.Width - 50, 20, 40, 80));
            zoomButtonsView.BackgroundColor = UIColor.FromRGBA(215, 215, 215, 255);
            zoomButtonsView.Add(zoomInButton);
            zoomButtonsView.Add(zoomOutButton);
            View.AddSubview(zoomButtonsView);

            UIView trackButtonsView = new UIView(new RectangleF(10, 20, 40, 380));
            trackButtonsView.Layer.CornerRadius = 8;
            trackButtonsView.BackgroundColor = UIColor.FromRGBA(215, 215, 215, 255);
            trackButtonsView.AutoresizingMask = UIViewAutoresizing.FlexibleBottomMargin;
            trackButtonsView.AddSubview(lineButton);
            trackButtonsView.AddSubview(pointButton);
            trackButtonsView.AddSubview(editButton);
            trackButtonsView.AddSubview(clearButton);
            trackButtonsView.AddSubview(circleButton);
            trackButtonsView.AddSubview(cursorButton);
            trackButtonsView.AddSubview(polygonButton);
            trackButtonsView.AddSubview(ellipseButton);
            trackButtonsView.AddSubview(rectangleButton);
            View.AddSubview(trackButtonsView);
        }

        private void TrackButtonClick(object sender, EventArgs e)
        {
            UIButton button = (UIButton)sender;
            switch (button.Title(UIControlState.Application))
            {
                case "Cursor":
                    iOSMap.TrackOverlay.TrackMode = TrackMode.None;
                    iOSMap.EditOverlay.ClearAllControlPoints();
                    iOSMap.Refresh();
                    break;

                case "Clear":
                    iOSMap.EditOverlay.ClearAllControlPoints();
                    iOSMap.EditOverlay.EditShapesLayer.Open();
                    iOSMap.EditOverlay.EditShapesLayer.Clear();
                    iOSMap.TrackOverlay.TrackShapeLayer.Open();
                    iOSMap.TrackOverlay.TrackShapeLayer.Clear();
                    iOSMap.Refresh();
                    break;

                case "Point":
                    iOSMap.TrackOverlay.TrackMode = TrackMode.Point;
                    break;

                case "Line":
                    iOSMap.TrackOverlay.TrackMode = TrackMode.Line;
                    break;

                case "Rectangle":
                    iOSMap.TrackOverlay.TrackMode = TrackMode.Rectangle;
                    break;

                case "Polygon":
                    iOSMap.TrackOverlay.TrackMode = TrackMode.Polygon;
                    break;

                case "Circle":
                    iOSMap.TrackOverlay.TrackMode = TrackMode.Circle;
                    break;

                case "Ellipse":
                    iOSMap.TrackOverlay.TrackMode = TrackMode.Ellipse;
                    break;

                case "Edit":
                    iOSMap.TrackOverlay.TrackMode = TrackMode.None;
                    foreach (Feature feature in iOSMap.TrackOverlay.TrackShapeLayer.InternalFeatures)
                    {
                        iOSMap.EditOverlay.EditShapesLayer.InternalFeatures.Add(feature);
                    }
                    iOSMap.TrackOverlay.TrackShapeLayer.InternalFeatures.Clear();
                    iOSMap.EditOverlay.CalculateAllControlPoints();
                    iOSMap.Refresh();
                    break;

                default:
                    iOSMap.TrackOverlay.TrackMode = TrackMode.None;
                    break;
            }
        }

        private UIButton GetUIButton(int topLocation, string imageName, EventHandler handler)
        {
            SizeF buttonSize = new SizeF(40, 40);
            UIButton button = new UIButton(new RectangleF(new Point(0, topLocation), buttonSize));
            button.SetImage(UIImage.FromBundle(imageName), UIControlState.Normal);
            button.SetTitle(imageName, UIControlState.Application);
            button.TouchUpInside += handler;
            return button;
        }
    }
}