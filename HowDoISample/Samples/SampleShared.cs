namespace HowDoISample.Samples;

/// <summary>
/// What every sample shares: the ThinkGeo Cloud key, where the data is and where
/// the tile cache goes.
/// </summary>
public static class SampleShared
{
    /// <summary>The ThinkGeo Cloud API key the gallery's basemaps are requested with.
    /// It is an evaluation key; an application uses its own.</summary>
    public const string CloudApiKey = "SGjwpxF60knqZi3R0A9RRE3GqiuRA1LsnxHMot7rJ58~";

    /// <summary>
    /// Where the sample data lands. A MAUI app ships its data inside the package and
    /// copies it into the app's own data directory on first run - see
    /// <see cref="SampleData"/> - so the samples read plain file paths.
    /// </summary>
    public static string DataFolder => Path.Combine(FileSystem.Current.AppDataDirectory, "Data");

    /// <summary>Shapefile path inside the copied data folder.</summary>
    public static string Shapefile(string file) => Path.Combine(DataFolder, "Shapefile", file);

    /// <summary>
    /// Where the ThinkGeo Cloud vector tiles are kept between runs: a tile source
    /// built in code has no cache unless it is given one, and a world view is dozens
    /// of HTTPS round trips - which on a phone is also the battery.
    /// </summary>
    public static string VectorTileCacheFolder =>
        Path.Combine(FileSystem.Current.CacheDirectory, "ThinkGeo", "HowDoI", "VectorTileCache");
}
