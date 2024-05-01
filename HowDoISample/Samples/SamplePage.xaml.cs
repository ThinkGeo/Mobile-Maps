namespace HowDoISample;

public partial class SamplePage
{
    public SamplePage()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty DescriptionProperty = BindableProperty.Create(
        propertyName: nameof(Description),
        returnType: typeof(string),
        declaringType: typeof(SamplePage),
        defaultValue: null);

    public string Description
    {
        get => (string)GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }
}