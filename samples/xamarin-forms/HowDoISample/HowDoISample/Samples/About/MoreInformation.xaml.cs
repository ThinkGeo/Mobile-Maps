using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MoreInformation : ContentPage
    {
        public MoreInformation()
        {
            InitializeComponent();
        }

        private async void Hyperlink_Tapped(object sender, EventArgs e)
        {
            await Launcher.OpenAsync(((TappedEventArgs)e).Parameter.ToString());
        }

        private async void HyperlinkButton_Tapped(object sender, EventArgs e)
        {
            await Launcher.OpenAsync(((ImageButton)sender).BindingContext.ToString());
        }
    }
}