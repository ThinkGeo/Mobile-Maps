﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
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
        /// ...
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            Collection<Layer> layersToDraw = new Collection<Layer>();

            // Create the background world maps using vector tiles stored locally in our MBTiles file and also set the styling though a json file
            ThinkGeoMBTilesLayer mbTilesLayer = new ThinkGeoMBTilesLayer(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Data/Mbtiles/Frisco.mbtiles"), new Uri(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Data/Json/thinkgeo-world-streets-light.json"), UriKind.Relative));
            mbTilesLayer.Open();
            layersToDraw.Add(mbTilesLayer);

            // Create the new layer and set the projection as the data is in srid 2276 and our background is srid 3857 (spherical mercator).
            ShapeFileFeatureLayer zoningLayer = new ShapeFileFeatureLayer(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Data/Shapefile/Zoning.shp"));
            zoningLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(2276, 3857);
            zoningLayer.Open();

            // Create an Area style on zoom level 1 and then apply it to all zoom levels up to 20.
            zoningLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = new AreaStyle(new GeoPen(GeoBrushes.Blue));
            zoningLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            layersToDraw.Add(zoningLayer);

            // Create a GeoCanvas to do the drawing
            GeoCanvas canvas = GeoCanvas.CreateDefaultGeoCanvas();

            // Create a GeoImage as the image to draw on
            GeoImage geoImage = new GeoImage(800, 600);

            // Start the drawing by specifying the image, extent and map units
            canvas.BeginDrawing(geoImage, MapUtil.GetDrawingExtent(zoningLayer.GetBoundingBox(), 800, 600), GeographyUnit.Meter);

            // This collection is used during drawing to pass labels in between layers so we can track collisions
            Collection<SimpleCandidate> labels = new Collection<SimpleCandidate>();

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
            MemoryStream imageStream = new MemoryStream();
            geoImage.Save(imageStream, GeoImageFormat.Png);

            // Create a new ImageBitmap using the stream as it's source
            //BitmapImage bitmapImage = new BitmapImage();
           //bitmapImage.BeginInit();
            //bitmapImage.StreamSource = imageStream;
           // bitmapImage.EndInit();

            // Set the source of the image control to the BitmapImage
            //MapImage.Source = bitmapImage;
        }
    }
}
