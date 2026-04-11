using ThinkGeo.Core;
using ThinkGeo.UI.Maui;

namespace HowDoISample.MapNavigation;

public partial class Navigation
{
	private bool _initialized;
	private GpsMarker _gpsMarker;
	private bool _gpsEnabled;
	private readonly System.Timers.Timer _gpsTimer = new();
	private CancellationTokenSource _cancellationTokenSource = new();

	public Navigation()
	{
		InitializeComponent();
		mapView.MapRotationChanged += Map_MapRotationChanged;
	}

	private void Map_MapRotationChanged(object sender, MapRotationChangedMapViewEventArgs e)
	{
		CompassButton.Rotation = (float)(mapView.MapRotation);
	}

	public bool GpsEnabled
	{
		get => _gpsEnabled;
		set
		{
			if (_gpsEnabled == value) return;
			_gpsEnabled = value;
			OnPropertyChanged();
		}
	}

	private async void Map_OnSizeChanged(object sender, EventArgs e)
	{
		if (_initialized)
			return;
		_initialized = true;

		// Set the map's unit of measurement to meters(Spherical Mercator)
		mapView.MapUnit = GeographyUnit.Meter;

		// Add ThinkGeo Cloud Maps as the background 
		var backgroundOverlay = new ThinkGeoRasterOverlay
		{
			ClientId = SampleKeys.ClientId,
			ClientSecret = SampleKeys.ClientSecret,
			MapType = ThinkGeoCloudRasterMapsMapType.Light_V2_X2,
			TileCache = new FileRasterTileCache(FileSystem.Current.CacheDirectory, "ThinkGeoRasterCache")
		};
		mapView.Overlays.Add(backgroundOverlay);

		// create a point for empire state building, convert the Lat/Lon (srid:4326) to Spherical Mercator(srid:3857), which is the projection of the background
		var empireStateBuilding =
			ProjectionConverter.Convert(4326, 3857, new PointShape(-73.9856654, 40.74843661));

		var marker = new RollingTextMarker
		{
			Position = empireStateBuilding,
			Text = "Empire State Building",
			ImagePath = "empire_state_building.png"
		};
		marker.IsVisible = false;

		var simpleMarkerOverlay = new SimpleMarkerOverlay();
		mapView.Overlays.Add("simpleMarkerOverlay", simpleMarkerOverlay);
		simpleMarkerOverlay.Children.Add(marker);

		// Add the GPS Marker
		_gpsMarker = new GpsMarker();
		_gpsMarker.IsVisible = false;
		simpleMarkerOverlay.Children.Add(_gpsMarker);
		// Update GPS position every 20s.
		_gpsTimer.Interval = 20000;
		_gpsTimer.Elapsed += _gpsTimer_Elapsed;

		mapView.IsRotationEnabled = true;

		// events
		DefaultExtentButton.Clicked += async (_, _) =>
			await mapView.ZoomToExtentAsync(empireStateBuilding, 100000, -30);
		CompassButton.Clicked += async (_, _) =>
			await mapView.ZoomToExtentAsync(mapView.CenterPoint, mapView.MapScale, 0);
		ThemeCheckBox.CheckedChanged += async (_, args) =>
		{
			backgroundOverlay.MapType = args.Value
				? ThinkGeoCloudRasterMapsMapType.Dark_V2_X2
				: ThinkGeoCloudRasterMapsMapType.Light_V2_X2;

			await UpdateCancellationToken();
			// if we don't pass in _cancellationTokenSource.Token, the tiles could be messed up when checking/unchecking the Dark Theme checkbox quickly. 
			await backgroundOverlay.RefreshAsync(_cancellationTokenSource.Token);
		};

		// set up the map extent and refresh
		mapView.MapRotation = -30;
		mapView.MapScale = 100000;
		mapView.CenterPoint = empireStateBuilding;

		await mapView.RefreshAsync();
		marker.IsVisible = true;
	}


	private async Task<PointShape> GetGpsPointAsync()
	{
		try
		{
			var location = await Geolocation.GetLocationAsync(new GeolocationRequest
			{
				DesiredAccuracy = GeolocationAccuracy.Medium,
				Timeout = TimeSpan.FromSeconds(15)
			});

			if (location == null)
			{
				throw new Exception("Unable to retrieve GPS Point");
			}

			var newCenter = ProjectionConverter.Convert(4326, 3857, location.Longitude, location.Latitude);
			return new PointShape(newCenter.X, newCenter.Y);

		}
		catch (Exception e)
		{
			WarningLabel.Text = e.Message;
			WarningLabel.IsVisible = true;

			// wait for 5s
			await Task.Delay(5 * 1000);
			WarningLabel.IsVisible = false;
			return null;
		}
	}

	private async void ImageButton_OnClicked(object sender, EventArgs e)
	{
		if (GpsEnabled)
		{
			GpsEnabled = false;
			_gpsMarker.IsVisible = false;
			_gpsTimer.Stop();
			return;
		}

		var gps = await GetGpsPointAsync();
		if (gps == null)
			return;

		_gpsMarker.Position = gps;
		_gpsMarker.IsVisible = true;

		GpsEnabled = true;

		await mapView.CenterAtAsync(gps);

		_gpsTimer.Start();
	}

	private async void _gpsTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
	{
		if (!GpsEnabled)
		{
			_gpsTimer.Stop();
			return;
		}

		var gps = await Dispatcher.DispatchAsync(GetGpsPointAsync);
		if (gps == null)
			return;

		_gpsMarker.Position = gps;
		await Dispatcher.DispatchAsync(async () => await mapView.Overlays["simpleMarkerOverlay"].RefreshAsync());
	}

	private async Task UpdateCancellationToken()
	{
		await _cancellationTokenSource.CancelAsync();
		_cancellationTokenSource.Dispose();
		_cancellationTokenSource = new CancellationTokenSource();
	}
}
