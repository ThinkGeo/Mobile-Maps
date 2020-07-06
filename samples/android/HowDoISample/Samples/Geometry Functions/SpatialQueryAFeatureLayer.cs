using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.ObjectModel;
using ThinkGeo.Core;

namespace ThinkGeo.UI.Android.HowDoI
{ 
    public class SpatialQueryAFeatureLayer : SampleFragment
    {
        private MapView mapView;

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnStart();

            ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer(SampleHelper.GetDataPath(@"SampleData/Countries02.shp"));
            worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 233, 232, 214), GeoColor.FromArgb(255, 118, 138, 69));
            worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            InMemoryFeatureLayer rectangleLayer = new InMemoryFeatureLayer();
            rectangleLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = new AreaStyle(new GeoSolidBrush(new GeoColor(50, 100, 100, 200)));
            rectangleLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Color = GeoColors.DarkBlue;
            rectangleLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            rectangleLayer.InternalFeatures.Add("Rectangle", new Feature("POLYGON((-50 -20,-50 20,50 20,50 -20,-50 -20))", "Rectangle"));

            InMemoryFeatureLayer spatialQueryResultLayer = new InMemoryFeatureLayer();
            spatialQueryResultLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = new AreaStyle(new GeoSolidBrush(GeoColor.FromArgb(200, GeoColors.PastelRed)));
            spatialQueryResultLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Color = GeoColors.Red;
            spatialQueryResultLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add("WorldLayer", worldLayer);

            LayerOverlay spatialQueryResultOverlay = new LayerOverlay();
            spatialQueryResultOverlay.TileType = TileType.SingleTile;
            spatialQueryResultOverlay.Layers.Add("RectangleLayer", rectangleLayer);
            spatialQueryResultOverlay.Layers.Add("SpatialQueryResultLayer", spatialQueryResultLayer);

            
            mapView.MapUnit = GeographyUnit.DecimalDegree;
            mapView.CurrentExtent = new RectangleShape(-133.2515625, 89.2484375, 126.9046875, -88.290625);
            mapView.Overlays.Add("WorldOverlay", layerOverlay);
            mapView.Overlays.Add("SpatialQueryResultOverlay", spatialQueryResultOverlay);

            Button withinButton = new Button(this.Context);
            withinButton.Text = "Within";
            withinButton.Click += button_TouchDown;

            Button disjointedButton = new Button(this.Context);
            disjointedButton.Text = "Disjointed";
            disjointedButton.Click += button_TouchDown;

            LinearLayout linearLayout = new LinearLayout(this.Context);
            linearLayout.Orientation = Orientation.Horizontal;
            linearLayout.AddView(withinButton);
            linearLayout.AddView(disjointedButton);

            SampleViewHelper.InitializeInstruction(this.Context, currentView.FindViewById<RelativeLayout>(Resource.Id.MainLayout), base.SampleInfo, new Collection<View>() { linearLayout });
        }

        private void button_TouchDown(object sender, EventArgs e)
        {
            LayerOverlay worldOverlay = (LayerOverlay)mapView.Overlays["WorldOverlay"];
            FeatureLayer worldLayer = (FeatureLayer)worldOverlay.Layers["WorldLayer"];

            LayerOverlay spatialQueryResultOverlay = (LayerOverlay)mapView.Overlays["SpatialQueryResultOverlay"];
            InMemoryFeatureLayer rectangleLayer = (InMemoryFeatureLayer)spatialQueryResultOverlay.Layers["RectangleLayer"];
            InMemoryFeatureLayer spatialQueryResultLayer = (InMemoryFeatureLayer)spatialQueryResultOverlay.Layers["SpatialQueryResultLayer"];

            Feature rectangleFeature = rectangleLayer.InternalFeatures["Rectangle"];
            Collection<Feature> spatialQueryResults;

            var button = sender as Button;
            worldLayer.Open();
            switch (button.Text)
            {
                case "Within":
                    spatialQueryResults = worldLayer.QueryTools.GetFeaturesWithin(rectangleFeature, new string[0]);
                    break;

                case "Disjointed":
                    spatialQueryResults = worldLayer.QueryTools.GetFeaturesDisjointed(rectangleFeature, new string[0]);
                    break;

                default:
                    spatialQueryResults = worldLayer.QueryTools.GetFeaturesWithin(rectangleFeature, new string[0]);
                    break;
            }
            worldLayer.Close();

            spatialQueryResultLayer.InternalFeatures.Clear();
            foreach (Feature feature in spatialQueryResults)
            {
                spatialQueryResultLayer.InternalFeatures.Add(feature.Id, feature);
            }

            mapView.Overlays["SpatialQueryResultOverlay"].Refresh();
        }
    }
}