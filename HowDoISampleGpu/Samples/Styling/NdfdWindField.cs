using ThinkGeo.Core;
using ThinkGeo.Core.Styling;
using ThinkGeo.Gpu;
using ThinkGeo.UI.Maui;

namespace HowDoISampleGpu.Samples;

/// <summary>
/// The National Digital Forecast Database's surface wind, drawn two ways from one
/// field: sixty-five thousand particles advected along it, and the same field as
/// magnitude-scaled arrows.
/// </summary>
/// <remarks>
/// The desktop sample reads the NDFD GeoTIFFs with GdalGrid. GDAL's natives are not
/// published for iOS or Android, so the grids are converted on the desktop and
/// shipped as one flat file - which is what a phone application does anyway: a
/// forecast grid is prepared by whatever has GDAL and the app reads numbers.
/// Everything after that - the field source, the particles, the arrows - is the same
/// code the desktop runs.
/// </remarks>
public class NdfdWindField : SamplePage
{
    private GpuBasemap _basemap;
    private MapStyle _style;
    // Particles only. The same field can be drawn as magnitude-scaled arrows and
    // there was a control to switch to them, which cost a row of card to show a
    // second picture of the thing already on screen - and the advected particles are
    // the one of the two that a still image cannot convey, so they are what the
    // sample is.
    private VectorFieldSource _field;
    private MapView _map;
    private Label _status;

    protected override void BuildMap(MapView map)
    {
        _map = map;
        map.MapUnit = GeographyUnit.Meter;

        try
        {
            LoadField();

            _style = new MapStyle();
            _style.AddStyle(ThinkGeoVectorStyles.Light, new ThinkGeoVectorTileSource(SampleShared.CloudApiKey)
            {
                VectorTileCache = new FileTileCache(SampleShared.VectorTileCacheFolder),
            });
            _style.AddParticleFlow(_field);

            _basemap = new GpuBasemap(_style);
            map.Basemap = _basemap;
            map.CurrentExtent = new RectangleShape(-14026255, 6446275, -7235766, 2632018);

            _status = AddStatusLabel();
            _status.Text = "NDFD surface wind - 65k particles advected, colour is speed";
        }
        catch (Exception exception)
        {
            _status ??= AddStatusLabel();
            _status.Text = "Wind data unavailable: " + exception.Message;
        }

        _ = map.RefreshAsync();
    }

    /// <summary>
    /// Reads the packed forecast hour of speed and direction into one
    /// VectorFieldSource, in the grid's own Lambert projection.
    /// </summary>
    private void LoadField()
    {
        using var stream = File.OpenRead(Path.Combine(SampleShared.DataFolder, "Ndfd", "ndfd-wind.bin"));
        using var reader = new BinaryReader(stream);

        if (new string(reader.ReadChars(4)) != "TGWF") throw new InvalidDataException("not a packed wind field");

        var rows = reader.ReadInt32();
        var columns = reader.ReadInt32();
        var minX = reader.ReadDouble();
        var minY = reader.ReadDouble();
        var maxX = reader.ReadDouble();
        var maxY = reader.ReadDouble();
        var projection = new string(reader.ReadChars(reader.ReadInt32()));

        var speed = ReadGrid(reader, rows, columns);
        var from = ReadGrid(reader, rows, columns);

        // A meteorological wind direction is where the wind comes FROM; the arrow
        // points where it is going.
        var flow = new FlowVector[rows, columns];
        for (var row = 0; row < rows; row++)
        {
            for (var column = 0; column < columns; column++)
            {
                flow[row, column] = FlowVector.FromSpeedDirection(
                    speed[row, column], (from[row, column] + 180f) % 360f);
            }
        }

        _field = new VectorFieldSource(flow,
            new RectangleShape(minX, maxY, maxX, minY),
            new Projection(projection));
    }

    private static float[,] ReadGrid(BinaryReader reader, int rows, int columns)
    {
        var values = new float[rows, columns];
        for (var row = 0; row < rows; row++)
        {
            for (var column = 0; column < columns; column++)
            {
                values[row, column] = reader.ReadSingle();
            }
        }

        return values;
    }
}
