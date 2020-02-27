/*===========================================
    Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    a Client ID and Secret. These were sent to you via email when you signed up
    with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Android;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;

namespace MapSuiteSiteSelection
{
    public class SampleMapView : MapView
    {
        private static SampleMapView current;
        private BaseMapType baseMapType;
        private FilterConfiguration filterConfiguration;

        public static SampleMapView Current
        {
            get { return current ?? (current = new SampleMapView(Application.Context)); }
        }

        protected SampleMapView(Context context)
            : base(context)
        {
            baseMapType = BaseMapType.ThinkGeoCloudMapLight;
            filterConfiguration = new FilterConfiguration();

            LoadOverlays();
        }

        public BaseMapType BaseMapType
        {
            get { return baseMapType; }
            set { baseMapType = value; }
        }

        public FilterConfiguration FilterConfiguration
        {
            get { return filterConfiguration; }
            set { filterConfiguration = value; }
        }

        public void SwitchBaseMapTo(BaseMapType baseMapType)
        {
            ThinkGeoCloudRasterMapsOverlay thinkGeoCloudMapsOverlay = FindOverlay<ThinkGeoCloudRasterMapsOverlay>(OverlayKey.ThinkGeoCloudMapsOverlay);
            OpenStreetMapOverlay openStreetMapOverlay = FindOverlay<OpenStreetMapOverlay>(OverlayKey.OpenStreetMapOverlay);
            BingMapsOverlay bingMapsAerialOverlay = FindOverlay<BingMapsOverlay>(OverlayKey.BingMapsAerialOverlay);
            BingMapsOverlay bingMapsRoadOverlay = FindOverlay<BingMapsOverlay>(OverlayKey.BingMapsRoadOverlay);

            thinkGeoCloudMapsOverlay.IsVisible = baseMapType == BaseMapType.ThinkGeoCloudMapAerial ||
                baseMapType == BaseMapType.ThinkGeoCloudMapLight || baseMapType == BaseMapType.ThinkGeoCloudMapHybrid;
            openStreetMapOverlay.IsVisible = baseMapType == BaseMapType.OpenStreetMap;
            bingMapsAerialOverlay.IsVisible = baseMapType == BaseMapType.BingMapsAerial;
            bingMapsRoadOverlay.IsVisible = baseMapType == BaseMapType.BingMapsRoad;

            switch (baseMapType)
            {
                case BaseMapType.ThinkGeoCloudMapAerial:
                    thinkGeoCloudMapsOverlay.MapType = ThinkGeoCloudRasterMapsMapType.Aerial;
                    break;
                case BaseMapType.ThinkGeoCloudMapHybrid:
                    thinkGeoCloudMapsOverlay.MapType = ThinkGeoCloudRasterMapsMapType.Hybrid;
                    break;
                case BaseMapType.ThinkGeoCloudMapLight:
                    thinkGeoCloudMapsOverlay.MapType = ThinkGeoCloudRasterMapsMapType.Light;
                    break;
                default:
                    break;
            }

            BaseMapType = baseMapType;
            Refresh();
        }

        public T FindOverlay<T>(string overlayKey) where T : Overlay
        {
            return (T)Overlays[overlayKey];
        }

        public T FindFeatureLayer<T>(string layerKey) where T : FeatureLayer
        {
            foreach (var overlay in Overlays.OfType<LayerOverlay>())
            {
                if (overlay.Layers.Contains(layerKey))
                {
                    return (T)overlay.Layers[layerKey];
                }
            }

            return null;
        }

        public void ClearQueryResult()
        {
            LayerOverlay highlightOverlay = FindOverlay<LayerOverlay>(OverlayKey.HighlightOverlay);

            InMemoryFeatureLayer highlightAreaLayer = FindFeatureLayer<InMemoryFeatureLayer>(LayerKey.HighlightAreaLayer);
            InMemoryFeatureLayer highlightMarkerLayer = FindFeatureLayer<InMemoryFeatureLayer>(LayerKey.HighlightMarkerLayer);
            InMemoryFeatureLayer highlightCenterMarkerLayer = FindFeatureLayer<InMemoryFeatureLayer>(LayerKey.HighlightCenterMarkerLayer);

            highlightAreaLayer.InternalFeatures.Clear();
            highlightMarkerLayer.InternalFeatures.Clear();
            highlightCenterMarkerLayer.InternalFeatures.Clear();

            highlightOverlay.Refresh();
        }

        public void UpdateHighlightOverlay()
        {
            LayerOverlay highlightOverlay = FindOverlay<LayerOverlay>(OverlayKey.HighlightOverlay);

            InMemoryFeatureLayer highlightAreaLayer = FindFeatureLayer<InMemoryFeatureLayer>(LayerKey.HighlightAreaLayer);
            InMemoryFeatureLayer highlightMarkerLayer = FindFeatureLayer<InMemoryFeatureLayer>(LayerKey.HighlightMarkerLayer);
            InMemoryFeatureLayer highlightCenterMarkerLayer = FindFeatureLayer<InMemoryFeatureLayer>(LayerKey.HighlightCenterMarkerLayer);

            if (highlightCenterMarkerLayer.InternalFeatures.Count > 0)
            {
                highlightAreaLayer.InternalFeatures.Clear();
                highlightMarkerLayer.InternalFeatures.Clear();

                MultipolygonShape bufferResultShape = highlightCenterMarkerLayer.InternalFeatures[0].GetShape().Buffer(FilterConfiguration.BufferValue, MapUnit, FilterConfiguration.BufferDistanceUnit);
                FilterConfiguration.QueryFeatureLayer.Open();
                Collection<Feature> filterResultFeatures = FilterConfiguration.QueryFeatureLayer.FeatureSource.GetFeaturesWithinDistanceOf(bufferResultShape, MapUnit, DistanceUnit.Meter, 0.1, ReturningColumnsType.AllColumns);

                highlightAreaLayer.InternalFeatures.Add(new Feature(bufferResultShape));
                foreach (Feature feature in FilterFeaturesByColumnValue(filterResultFeatures))
                {
                    highlightMarkerLayer.InternalFeatures.Add(feature);
                }

                highlightOverlay.Refresh();
            }
        }

        private Collection<Feature> FilterFeaturesByColumnValue(Collection<Feature> filterResultFeatures)
        {
            Collection<Feature> resultFeatures = new Collection<Feature>();

            foreach (Feature feature in filterResultFeatures)
            {
                if (filterConfiguration.QueryColumnValue.Equals(SettingKey.AllFeature))
                {
                    resultFeatures.Add(feature);
                }
                else if (filterConfiguration.QueryColumnValue.Equals(feature.ColumnValues[filterConfiguration.QueryColumnName]))
                {
                    resultFeatures.Add(feature);
                }
                else if (filterConfiguration.QueryFeatureLayer.Name.Equals(LayerKey.HotelsLayer))
                {
                    string[] values = filterConfiguration.QueryColumnValue.Split('~');
                    double min = 0, max = 0, value = 0;
                    if (double.TryParse(values[0], out min) && double.TryParse(values[1], out max)
                        && double.TryParse(feature.ColumnValues[filterConfiguration.QueryColumnName], out value))
                    {
                        if (min < value && value < max)
                        {
                            resultFeatures.Add(feature);
                        }
                    }
                }
            }

            return resultFeatures;
        }

        private void TrackOverlay_TrackEnded(object sender, TrackEndedTrackInteractiveOverlayEventArgs e)
        {
            if (TrackOverlay.TrackShapeLayer.InternalFeatures.Count > 0)
            {
                Feature centerFeature = TrackOverlay.TrackShapeLayer.InternalFeatures[0];
                TrackOverlay.TrackShapeLayer.InternalFeatures.Clear();

                InMemoryFeatureLayer highlightCenterMarkerLayer = FindFeatureLayer<InMemoryFeatureLayer>(LayerKey.HighlightCenterMarkerLayer);

                highlightCenterMarkerLayer.InternalFeatures.Clear();
                highlightCenterMarkerLayer.InternalFeatures.Add(centerFeature);

                UpdateHighlightOverlay();
            }
        }

        private void LoadOverlays()
        {
            //Highlight Overlay
            GeoImage pinImage = GetGeoImageFromImageId(Resource.Drawable.drawPoint);
            InMemoryFeatureLayer highlightCenterMarkerLayer = new InMemoryFeatureLayer();
            highlightCenterMarkerLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = new PointStyle(pinImage);
            highlightCenterMarkerLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle.YOffsetInPixel = -(pinImage.Height / 2f);
            highlightCenterMarkerLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            InMemoryFeatureLayer highlightMarkerLayer = new InMemoryFeatureLayer();
            highlightMarkerLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = new PointStyle(GetGeoImageFromImageId(Resource.Drawable.selectedHalo));
            highlightMarkerLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            InMemoryFeatureLayer highlightAreaLayer = new InMemoryFeatureLayer();
            highlightAreaLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(new GeoColor(120, GeoColor.FromHtml("#1749c9")), GeoColor.FromHtml("#fefec1"), 3, LineDashStyle.Solid);
            highlightAreaLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            //LimitPolygon
            ShapeFileFeatureLayer limitPolygonLayer = new ShapeFileFeatureLayer(SampleHelper.GetDataPath("CityLimitPolygon.shp"));
            limitPolygonLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(new AreaStyle(new GeoPen(GeoColor.SimpleColors.White, 5.5f), new GeoSolidBrush(GeoColor.SimpleColors.Transparent)));
            limitPolygonLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(new AreaStyle(new GeoPen(GeoColor.SimpleColors.Red, 1.5f) { DashStyle = LineDashStyle.Dash }, new GeoSolidBrush(GeoColor.SimpleColors.Transparent)));
            limitPolygonLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            limitPolygonLayer.FeatureSource.Projection = GetWgs84ToMercatorProjection();

            // Poi Overlay
            ShapeFileFeatureLayer hotelsLayer = new ShapeFileFeatureLayer(SampleHelper.GetDataPath("POIs", "Hotels.shp"));
            hotelsLayer.Name = LayerKey.HotelsLayer;
            hotelsLayer.Transparency = 120f;
            hotelsLayer.ZoomLevelSet.ZoomLevel10.DefaultPointStyle = new PointStyle(GetGeoImageFromImageId(Resource.Drawable.Hotel));
            hotelsLayer.ZoomLevelSet.ZoomLevel10.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            hotelsLayer.FeatureSource.Projection = GetWgs84ToMercatorProjection();

            ShapeFileFeatureLayer medicalFacilitesLayer = new ShapeFileFeatureLayer(SampleHelper.GetDataPath("POIs", "Medical_Facilities.shp"));
            medicalFacilitesLayer.Name = LayerKey.MedicalFacilitiesLayer;
            medicalFacilitesLayer.Transparency = 120f;
            medicalFacilitesLayer.ZoomLevelSet.ZoomLevel10.DefaultPointStyle = new PointStyle(GetGeoImageFromImageId(Resource.Drawable.DrugStore));
            medicalFacilitesLayer.ZoomLevelSet.ZoomLevel10.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            medicalFacilitesLayer.FeatureSource.Projection = GetWgs84ToMercatorProjection(); ;

            ShapeFileFeatureLayer publicFacilitesLayer = new ShapeFileFeatureLayer(SampleHelper.GetDataPath("POIs", "Public_Facilities.shp"));
            publicFacilitesLayer.Name = LayerKey.PublicFacilitiesLayer;
            publicFacilitesLayer.Transparency = 120f;
            publicFacilitesLayer.ZoomLevelSet.ZoomLevel10.DefaultPointStyle = new PointStyle(GetGeoImageFromImageId(Resource.Drawable.public_facility));
            publicFacilitesLayer.ZoomLevelSet.ZoomLevel10.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            publicFacilitesLayer.FeatureSource.Projection = GetWgs84ToMercatorProjection();

            ShapeFileFeatureLayer restaurantsLayer = new ShapeFileFeatureLayer(SampleHelper.GetDataPath("POIs", "Restaurants.shp"));
            restaurantsLayer.Name = LayerKey.RestaurantsLayer;
            restaurantsLayer.Transparency = 120f;
            restaurantsLayer.ZoomLevelSet.ZoomLevel10.DefaultPointStyle = new PointStyle(GetGeoImageFromImageId(Resource.Drawable.restaurant));
            restaurantsLayer.ZoomLevelSet.ZoomLevel10.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            restaurantsLayer.FeatureSource.Projection = GetWgs84ToMercatorProjection();

            ShapeFileFeatureLayer schoolsLayer = new ShapeFileFeatureLayer(SampleHelper.GetDataPath("POIs", "Schools.shp"));
            schoolsLayer.Name = LayerKey.SchoolsLayer;
            schoolsLayer.Transparency = 120f;
            schoolsLayer.ZoomLevelSet.ZoomLevel10.DefaultPointStyle = new PointStyle(GetGeoImageFromImageId(Resource.Drawable.school));
            schoolsLayer.ZoomLevelSet.ZoomLevel10.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            schoolsLayer.FeatureSource.Projection = GetWgs84ToMercatorProjection();

            LayerOverlay highlightOverlay = new LayerOverlay();
            highlightOverlay.TileType = TileType.SingleTile;
            highlightOverlay.Layers.Add(limitPolygonLayer);
            highlightOverlay.Layers.Add(LayerKey.HotelsLayer, hotelsLayer);
            highlightOverlay.Layers.Add(LayerKey.MedicalFacilitiesLayer, medicalFacilitesLayer);
            highlightOverlay.Layers.Add(LayerKey.PublicFacilitiesLayer, publicFacilitesLayer);
            highlightOverlay.Layers.Add(LayerKey.RestaurantsLayer, restaurantsLayer);
            highlightOverlay.Layers.Add(LayerKey.SchoolsLayer, schoolsLayer);
            highlightOverlay.Layers.Add(LayerKey.HighlightAreaLayer, highlightAreaLayer);
            highlightOverlay.Layers.Add(LayerKey.HighlightMarkerLayer, highlightMarkerLayer);
            highlightOverlay.Layers.Add(LayerKey.HighlightCenterMarkerLayer, highlightCenterMarkerLayer);

            string baseFolder = Environment.ExternalStorageDirectory.AbsolutePath;
            string cachePathFilename = System.IO.Path.Combine(baseFolder, "MapSuiteTileCaches/SampleCaches.db");
            // OSM
            OpenStreetMapOverlay osmOverlay = new OpenStreetMapOverlay();
            osmOverlay.TileCache = new SqliteBitmapTileCache(cachePathFilename, "OSMSphericalMercator");
            osmOverlay.IsVisible = false;
            
            // Please input your ThinkGeo Cloud Client ID / Client Secret to enable the background map. 
            ThinkGeoCloudRasterMapsOverlay thinkGeoCloudMapsOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud Client ID", "ThinkGeo Cloud Client Secret");

            // Bing - Aerial
            BingMapsOverlay bingMapsAerialOverlay = new BingMapsOverlay();
            bingMapsAerialOverlay.IsVisible = false;
            bingMapsAerialOverlay.MapType = BingMapsMapType.AerialWithLabels;
            bingMapsAerialOverlay.TileCache = new SqliteBitmapTileCache(cachePathFilename, "BingAerialWithLabels");

            // Bing - Road
            BingMapsOverlay bingMapsRoadOverlay = new BingMapsOverlay();
            bingMapsRoadOverlay.IsVisible = false;
            bingMapsRoadOverlay.MapType = BingMapsMapType.Road;
            bingMapsRoadOverlay.TileCache = new SqliteBitmapTileCache(cachePathFilename, "BingRoad");

            //Maps
            SetBackgroundColor(Color.Argb(255, 244, 242, 238));
            MapUnit = GeographyUnit.Meter;
            ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            MapTools.ZoomMapTool.Visibility = ViewStates.Invisible;
            CurrentExtent = new RectangleShape(-10789390.0630888, 3924457.19413373, -10768237.5787263, 3906066.41190523);

            Overlays.Add(OverlayKey.OpenStreetMapOverlay, osmOverlay);
            Overlays.Add(OverlayKey.ThinkGeoCloudMapsOverlay, thinkGeoCloudMapsOverlay);
            Overlays.Add(OverlayKey.BingMapsAerialOverlay, bingMapsAerialOverlay);
            Overlays.Add(OverlayKey.BingMapsRoadOverlay, bingMapsRoadOverlay);
            Overlays.Add(OverlayKey.HighlightOverlay, highlightOverlay);

            FilterConfiguration.QueryFeatureLayer = hotelsLayer;
            TrackOverlay.TrackEnded += TrackOverlay_TrackEnded;
        }

        private Proj4Projection GetWgs84ToMercatorProjection()
        {
            Proj4Projection wgs84ToMercatorProjection = new Proj4Projection();
            wgs84ToMercatorProjection.InternalProjectionParametersString = Proj4Projection.GetWgs84ParametersString();
            wgs84ToMercatorProjection.ExternalProjectionParametersString = Proj4Projection.GetBingMapParametersString();
            wgs84ToMercatorProjection.Open();
            return wgs84ToMercatorProjection;
        }

        private GeoImage GetGeoImageFromImageId(int imageId)
        {
            MemoryStream ms = new MemoryStream();
            BitmapFactory.DecodeResource(this.Resources, imageId).Compress(Bitmap.CompressFormat.Png, 100, ms);
            ms.Seek(0, SeekOrigin.Begin);
            return new GeoImage(ms);
        }
    }

    public static class OverlayKey
    {
        public const string OpenStreetMapOverlay = "OpenStreetMapOverlay";
        public const string BingMapsAerialOverlay = "BingMapsAerialOverlay";
        public const string ThinkGeoCloudMapsOverlay = "ThinkGeoCloudMapsOverlay";
        public const string BingMapsRoadOverlay = "BingMapsRoadOverlay";
        public const string HighlightOverlay = "HighlightOverlay";
    }

    public static class LayerKey
    {
        public const string HighlightAreaLayer = "HighlightAreaLayer";
        public const string HighlightMarkerLayer = "HighlightMarkerLayer";
        public const string HighlightCenterMarkerLayer = "HighlightCenterMarkerLayer";
        public const string SchoolsLayer = "SchoolsLayer";
        public const string RestaurantsLayer = "RestaurantsLayer";
        public const string PublicFacilitiesLayer = "PublicFacilitiesLayer";
        public const string MedicalFacilitiesLayer = "MedicalFacilitiesLayer";
        public const string HotelsLayer = "HotelsLayer";
    }

    public static class SettingKey
    {
        public const string AllFeature = "All";
        public const string PrefsBingMapKey = "BingMapKey";
        public const string PrefsFile = "SamplePrefsFile";
    }
}