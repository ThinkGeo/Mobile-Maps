namespace HowDoISample;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class RollingTextMarker
{
    public RollingTextMarker()
    {
        InitializeComponent();
        StartRollingTextAnimation();
    }

    private void StartRollingTextAnimation()
    {
        RollingTextLabel.SizeChanged += (_, _) =>
        {
            // Ensure the animation moves the label from fully right to fully left
            var animation = new Animation(v => RollingTextLabel.TranslationX = v, 200, -200);
            animation.Commit(this, "RollingText", 16, 5000, Easing.Linear, (_, _) =>
            {
                RollingTextLabel.TranslationX = 200; // Reset to start for looping
            }, () => true); // Repeat indefinitely
        };
    }

    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text), typeof(string), typeof(RollingTextMarker), string.Empty);

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }


    public static readonly BindableProperty ImagePathProperty = BindableProperty.Create(
        nameof(ImagePath), typeof(string), typeof(RollingTextMarker), string.Empty);

    public string ImagePath
    {
        get => (string)GetValue(ImagePathProperty);
        set => SetValue(ImagePathProperty, value);
    }
}
