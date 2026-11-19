using System.Text.Json.Nodes;
using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// The smallest thing that proves the chain: a MAUI MapView whose basemap is drawn
/// by the GPU pipeline. Everything specific to that is the one line that assigns the
/// basemap - the map view, the camera and the gestures are the ordinary MAUI ones.
/// </summary>
public class FirstLight : ContentPage
{
    private const string StyleUri = "https://tiles.preludemaps.com/styles/WorldStreets_Light/style.json";

    // Manhattan, in spherical mercator metres - the view the desktop parity work uses.
    private static readonly PointShape HomeCenter = new(-8237642d, 4976252d);

    /// <summary>The vector source the style declares; the buildings come from it.</summary>
    private const string BuildingSource = "planet_06-20-2026";

    public FirstLight()
    {
        Title = "First Light";
        Shell.SetNavBarIsVisible(this, false);

        var map = new MapView
        {
            MapUnit = GeographyUnit.Meter,
            IsRotationEnabled = true,
            CenterPoint = HomeCenter,
            MapScale = new ThinkGeoCloudMapsZoomLevelSet().GetScales()[13],
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
        };

        // A style.json URI and nothing else. GpuBasemap is the whole public surface
        // of the GPU story on MAUI as it is on the desktop, and a basemap is not one
        // of the Overlays - it is the ground they sit on.
        var style = new MapStyle(StyleUri);

        // The style has no 3D in it, but the tiles behind it do: the building layer
        // carries render_height, render_min_height and hide_3d, so one extrusion
        // layer is enough to stand the city up. The buildings are only in the tiles
        // from zoom 13, and they are worth seeing only once the map is leaning, so
        // the layer waits for both.
        style.AddLayer(new JsonObject
        {
            ["id"] = "building-3d",
            ["type"] = "fill-extrusion",
            ["source"] = BuildingSource,
            ["source-layer"] = "building",
            ["minzoom"] = 13,
            ["filter"] = new JsonArray("!=", "hide_3d", true),
            ["paint"] = new JsonObject
            {
                ["fill-extrusion-color"] = "#cfd3da",
                ["fill-extrusion-height"] = new JsonArray("get", "render_height"),
                ["fill-extrusion-base"] = new JsonArray("get", "render_min_height"),
                ["fill-extrusion-opacity"] = 0.9,
            },
        });

        var basemap = new GpuBasemap(style);
        map.Basemap = basemap;

        // Flat, the extrusions are just roofs. The map opens already leaning so the
        // first thing it shows is the thing worth showing.
        map.TiltAngle = 50;

        // The map draws itself once it first has a size. Without this nothing is
        // ever asked to draw, and the page comes up empty with nothing in the log.
        map.Behaviors.Add(new AutoLoadMapViewBehavior());

        // Off and on again without rebuilding a tile: extrusion is a decision the
        // renderer makes while drawing, so the city stands up and lies down with the
        // geometry already uploaded. Worth having here, on the first sample, because
        // seeing it go flat is what shows that the 3D is the renderer's doing and not
        // a different map.
        Content = new Grid
        {
            Children =
            {
                map,
                BuildBuildingsToggle(map, basemap),
            },
        };
    }

    /// <summary>A small card over the map, out of the way of the gestures.</summary>
    private static View BuildBuildingsToggle(MapView map, GpuBasemap basemap)
    {
        var toggle = new Switch { IsToggled = true, VerticalOptions = LayoutOptions.Center };
        toggle.Toggled += (_, _) =>
        {
            // A renderer switch, not a restyle: no tile is rebuilt.
            basemap.IsExtrusionDrawingEnabled = toggle.IsToggled;
            _ = map.RefreshAsync();
        };

        var card = new HorizontalStackLayout
        {
            Spacing = 8,
            Padding = new Thickness(12, 6),
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.End,
            Margin = new Thickness(16, 0, 0, 28),
            Children =
            {
                new Label { Text = "Buildings", FontSize = 13, VerticalOptions = LayoutOptions.Center },
                toggle,
            },
        };

        // Readable in both themes: the card sits on whatever the map is drawing.
        card.SetAppThemeColor(VisualElement.BackgroundColorProperty, Color.FromArgb("#F2FFFFFF"), Color.FromArgb("#F2202124"));
        ((Label)card.Children[0]).SetAppThemeColor(Label.TextColorProperty, Color.FromArgb("#1F2328"), Color.FromArgb("#E6E6E6"));

        return card;
    }
}
