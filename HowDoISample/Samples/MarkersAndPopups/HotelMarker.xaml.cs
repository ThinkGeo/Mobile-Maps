namespace HowDoISample.MarkersAndPopups;

public partial class HotelMarker 
{
    public HotelMarker()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty StatusCodeProperty =
        BindableProperty.Create(nameof(StatusCode), typeof(int), typeof(HotelMarker));

    public int StatusCode
    {
        get => (int)GetValue(StatusCodeProperty);
        set => SetValue(StatusCodeProperty, value);
    }
}