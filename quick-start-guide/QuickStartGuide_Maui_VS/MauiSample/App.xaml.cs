using ThinkGeo.UI.Maui;

namespace MauiSample
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();
        }

        protected override async void OnStart()
        {
            await LicenseLoader.LoadLicense("mauidemo.thinkgeo.com.android.mapsuitelicense");
            await LicenseLoader.LoadLicense("mauidemo.thinkgeo.com.ios.mapsuitelicense");            
            MainPage = new AppShell();
        }
    }
}
