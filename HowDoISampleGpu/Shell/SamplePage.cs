using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu;

/// <summary>
/// What every sample page shares: a full-bleed MapView that has a size before the
/// sample touches it, and the one behaviour that makes a map draw itself.
/// <para>
/// The desktop gallery does this with a <c>Map_SizeChanged</c> guard in each sample,
/// because a WPF MapView is configured once it first has a size. Here the wait is in
/// one place and a sample only writes <see cref="BuildMap"/>.
/// </para>
/// </summary>
public abstract class SamplePage : ContentPage
{
    protected SamplePage()
    {
        Shell.SetNavBarIsVisible(this, false);

        Map = new MapView
        {
            MapUnit = GeographyUnit.Meter,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
        };

        // The map draws itself once it first has a size. Without this nothing is ever
        // asked to draw, and the page comes up empty with nothing in the log.
        Map.Behaviors.Add(new AutoLoadMapViewBehavior());
        Content = Map;

        var built = false;
        Map.SizeChanged += (_, _) =>
        {
            if (built || Map.Width <= 0 || Map.Height <= 0) return;
            built = true;
            BuildMap(Map);
        };
    }

    /// <summary>The map this page draws into.</summary>
    protected MapView Map { get; }

    /// <summary>
    /// Builds the sample, once the map has a real size. The counterpart of the desktop
    /// gallery's <c>Map_SizeChanged</c> body.
    /// </summary>
    protected abstract void BuildMap(MapView map);
}
