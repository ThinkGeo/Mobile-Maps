using System;
using System.ComponentModel;
using HowDoISample.Models;
using HowDoISample.ViewModels;
using Xamarin.Forms;

namespace ThinkGeo.UI.XamarinForms.HowDoI
{
    [DesignTimeVisible(false)]
    public partial class MenuPage : ContentPage
    {
        private SampleMenu sampleMenu;

        MainPage RootPage { get => Application.Current.MainPage as MainPage; }

        public MenuPage()
        {
            InitializeComponent();
            sampleMenu = new SampleMenu();
            ListViewMenu.ItemsSource = sampleMenu.SampleMenuItems;
        }

        private async void ListViewMenu_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;

            var id = ((SampleMenuItem)e.SelectedItem).Id;
            await RootPage.NavigateFromMenu(id);
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            // TODO: Use MVVM properly
            StackLayout stackLayout = (StackLayout)sender;
            var item = (TapGestureRecognizer)stackLayout.GestureRecognizers[0];
            var id = item.CommandParameter;
            int i = sampleMenu.SampleMenuItems.IndexOf((MenuGroup)id);
            sampleMenu.ToggleGroupExpanded(i);
            ListViewMenu.ItemsSource = sampleMenu.SampleMenuItems;
        }
    }
}
