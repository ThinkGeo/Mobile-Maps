namespace HowDoISample;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class DescriptionView
{
    public static readonly BindableProperty DescriptionProperty = BindableProperty.Create(nameof(Description), typeof(string), typeof(DescriptionView));

    public string Description
    {
        get => (string)GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public DescriptionView()
    {
        InitializeComponent();
    }
}