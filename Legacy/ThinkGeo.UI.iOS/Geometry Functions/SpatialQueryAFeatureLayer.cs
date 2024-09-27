using CoreGraphics;
using System;
using System.Collections.ObjectModel;
using ThinkGeo.Core;
using UIKit;

namespace ThinkGeo.UI.iOS.HowDoI
{
    public class SpatialQueryAFeatureLayer : BaseViewController
    {
        public SpatialQueryAFeatureLayer()
            : base()
        { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            MapView.MapUnit = GeographyUnit.DecimalDegree;
            MapView.CurrentExtent = new RectangleShape(-133.2515625, 89.2484375, 126.9046875, -88.290625);

            LayerOverlay layerOverlay = new LayerOverlay();
            MapView.Overlays.Add("WorldOverlay", layerOverlay);

            ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer("AppData/SampleData/Countries02.shp");
            worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 233, 232, 214), GeoColor.FromArgb(255, 118, 138, 69));
            worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            layerOverlay.Layers.Add("WorldLayer", worldLayer);

            InMemoryFeatureLayer rectangleLayer = new InMemoryFeatureLayer();
            rectangleLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = new AreaStyle(new GeoSolidBrush(new GeoColor(50, 100, 100, 200)));
            rectangleLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Color = GeoColors.DarkBlue;
            rectangleLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            rectangleLayer.InternalFeatures.Add("Rectangle", new Feature("POLYGON((-50 -20,-50 20,50 20,50 -20,-50 -20))", "Rectangle"));

            InMemoryFeatureLayer spatialQueryResultLayer = new InMemoryFeatureLayer();
            spatialQueryResultLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = new AreaStyle(new GeoSolidBrush(GeoColor.FromArgb(200, GeoColors.PastelRed)));
            spatialQueryResultLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Color = GeoColors.Red;
            spatialQueryResultLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            LayerOverlay spatialQueryResultOverlay = new LayerOverlay();
            spatialQueryResultOverlay.TileType = TileType.SingleTile;
            spatialQueryResultOverlay.Layers.Add("RectangleLayer", rectangleLayer);
            spatialQueryResultOverlay.Layers.Add("SpatialQueryResultLayer", spatialQueryResultLayer);
            MapView.Overlays.Add("SpatialQueryResultOverlay", spatialQueryResultOverlay);

            MapView.Refresh();
        }

        private void button_TouchDown(object sender, EventArgs e)
        {
            LayerOverlay worldOverlay = (LayerOverlay)MapView.Overlays["WorldOverlay"];
            FeatureLayer worldLayer = (FeatureLayer)worldOverlay.Layers["WorldLayer"];

            LayerOverlay spatialQueryResultOverlay = (LayerOverlay)MapView.Overlays["SpatialQueryResultOverlay"];
            InMemoryFeatureLayer rectangleLayer = (InMemoryFeatureLayer)spatialQueryResultOverlay.Layers["RectangleLayer"];
            InMemoryFeatureLayer spatialQueryResultLayer = (InMemoryFeatureLayer)spatialQueryResultOverlay.Layers["SpatialQueryResultLayer"];

            Feature rectangleFeature = rectangleLayer.InternalFeatures["Rectangle"];
            Collection<Feature> spatialQueryResults;

            UIButton button = (UIButton)sender;
            worldLayer.Open();
            switch (button.Title(UIControlState.Normal))
            {
                case "Within":
                    spatialQueryResults = worldLayer.QueryTools.GetFeaturesWithin(rectangleFeature, ReturningColumnsType.NoColumns);
                    break;

                case "Containing":
                    spatialQueryResults = worldLayer.QueryTools.GetFeaturesContaining(rectangleFeature, ReturningColumnsType.NoColumns);
                    break;

                case "Disjointed":
                    spatialQueryResults = worldLayer.QueryTools.GetFeaturesDisjointed(rectangleFeature, ReturningColumnsType.NoColumns);
                    break;

                case "Intersecting":
                    spatialQueryResults = worldLayer.QueryTools.GetFeaturesIntersecting(rectangleFeature, ReturningColumnsType.NoColumns);
                    break;

                case "Overlapping":
                    spatialQueryResults = worldLayer.QueryTools.GetFeaturesOverlapping(rectangleFeature, ReturningColumnsType.NoColumns);
                    break;

                case "TopologicalEqual":
                    spatialQueryResults = worldLayer.QueryTools.GetFeaturesTopologicalEqual(rectangleFeature, ReturningColumnsType.NoColumns);
                    break;

                case "Touching":
                    spatialQueryResults = worldLayer.QueryTools.GetFeaturesTouching(rectangleFeature, ReturningColumnsType.NoColumns);
                    break;

                default:
                    spatialQueryResults = worldLayer.QueryTools.GetFeaturesWithin(rectangleFeature, ReturningColumnsType.NoColumns);
                    break;
            }

            spatialQueryResultLayer.InternalFeatures.Clear();
            foreach (Feature feature in spatialQueryResults)
            {
                spatialQueryResultLayer.InternalFeatures.Add(feature.Id, feature);
            }

            MapView.Overlays["SpatialQueryResultOverlay"].Refresh();
        }

        protected override void InitializeInstruction()
        {
            SampleUIHelper.InitializeInstruction(MapView, View, GetType(), isIphone => isIphone ? 240 : 160, contentView =>
            {
                int withinLeft = 0;
                int containingLeft = 140;
                int disjointedLeft = 280;
                int intersectingLeft = 420;
                int overlappingLeft = 560;
                int topologicalLeft = 0;
                int touchingLeft = 140;

                int withinTop = 0;
                int containingTop = 0;
                int disjointedTop = 0;
                int intersectingTop = 0;
                int overlappingTop = 0;
                int topologicalTop = 40;
                int touchingTop = 40;

                if (SampleUIHelper.IsOnIPhone)
                {
                    disjointedLeft = 0;
                    disjointedTop = 40;
                    intersectingLeft = 140;
                    intersectingTop = 40;

                    overlappingLeft = 0;
                    overlappingTop = 80;
                    topologicalLeft = 140;
                    topologicalTop = 80;

                    touchingLeft = 0;
                    touchingTop = 120;
                }

                UIButton withinButton = GetUIButton("Within", withinLeft, withinTop);
                UIButton containingButton = GetUIButton("Containing", containingLeft, containingTop);
                UIButton disjointedButton = GetUIButton("Disjointed", disjointedLeft, disjointedTop);
                UIButton intersectingButton = GetUIButton("Intersecting", intersectingLeft, intersectingTop);
                UIButton overlappingButton = GetUIButton("Overlapping", overlappingLeft, overlappingTop);
                UIButton topologicalEqualButton = GetUIButton("TopologicalEqual", topologicalLeft, topologicalTop);
                UIButton touchingButton = GetUIButton("Touching", touchingLeft, touchingTop);

                contentView.AddSubviews(new UIView[] { withinButton, containingButton, disjointedButton, intersectingButton, overlappingButton, topologicalEqualButton, touchingButton });
            });
        }

        private UIButton GetUIButton(string title, int leftLocation, int topLocation)
        {
            UIButton button = UIButton.FromType(UIButtonType.RoundedRect);
            button.Frame = new CGRect(leftLocation, topLocation, 130, 30);
            button.BackgroundColor = UIColor.FromRGB(241, 241, 241);
            button.TouchUpInside += button_TouchDown;
            button.SetTitle(title, UIControlState.Normal);

            return button;
        }
    }
}