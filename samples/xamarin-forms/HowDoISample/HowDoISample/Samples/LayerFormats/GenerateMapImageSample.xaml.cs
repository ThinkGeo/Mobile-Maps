﻿using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using ThinkGeo.Core;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GenerateMapImageSample : ContentPage
    {
        public GenerateMapImageSample()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     ...
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            loadingIndicator.IsRunning = true;
            loadingLayout.IsVisible = true;

            MapImage.Source = await Task.Run(GenerateMapImage);

            loadingIndicator.IsRunning = false;
            loadingLayout.IsVisible = false;
        }

        private ImageSource GenerateMapImage()
        {
            var layersToDraw = new Collection<Layer>();

            // Create the background world maps using vector tiles stored locally in our MBTiles file and also set the styling though a json file
            var mbTilesLayer = new ThinkGeoMBTilesLayer(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "Data/Mbtiles/Frisco.mbtiles"),
                new Uri(
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                        "Data/Json/thinkgeo-world-streets-light.json"), UriKind.Relative));
            mbTilesLayer.Open();
            layersToDraw.Add(mbTilesLayer);

            // Create the new layer and set the projection as the data is in srid 2276 and our background is srid 3857 (spherical mercator).
            var zoningLayer = new ShapeFileFeatureLayer(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Data/Shapefile/Zoning.shp"));
            zoningLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);
            zoningLayer.Open();

            // Create an Area style on zoom level 1 and then apply it to all zoom levels up to 20.
            zoningLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = new AreaStyle(new GeoPen(GeoBrushes.Blue));
            zoningLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            layersToDraw.Add(zoningLayer);

            // Create a GeoCanvas to do the drawing
            var canvas = GeoCanvas.CreateDefaultGeoCanvas();

            // Create a GeoImage as the image to draw on
            var geoImage = new GeoImage(800, 600);

            // Start the drawing by specifying the image, extent and map units
            canvas.BeginDrawing(geoImage, MapUtil.GetDrawingExtent(zoningLayer.GetBoundingBox(), 800, 600),
                GeographyUnit.Meter);

            // This collection is used during drawing to pass labels in between layers so we can track collisions
            var labels = new Collection<SimpleCandidate>();

            // Loop through all the layers and draw them to the GeoCanvas
            // The flush is to compact styles that use different drawing levels
            foreach (var layer in layersToDraw)
            {
                layer.Draw(canvas, labels);
                canvas.Flush();
            }

            // End drawing, we can now use the GeoImage
            canvas.EndDrawing();

            // Create a memory stream and save the GeoImage as a standard PNG formatted image
            var imageStream = new MemoryStream();
            geoImage.Save(imageStream, GeoImageFormat.Png);

            // Reset the image stream back to the beginning
            imageStream.Position = 0;

            return ImageSource.FromStream(() => { return imageStream; });
        }
    }
}