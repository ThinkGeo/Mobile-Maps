using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace HowDoISample
{
    /// <summary>
    /// Provides bindable map-related properties for sample pages.
    /// This ViewModel supports MVVM data binding for elements like the compass rotation.
    /// </summary>
    public class MapViewModel : INotifyPropertyChanged
    {
        private double mapRotation;
        private bool gpsEnabled;
        private string description = string.Empty;

        /// <summary>
        /// Gets or sets the current rotation angle of the map (in degrees).
        /// UI elements such as the compass image can bind to this property.
        /// </summary>
        public double MapRotation
        {
            get => mapRotation;
            set
            {
                if (mapRotation != value)
                {
                    mapRotation = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether GPS tracking is currently enabled.
        /// UI elements can bind to this property to change icons dynamically.
        /// </summary>
        public bool GpsEnabled
        {
            get => gpsEnabled;
            set
            {
                if (gpsEnabled != value)
                {
                    gpsEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Description
        {
            get => description;
            set
            {
                if (description != value)
                {
                    description = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Converts MapViewModel.GpsEnabled (bool) to the proper GPS icon image source.
        /// </summary>
        public class GpsEnabledToImageConverter : IValueConverter
        {
            public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
            {
                if (value is bool gpsEnabled)
                {
                    return gpsEnabled ? "icon_gps_enabled.png" : "icon_gps_disabled.png";
                }
                return "icon_gps_disabled.png";
            }

            public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }
    }


    /// <summary>
    /// Converts MapViewModel.GpsEnabled (bool) to the proper GPS icon image source.
    /// </summary>
    public class GpsEnabledToImageConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool gpsEnabled)
            {
                return gpsEnabled ? "icon_gps_enabled.png" : "icon_gps_disabled.png";
            }
            return "icon_gps_disabled.png";
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
