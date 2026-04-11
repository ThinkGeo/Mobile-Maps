using ThinkGeo.UI.Maui;

namespace HowDoISample.MarkupExtensions;

/// <summary>
/// Provides the ThinkGeo AppDataFolder markup extension without requiring a service provider.
/// </summary>
[AcceptEmptyServiceProvider]
public class SafeAppDataFolderExtension : AppDataFolderExtension
{
}
