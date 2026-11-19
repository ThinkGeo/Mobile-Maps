namespace HowDoISampleGpu.Samples;

/// <summary>
/// What every sample shares: the ThinkGeo Cloud credentials, where the data is,
/// where the tile cache goes. The desktop gallery's counterpart is
/// samples/wpf/HowDoISample/Samples/SampleShared.cs, and the credentials are the
/// same demo ones.
/// </summary>
public static class SampleShared
{
    /// <summary>The ThinkGeo Cloud v3 apiKey the gallery demos with (the classic
    /// ClientId/ClientSecret pair is a different credential and does not open v3).</summary>
    public const string CloudApiKey = "SGjwpxF60knqZi3R0A9RRE3GqiuRA1LsnxHMot7rJ58~";

    /// <summary>The ThinkGeo Cloud ClientId and ClientSecret the Cloud services -
    /// geocoding, routing, elevation, colors - and the classic raster maps layers
    /// sign in with.</summary>
    public const string ClientId = "AOf22-EmFgIEeK4qkdx5HhwbkBjiRCmIDbIYuP8jWbc~";
    public const string ClientSecret = "xK0pbuywjaZx4sqauaga8DMlzZprz0qQSjLTow90EhBx5D8gFd2krw~~";

    /// <summary>
    /// Where the sample data lands. Unlike the desktop gallery, which reads the
    /// files beside its executable, a MAUI app ships them as embedded resources and
    /// copies them into the app's own data directory on first run - see
    /// <see cref="App"/>. Both galleries then speak in plain file paths.
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
