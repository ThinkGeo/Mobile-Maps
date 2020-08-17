﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinkGeo.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.Xamarin.HowDoI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PrintMapsUsingPrintOverlaySample : ContentPage
    {
        public PrintMapsUsingPrintOverlaySample()
        {
            InitializeComponent();
        }

        /// <summary>
        /// ...
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            SetupMapForPrinting();
            AddPageTitleLabel();
            AddMapLayers();
            AddMosquitoDataGrid();
        }
        /// <summary>
        /// Creates a PrintDocument and draws all of the layers for it to print onto.
        /// </summary>
        private void PrintMap_OnClick(object sender, EventArgs e)
        {
            //PrinterInteractiveOverlay printerOverlay = (PrinterInteractiveOverlay)mapView.InteractiveOverlays["printerOverlay"];
            //PagePrinterLayer pageLayer = (PagePrinterLayer)printerOverlay.PrinterLayers["pageLayer"];

            //// Create a printDocument that matches the size of our pageLayer
            //var printDocument = new PrintDocument
            //{
            //    DefaultPageSettings =
            //    {
            //        Landscape = pageLayer.Orientation == PrinterOrientation.Landscape,
            //        PaperSize = new PaperSize("AnsiA", 850, 1100)
            //    }
            //};

            //// Create a printerGeoCanvas that will allow us to print Layers onto the printDocument
            //var printerGeoCanvas = new PrinterGeoCanvas
            //{
            //    DrawingArea = new Rectangle(0, 0, (int)printDocument.DefaultPageSettings.PrintableArea.Width, (int)printDocument.DefaultPageSettings.PrintableArea.Height)
            //};

            //// Start drawing on the printDocument
            //printerGeoCanvas.BeginDrawing(printDocument, pageLayer.GetBoundingBox(), mapView.MapUnit);

            //// Draw each layer in the PrinterLayers collection except for the background PagePrinterLayer
            //foreach (var printerLayer in printerOverlay.PrinterLayers.Reverse())
            //{
            //    printerLayer.IsDrawing = true;
            //    if (!(printerLayer is PagePrinterLayer))
            //    {
            //        // Draw the layer
            //        printerLayer.Draw(printerGeoCanvas, new Collection<SimpleCandidate>());
            //    }

            //    printerLayer.IsDrawing = false;
            //}

            //// Finish drawing and send the print commands to the printer
            //printerGeoCanvas.EndDrawing();
        }

        /// <summary>
        /// Setup the mapView for a print preview and add a printerOverlay to hold various print layers
        /// </summary>
        private void SetupMapForPrinting()
        {
            //// Set the map's unit of measurement to meters(Spherical Mercator)
            //mapView.MapUnit = GeographyUnit.Meter;

            //// Set the map's ZoomLevelSet to a set of common printer zoom settings
            //mapView.ZoomLevelSet =
            //    new PrinterZoomLevelSet(GeographyUnit.Meter, PrinterHelper.GetPointsPerGeographyUnit(GeographyUnit.Meter));
            //mapView.MinimumScale = mapView.ZoomLevelSet.ZoomLevel20.Scale;

            //PrinterInteractiveOverlay printerOverlay = new PrinterInteractiveOverlay();
            //PagePrinterLayer pageLayer = new PagePrinterLayer(PrinterPageSize.AnsiA, PrinterOrientation.Portrait);

            //// Style the pageLayer to appear to look like a piece of paper
            //pageLayer.BackgroundMask = AreaStyle.CreateSimpleAreaStyle(GeoColors.White, GeoColors.Black);

            //// Add the pageLayer to the printerOverlay
            //printerOverlay.PrinterLayers.Add("pageLayer", pageLayer);

            //// Add the printerOverlay to the map
            //mapView.InteractiveOverlays.Add("printerOverlay", printerOverlay);

            //// Set the map extent
            //mapView.CurrentExtent = pageLayer.GetPosition().GetBoundingBox();
        }

        /// <summary>
        /// Adds a title to the top of the page
        /// </summary>
        private void AddPageTitleLabel()
        {
            //PrinterInteractiveOverlay printerOverlay = (PrinterInteractiveOverlay)mapView.InteractiveOverlays["printerOverlay"];
            //PagePrinterLayer pageLayer = (PagePrinterLayer)printerOverlay.PrinterLayers["pageLayer"];

            //var titleLabel = new LabelPrinterLayer("Frisco Mosquito Report - 5/5/2020", new GeoFont("Verdana", 8), GeoBrushes.Black)
            //{
            //    PrinterWrapMode = PrinterWrapMode.AutoSizeText
            //};
            //titleLabel.SetPosition(7.5, .5, 0, 4.75, PrintingUnit.Inch);

            //printerOverlay.PrinterLayers.Add(titleLabel);
        }

        /// <summary>
        /// Creates various layers from shapefile data and adds them to a mapPrinterLayer that will be able to translate the layers into print commands
        /// </summary>
        private void AddMapLayers()
        {
            //PrinterInteractiveOverlay printerOverlay = (PrinterInteractiveOverlay)mapView.InteractiveOverlays["printerOverlay"];
            //PagePrinterLayer pageLayer = (PagePrinterLayer)printerOverlay.PrinterLayers["pageLayer"];

            ///***************************
            // * Create cityLimits layer *
            // ***************************/
            //var cityLimits = new ShapeFileFeatureLayer(@"../../../Data/Shapefile/FriscoCityLimits.shp");

            //// Style cityLimits layer
            //cityLimits.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColors.Transparent, GeoColors.Black, 2);
            //cityLimits.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.DrawingLevel = DrawingLevel.LevelFour;
            //cityLimits.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            //// Project cityLimits layer to Spherical Mercator to match the mapLayer projection
            //cityLimits.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

            ///************************
            // * Create streets layer *
            // ************************/
            //var streets = new ShapeFileFeatureLayer(@"../../../Data/Shapefile/Streets.shp");

            //// Style streets layer
            //var majorStreetsStyle = new FilterStyle();
            //majorStreetsStyle.Conditions.Add(new FilterCondition("SUBTYPE", "<= 6"));
            //majorStreetsStyle.Styles.Add(LineStyle.CreateSimpleLineStyle(GeoColors.DarkGray, 1, true));
            //streets.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(majorStreetsStyle);
            //streets.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            //// Project streets layer to Spherical Mercator to match the mapLayer projection
            //streets.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

            ///**********************
            // * Create parks layer *
            // **********************/
            //var parks = new ShapeFileFeatureLayer(@"../../../Data/Shapefile/Parks.shp");

            //// Style parks layer
            //parks.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColors.LightGray, GeoColors.Transparent);
            //parks.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            //// Project parks layer to Spherical Mercator to match the mapLayer projection
            //parks.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

            ///*************************
            // * Create mosquitoSightings layer *
            // *************************/
            //var mosquitoSightings = new ShapeFileFeatureLayer(@"../../../Data/Shapefile/Frisco_Mosquitos.shp");

            //// Style parks layer
            //var mayFifthSightings = new FilterStyle();
            //mayFifthSightings.Conditions.Add(new FilterCondition("DateCollec", "20200505"));
            //mayFifthSightings.Styles.Add(PointStyle.CreateSimpleCircleStyle(GeoColors.Black, 10, GeoColors.White, 2));
            //mayFifthSightings.Styles.Add(TextStyle.CreateSimpleTextStyle("TrapSite", "Verdana", 8, DrawingFontStyles.Bold, GeoColors.Black, GeoColors.White, 2, 0, 6));
            //mosquitoSightings.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(mayFifthSightings);
            //mosquitoSightings.ZoomLevelSet.ZoomLevel01.DefaultPointStyle.DrawingLevel = DrawingLevel.LevelFour;
            //mosquitoSightings.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            //// Project parks layer to Spherical Mercator to match the mapLayer projection
            //mosquitoSightings.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);

            ///********************************
            // * Create mapPrinterLayer layer *
            // *******************************/
            //// Open cityLimits layer to get the bounding box for the mapLayer
            //cityLimits.Open();
            //var mapExtent = cityLimits.GetBoundingBox();
            //cityLimits.Close();

            //// Create the mapPrinterLayer, adding the FeatureLayers that we want to print
            //var mapPrinterLayer = new MapPrinterLayer(new Layer[] { cityLimits, streets, parks, mosquitoSightings }, mapExtent, GeographyUnit.Meter);

            //// Set the position of the map using the pageLayer's centerPoint
            //var pageCenter = pageLayer.GetPosition().GetCenterPoint();
            //mapPrinterLayer.SetPosition(7.5, 5, pageCenter.X, pageCenter.Y + 1.75, PrintingUnit.Inch);

            //printerOverlay.PrinterLayers.Add(mapPrinterLayer);
        }

        /// <summary>
        /// Creates a DataGridLayer containing mosquito trap data collected on 5/5/2020
        /// </summary>
        private void AddMosquitoDataGrid()
        {
            //PrinterInteractiveOverlay printerOverlay = (PrinterInteractiveOverlay)mapView.InteractiveOverlays["printerOverlay"];
            //PagePrinterLayer pageLayer = (PagePrinterLayer)printerOverlay.PrinterLayers["pageLayer"];

            //// Create a table with columns
            //var table = new DataTable();
            //table.Columns.Add("TRAP SITE");
            //table.Columns.Add("ADDRESS");
            //table.Columns.Add("MALES");
            //table.Columns.Add("FEMALES");
            //table.Columns.Add("WEST NILE DETECTED");

            //// Load in the mosquito feature source data
            //var mosquitoFeatureSource = new ShapeFileFeatureSource(@"../../../Data/Shapefile/Frisco_Mosquitos.shp");

            //// Query the data for all features whose data was collected on 5/5/2020
            //mosquitoFeatureSource.Open();
            //var trapSites = mosquitoFeatureSource.GetFeaturesByColumnValue("DateCollec", "20200505", ReturningColumnsType.AllColumns);
            //mosquitoFeatureSource.Close();

            //// Add a row to the table for every feature
            //foreach (var trapSite in trapSites)
            //{
            //    table.Rows.Add(trapSite.ColumnValues["TrapSite"], trapSite.ColumnValues["Address"], trapSite.ColumnValues["Male"], trapSite.ColumnValues["Female"], trapSite.ColumnValues["WestNileMo"]);
            //}

            //// Create the dataGridLayer that will display the mosquito data
            //var dataGridLayer = new DataGridPrinterLayer(table, new GeoFont("Verdana", 8), new GeoFont("Verdana", 8, DrawingFontStyles.Bold));

            //// Set the position of the map using the pageLayer's centerPoint
            //var pageCenter = pageLayer.GetPosition().GetCenterPoint();
            //dataGridLayer.SetPosition(7.5, 4, pageCenter.X, pageCenter.Y - 3, PrintingUnit.Inch);

            //// Add the dataGridLayer to the the PrinterLayers collection to print later
            //printerOverlay.PrinterLayers.Add(dataGridLayer);
        }
    }
}