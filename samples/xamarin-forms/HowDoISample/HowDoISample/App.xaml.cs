using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ThinkGeo.UI.Xamarin.HowDoI;
using HowDoISample.Services;

namespace HowDoISample
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();
            Device.SetFlags(new string[] { "Expander_Experimental" });
            DependencyService.Register<SampleDataStore>();
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
