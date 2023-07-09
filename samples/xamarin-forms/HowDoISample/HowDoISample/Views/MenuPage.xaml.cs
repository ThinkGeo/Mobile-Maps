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
        private readonly SampleMenu sampleMenu;

        public MenuPage()
        {
            InitializeComponent();
            sampleMenu = new SampleMenu();
            ListViewMenu.ItemsSource = sampleMenu.SampleMenuItems;
            
            // Set the first item as the selected item
            ListViewMenu.SelectedItem = sampleMenu.SampleMenuItems[0][0];
        }

        private MainPage RootPage => Application.Current.MainPage as MainPage;

        private async void ListViewMenu_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (RootPage == null) 
                return;
            
            var sample = (SampleMenuItem) e.SelectedItem; 
            await RootPage.NavigateFromMenu(sample);
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            var stackLayout = (StackLayout) sender;
            var item = (TapGestureRecognizer) stackLayout.GestureRecognizers[0];
            var id = item.CommandParameter;
            var i = sampleMenu.SampleMenuItems.IndexOf((MenuGroup) id);
            sampleMenu.ToggleGroupExpanded(i);
            ListViewMenu.ItemsSource = sampleMenu.SampleMenuItems;
        }
    }
}