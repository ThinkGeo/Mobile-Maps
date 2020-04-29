# ThinkGeo Mobile Maps

Welcome, we're glad you're here!  If you're new to ThinkGeo's Mobile Maps, we suggest that you start by taking a look at our quickstart guide below.  This will introduce you to get a nice looking map up and running with ThinkGeo Background and external data.  After reviewing this, we strongly recommend that you check out our samples for both [iOS](samples/ios) and [iOS](samples/iOS).  It's packed with examples covering nearly everything you can do with our Mobile Maps control.

## Repository Layout

`/api-docs`: An offline version the API documentation HTML pages.

`/hero-app`: A real world application that shows off many of this products features along with best practices.

`/samples`: A collection of feature by feature samples.

`/.assets`: Any assets needed for the readme.md.

`README.md`: A quick start guide to show you how to quickly get up and running.

## Samples ##

We have a number of samples for both Android and iOS that show off ThinkGeo Mobile Maps' full capabilities. You can use these samples as a starting point for your own application, or simply reference them for how to use our controls using best practices.

* [Android samples](samples/Android)
* [iOS samples](samples/iOS)


## Quick Start: Display a Simple Map on iOS ##

This will introduce you to ThinkGeo Mobile Maps by getting a nice looking map up and running with ThinkGeo background map along with some external data  on a Xamarin iOS application. By the end of this guide, you should have a basic understanding of how to use the Mobile Maps controls.

![alt text](.assets/ios_quickstart_shapefile_pointstyle_screenshot.PNG "Simple Map")

### Step 1: Set Up Prerequisites

In order to develop and debug Xamarin iOS applications, you'll need to have a few prerequisites set up. 

##### A. To develop on Mac, You need 

* XCode, which provides iOS emulator. 
* A development IDE, it could be Visual Studio for Mac, Xamarin Studio or others. 
* Xamarin. It has been installed by default in Visual Studio for Mac or Xamarin Studio, might need to be manually installed in other IDEs. 
* A provisioning profile is needed if you want to test on an iOS device. 

##### B. To develop on Windows, you need
* A Mac Machine in the same network as your build server. XCode needs to be installed on that machine.
* And on your Windows machine, you need: 
	* A development IDE, it could be Visual Studio, Xamarin Studio or others. 
	* Xamarin. Make sure it is well installed. 
 
Even it sounds complicated, Microsoft in fact has made it very straightforward to connect to a MAC and develop Xamarin on Windows. So if you are a .NET developer get used to Visual Studio, feel free to stay on Windows for Xamarin development. This quick start guide is based on Visual Studio on Windows and there's no problem at all to start your application on Mac.   

### Step 1: Set Up a New Project

Once these prerequisites have been installed, let's create a new **iOS App (Xamarin)** project in Visual Studio and select **Single View App** template. Here is a [guide to creating a sample project](https://docs.microsoft.com/en-us/xamarin/iOS/get-started/hello-iOS/hello-iOS-quickstart) for reference. 

Go ahead and run the project once it is created, and Visual Studio will help you to locate the MacOS server and initialize the connection. If everything goes well, you should see the iOS emulator get launched on Windows and the default project runs in the emulator.  

### Step 2: Implement the code

##### A. Install Nuget Package **ThinkGeo.UI.iOS** to the project. 

##### B. Add the required usings to the ViewController.cs file:

```csharp
using ThinkGeo.Core;
using ThinkGeo.iOS.UI
```

##### C. Update ViewDidLoad() method in ViewController.cs as following:

```csharp
public override void ViewDidLoad ()
{
    base.ViewDidLoad ();
    // Perform any additional setup after loading the view, typically from a nib.

    // Creat a new MapView, which is the canvas of the map, and add it to the View.
    MapView mapView = new MapView(View.Frame);
    View.AddSubview(mapView);

    // Set the Map Unit to Meter and set the map's current extent to North America.
    mapView.MapUnit = GeographyUnit.Meter;
    mapView.CurrentExtent = new RectangleShape(-13939426, 6701997, -7812401, 2626987);

    // Create a new ThinkGeoCloud Overlay using Client ID / Client Secret, and add it the overlay to MapView. 
    string clientKey = "9ap16imkD_V7fsvDW9I8r8ULxgAB50BX_BnafMEBcKg~";
    string secret = "vtVao9zAcOj00UlGcK7U-efLANfeJKzlPuDB9nw7Bp4K4UxU_PdRDg~~";
    ThinkGeoCloudVectorMapsOverlay thinkGeoCloudMapsOverlay = new ThinkGeoCloudVectorMapsOverlay(clientKey, secret);
    mapView.Overlays.Add(thinkGeoCloudMapsOverlay);
    
    mapView.Refresh();
}
```

