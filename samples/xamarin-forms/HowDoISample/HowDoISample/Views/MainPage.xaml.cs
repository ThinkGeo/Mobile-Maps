using System;
using System.ComponentModel;
using System.Threading.Tasks;
using HowDoISample.Models;
using Xamarin.Forms;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : FlyoutPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        public async Task NavigateFromMenu(SampleMenuItem sample)
        {
            var type = Type.GetType(sample.Id);
            var samplePage = (ContentPage) Activator.CreateInstance(type);
            samplePage.Title = sample.Title;
            samplePage.FindByName<Label>("descriptionLabel").Text = sample.Description;

            var sampleNavPage = new NavigationPage(samplePage);

            if (Detail != sampleNavPage)
            {
                Detail = sampleNavPage;

                if (Device.RuntimePlatform == Device.Android)
                    await Task.Delay(100);

                IsPresented = false;
            }
        }
    }
}