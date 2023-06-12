using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using ThinkGeo.Core;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    /// <summary>
    ///     Learn to draw, edit, or delete shapes using the map's TrackOverlay and EditOverlay.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditWithSnappingSample
    {
        public EditWithSnappingSample()
        {
            InitializeComponent();
        }

        private readonly float tolerance = 25;
        private InMemoryFeatureLayer pointsLayer;

        /// <summary>
        ///     Setup the map with the ThinkGeo Cloud Maps overlay to show a basic map
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();


            // Set the map's unit of measurement to meters(Spherical Mercator)
            mapView.MapUnit = GeographyUnit.Meter;

            // Add Cloud Maps as a background overlay
            var backgroundOverlay = new ThinkGeoCloudVectorMapsOverlay(
                "9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~",
                "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~", ThinkGeoCloudVectorMapsMapType.Light);
            backgroundOverlay.TileCache = new FileRasterTileCache(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ThinkGeoLightBackground");
            mapView.Overlays.Add("Background Maps", backgroundOverlay);

            mapView.CurrentExtent = new RectangleShape(-10783147, 3917677, -10782596, 3917271);

            var parksLayer = new ShapeFileFeatureLayer(Path.Combine(
                  Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Data/Shapefile/Parks.shp"));
            // Project the layer's data from its native projection (2276) to spherical mercator(3857)
            parksLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);
            parksLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = new AreaStyle(new GeoPen(GeoColors.Transparent, 0), new GeoSolidBrush(new GeoColor(255, 211, 226, 190)));
            parksLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            parksLayer.Open();
            Collection<Feature> features = parksLayer.QueryTools.GetAllFeatures(ReturningColumnsType.NoColumns);

            pointsLayer = new InMemoryFeatureLayer();
            pointsLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(new TolerancePointStyle(tolerance));
            pointsLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            foreach (Feature feature in features)
            {
                MultipolygonShape multipolygonShape = (MultipolygonShape)feature.GetShape();
                foreach (PolygonShape polygonShape in multipolygonShape.Polygons)
                {
                    foreach (Vertex vertex in polygonShape.OuterRing.Vertices)
                    {
                        pointsLayer.InternalFeatures.Add(new Feature(vertex));
                    }
                }
            }
            pointsLayer.BuildIndex();

            LayerOverlay inMemoryOverlay = new LayerOverlay();
            inMemoryOverlay.Layers.Add(parksLayer);
            inMemoryOverlay.Layers.Add(pointsLayer);
            mapView.Overlays.Add(inMemoryOverlay);

            mapView.EditOverlay.VertexMoving += SnapToLayerEditInteractiveOverlay_VertexMoving;

            LineShape lineShape = new LineShape();
            lineShape.Vertices.Add(new Vertex(-10782998.3264, 3917614.8534));
            lineShape.Vertices.Add(new Vertex(-10783038.5478, 3917484.6087));
            lineShape.Vertices.Add(new Vertex(-10783037.7018, 3917327.7685));
            mapView.EditOverlay.EditShapesLayer.InternalFeatures.Add(new Feature(lineShape));

            mapView.EditOverlay.CalculateAllControlPoints();

            await mapView.RefreshAsync();
        }

        private void SnapToLayerEditInteractiveOverlay_VertexMoving(object sender, VertexMovingEditInteractiveOverlayEventArgs e)
        {
            var toSnapInMemoryFeatures = pointsLayer.QueryTools.GetFeaturesNearestTo(e.TargetVertex, GeographyUnit.Meter, 1, ReturningColumnsType.AllColumns);

            if (toSnapInMemoryFeatures.Count == 0)
                return;

            PointShape toSnapPointShape = toSnapInMemoryFeatures[0].GetShape() as PointShape;
            float screenDistance = MapUtil.GetScreenDistanceBetweenTwoWorldPoints(mapView.CurrentExtent, toSnapPointShape, e.TargetVertex, (float)mapView.Width, (float)mapView.Height);

            if (screenDistance < tolerance)
            {
                e.TargetVertex.X = toSnapPointShape.X;
                e.TargetVertex.Y = toSnapPointShape.Y;
            }
        }
    }

    class TolerancePointStyle : PointStyle
    {
        public TolerancePointStyle(float tolerance)
        {
            Tolerance = tolerance;
        }

        public float Tolerance { get; set; }

        protected override void DrawCore(IEnumerable<Feature> features, GeoCanvas canvas, Collection<SimpleCandidate> labelsInThisLayer, Collection<SimpleCandidate> labelsInAllLayers)
        {
            foreach (Feature feature in features)
            {
                //Draws the vertex.
                PointShape pointShape = (PointShape)feature.GetShape();
                canvas.DrawEllipse(pointShape, 5, 5, new GeoSolidBrush(GeoColors.Black), DrawingLevel.LevelOne);

                //Draws the tolerance circle.
                ScreenPointF screenPointF = MapUtil.ToScreenCoordinate(canvas.CurrentWorldExtent, pointShape, canvas.Width, canvas.Height);
                canvas.DrawEllipse(screenPointF, Tolerance * 2, Tolerance * 2, new GeoPen(GeoColors.Black), new GeoSolidBrush(), DrawingLevel.LevelFour, 0, 0, PenBrushDrawingOrder.PenFirst);
            }
        }
    }
}