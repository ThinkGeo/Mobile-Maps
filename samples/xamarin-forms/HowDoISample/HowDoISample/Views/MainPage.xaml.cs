using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using HowDoISample.Models;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : MasterDetailPage
    {
        public MainPage()
        {
            InitializeComponent();

            MasterBehavior = MasterBehavior.Popover;
        }

        public async Task NavigateFromMenu(string id)
        {
            var type = Type.GetType(id);
            var samplePage = new NavigationPage((Page)Activator.CreateInstance(type));

            if (samplePage != null && Detail != samplePage)
            {
                Detail = samplePage;

                if (Device.RuntimePlatform == Device.Android)
                    await Task.Delay(100);

                IsPresented = false;
            }
        }
    }
}