### Step 3: Apply an Evaluation License for Free

Build the project and make sure it builds through. "A license is needed" exception will be thrown if you run it though and here is how to fix it: 

1. Run ThinkGeo.ProductCenter.exe to open the product center. This can be found in the `bin` folder of the project. (ThinkGeo.ProductCenter.exe can only be opened on Windows, there's a CLI version for Mac.)

1. Click on `Log In` in the upper-right corner, input the username/password to login or click `Create a new account` to create a ThinkGeo account for free. 

1. Once logged in, click on `ThinkGeo UI Mobile for iOS` tile and then click on `Start Evaluation`(it would be `Activate License` if you already purchased), now you can see a textbox with textholder `Bundle Identifer` on the right. 

1. Now we need to generate a license for the project following the steps below: 
	* Get the project's bundle identifier in info.plist, copy and paste it to the 'bundle identifier' textbox in product center. 
    * Hit 'Create' and save the license file (the file name would be "bundle-identifer.mapsuitelicense") to the solution's root folder. 
	* Add the license to the project in the solution explorer by right-clicking the project and select `Add -> Existing Item...`.
	* Right-click the license file in the solution explorer, select `Properties` and change the `Build Action` to `BundleResource`.

Now go ahead and run the application and the map will be displayed properly. 


### Step 4: Adding an External Data Source

Now let's add an external data source (Shape File) to the map. 
1. Download [WorldCapitals.zip](.assets/WorldCapitals.zip) shapefile and unzip it in your project under a new folder called `SampleData`. 

1. Include those files to the project. Multi-select them and change the Build Action to "Content". 

1. Now add the following code to ViewDidLoad() method. 

```csharp
// Create a new Feature Layer using the WorldCapitals.shp Shapefile.
ShapeFileFeatureLayer worldCapitalsFeatureLayer = new ShapeFileFeatureLayer("SampleData/WorldCapitals.shp");
// Set the pointstyle to black circle with the size of 8. 
worldCapitalsFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = PointStyle.CreateSimpleCircleStyle(GeoColors.White, 8, GeoColors.Black);
// Apply the point style from zoomlevel01 to zoomlevel20, that's accross all the zoomlevels. 
worldCapitalsFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

// Convert the world capital featurelay from DecimalDegrees, which is the projection of the raw data, to Spherical Mercator, which is the projection of the map. 
worldCapitalsFeatureLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(Projection.GetDecimalDegreesProjString(), Projection.GetSphericalMercatorProjString());

// Add the Layer to an Overlay and add the overlay to the map. 
LayerOverlay layerOverlay = new LayerOverlay();
layerOverlay.Layers.Add(worldCapitalsFeatureLayer);
mapView.Overlays.Add(layerOverlay);
```

### Summary

You now know the basics of using the ThinkGeo Map controls and are able to get started adding functionality into your own applications. Let's recap what we have learned about the object relationships and how the pieces of ThinkGeo UI work together:

1. It is of the utmost importance that the units (feet, meters, decimal degrees, etc.) be set properly for the Map control based on the data.
1. FeatureLayers provide the data used by a Map control to render a map.
1. A Map is the basic control that contains all of the other objects that are used to tell how the map is to be rendered.
1. A Map has many layers. A Layer correlates one-to-one with a single data source and typically of one type (point, polygon, line etc).
1. A FeatureLayer can have several ZoomLevels. ZoomLevels help to define ranges (upper and lower) of when a Layer should be shown or hidden.

You are now in a great position to look over the [other samples available](samples/iOS) and explore our other features.

## Need Help? ##

If you run into any issues with running the samples, please create a new issue in the issue tracker. 

If you have any questions about the product or sales, please contact us at [sales@thinkgeo.com](mailto:sales@thinkgeo.com).
