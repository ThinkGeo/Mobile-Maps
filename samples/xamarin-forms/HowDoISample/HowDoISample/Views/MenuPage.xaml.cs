using HowDoISample.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HowDoISample.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MenuPage : ContentPage
    {
        MainPage RootPage { get => Application.Current.MainPage as MainPage; }
        ObservableCollection<CategoryMenuItem> categoryMenuItems;

        public MenuPage()
        {
            InitializeComponent();

            var category = new CategoryMenuItem() { Title = "Category" };
            category.Add(new SampleMenuItem() { Title = "Sample", Id = "SampleTemplate" });

            categoryMenuItems = new ObservableCollection<CategoryMenuItem>
            {
                category
            };

            ListViewMenu.ItemsSource = categoryMenuItems;

            ListViewMenu.ItemSelected += async (sender, e) =>
            {
                if (e.SelectedItem == null)
                    return;

                var id = ((SampleMenuItem)e.SelectedItem).Id;
                await RootPage.NavigateFromMenu(id);
            };
        }
    }
}