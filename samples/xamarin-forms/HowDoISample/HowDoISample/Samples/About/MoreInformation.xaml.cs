using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;

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