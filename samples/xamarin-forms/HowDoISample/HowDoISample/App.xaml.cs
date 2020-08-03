using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ThinkGeo.UI.Xamarin.HowDoI;

namespace HowDoISample
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();
            Device.SetFlags(new string[] { "Expander_Experimental", "RadioButton_Experimental" });
            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
