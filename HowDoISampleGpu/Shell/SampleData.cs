using System.Diagnostics;

namespace HowDoISampleGpu;

/// <summary>
/// Puts the sample data where the samples expect it: in the app's own data
/// directory, as plain files.
/// <para>
/// The desktop gallery reads its data beside the executable. A mobile app has no
/// such place - what it ships is inside the package, reachable only as a stream -
/// so the files are copied out once, on first run, and every sample afterwards is
/// written exactly as the desktop one is. The copy is skipped for files that are
/// already there, so it costs nothing after the first launch.
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
            if (File.Exists(destination)) continue;

            Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
            try
            {
                using var source = await FileSystem.OpenAppPackageFileAsync(entry);
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
}
