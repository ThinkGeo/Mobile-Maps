using System.Diagnostics;

namespace HowDoISample;

/// <summary>
/// Puts the sample data where the samples expect it: in the app's own data
/// directory, as plain files.
/// <para>
/// What a mobile app ships is inside the package, reachable only as a stream, so
/// the files are copied out on first run and the samples read them by path. A file
/// already there is left alone unless the package carries a different one, so the
/// copy costs almost nothing after the first launch.
/// </para>
/// </summary>
public static class SampleData
{
    private const string ManifestName = "manifest.txt";

    /// <summary>
    /// Copies any sample data that is not already in the app's data directory.
    /// Safe to call more than once; returns how many files it wrote.
    /// </summary>
    public static async Task<int> EnsureCopiedAsync()
    {
        var target = Samples.SampleShared.DataFolder;
        Directory.CreateDirectory(target);

        string[] entries;
        try
        {
            using var manifest = await FileSystem.OpenAppPackageFileAsync(ManifestName);
            using var reader = new StreamReader(manifest);
            entries = (await reader.ReadToEndAsync())
                .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                .Select(line => line.Trim())
                .Where(line => line.Length > 0)
                .ToArray();
        }
        catch (Exception exception)
        {
            // No manifest means no data shipped; the samples that need it will say so
            // rather than the app failing to start.
            Debug.WriteLine($"[SampleData] no {ManifestName}: {exception.Message}");
            return 0;
        }

        var written = 0;
        foreach (var entry in entries)
        {
            var destination = Path.Combine(target, entry.Replace('/', Path.DirectorySeparatorChar));

            Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
            try
            {
                using var source = await FileSystem.OpenAppPackageFileAsync(entry);

                // A file that is already there with the packaged length is kept. A
                // different length means the package carries a newer one, which
                // replaces the copy made by an earlier install.
                if (File.Exists(destination) && TryGetLength(source, out var packaged)
                    && new FileInfo(destination).Length == packaged)
                {
                    continue;
                }

                await using var file = File.Create(destination);
                await source.CopyToAsync(file);
                written++;
            }
            catch (Exception exception)
            {
                Debug.WriteLine($"[SampleData] could not copy {entry}: {exception.Message}");
            }
        }

        return written;
    }

    /// <summary>
    /// The length of a packaged file, where the platform will say: iOS hands back a
    /// file stream that knows it, and an Android asset stream usually does. When it
    /// does not, the file already on disk is kept rather than rewritten on every
    /// launch.
    /// </summary>
    private static bool TryGetLength(Stream stream, out long length)
    {
        length = 0;
        if (!stream.CanSeek) return false;

        try
        {
            length = stream.Length;
            return true;
        }
        catch (NotSupportedException)
        {
            return false;
        }
    }
}
