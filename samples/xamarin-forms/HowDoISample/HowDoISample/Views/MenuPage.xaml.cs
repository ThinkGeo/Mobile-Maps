using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using HowDoISample.Models;
using HowDoISample.Services;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThinkGeo.UI.Xamarin.HowDoI
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MenuPage : ContentPage
    {
        MainPage RootPage { get => Application.Current.MainPage as MainPage; }
        List<MenuGroup> CategoryMenuItems { get; }

        public MenuPage()
        {
            InitializeComponent();

            CategoryMenuItems = GetMenuItems();

            ListViewMenu.ItemsSource = CategoryMenuItems;
        }

        private async void ListViewMenu_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;

            var id = ((SampleMenuItem)e.SelectedItem).Id;
            await RootPage.NavigateFromMenu(id);
        }

        /// <summary>
        /// Reads samples.json to build the menu items
        /// NOTE: This method slows down the application due to running on the main thread. A workaround will be implemented in the future.
        /// </summary>
        /// <returns></returns>
        private List<MenuGroup> GetMenuItems()
        {
            List<MenuGroup> menuItems = new List<MenuGroup>();
            List<Category> categories;

            var assembly = IntrospectionExtensions.GetTypeInfo(typeof(SampleDataStore)).Assembly;
            Stream stream = assembly.GetManifestResourceStream("HowDoISample.samples.json");
            using (var reader = new StreamReader(stream))
            {
                var text = reader.ReadToEnd();
                categories = JsonConvert.DeserializeObject<List<Category>>(text);
            }

            foreach (var category in categories)
            {
                var sampleGroup = new MenuGroup() { Title = category.Title };
                foreach (var sample in category.Children)
                {
                    sampleGroup.Add(new SampleMenuItem() { Id = sample.Id, Title = sample.Title });
                }
                menuItems.Add(sampleGroup);
            }

            return menuItems;
        }
    }
}