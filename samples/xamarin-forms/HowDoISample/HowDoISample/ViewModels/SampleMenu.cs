using HowDoISample.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HowDoISample.ViewModels
{
    class SampleMenu : INotifyPropertyChanged
    {
        private ObservableCollection<MenuGroup> _allMenuItems;

        public ObservableCollection<MenuGroup> SampleMenuItems { get; set; }

        // unused
        public Command LoadSamplesCommand { get; set; }

        public SampleMenu()
        {
            _allMenuItems = new ObservableCollection<MenuGroup>();
            SampleMenuItems = new ObservableCollection<MenuGroup>();
            LoadMenu();

            // unused
            LoadSamplesCommand = new Command(async () => await LoadSamples());
        }

        public void ToggleGroupExpanded(int index)
        {
            _allMenuItems[index].IsExpanded = !_allMenuItems[index].IsExpanded;
            UpdateMenu();
        }

        private void LoadMenu()
        {
            List<SampleCategory> samplesJson;

            // Deserialize samples.json
            var assembly = IntrospectionExtensions.GetTypeInfo(typeof(SampleMenu)).Assembly;
            Stream stream = assembly.GetManifestResourceStream("HowDoISample.samples.json");
            using (var reader = new StreamReader(stream))
            {
                var text = reader.ReadToEnd();
                samplesJson = JsonConvert.DeserializeObject<List<SampleCategory>>(text);
            }

            // Translate samples.json object into the master sample menu
            foreach (var category in samplesJson)
            {
                // Make the group
                var sampleGroup = new MenuGroup() { Title = category.Title, IsExpanded = false };
                foreach (var sample in category.Children)
                {
                    sampleGroup.Add(new SampleMenuItem() { Id = sample.Id, Title = sample.Title });
                }
                _allMenuItems.Add(sampleGroup);
            }
            UpdateMenu();
        }

        private void UpdateMenu()
        {
            var updatedMenu = new ObservableCollection<MenuGroup>();
            foreach (var group in _allMenuItems)
            {
                var sampleGroup = new MenuGroup() { Title = group.Title, IsExpanded = group.IsExpanded };
                if (sampleGroup.IsExpanded)
                {
                    foreach (var sample in group)
                    {
                        sampleGroup.Add(new SampleMenuItem() { Id = sample.Id, Title = sample.Title });
                    }
                }
                updatedMenu.Add(sampleGroup);
            }
            SampleMenuItems = updatedMenu;
        }

        // unused
        async Task LoadSamples()
        {
            List<SampleCategory> samplesJson;

            // Deserialize samples.json
            var assembly = IntrospectionExtensions.GetTypeInfo(typeof(SampleMenu)).Assembly;
            Stream stream = assembly.GetManifestResourceStream("HowDoISample.samples.json");
            using (var reader = new StreamReader(stream))
            {
                var text = await reader.ReadToEndAsync();
                samplesJson = JsonConvert.DeserializeObject<List<SampleCategory>>(text);
            }

            // Translate samples.json object into the master sample menu
            foreach (var category in samplesJson)
            {
                var sampleGroup = new MenuGroup() { Title = category.Title };
                foreach (var sample in category.Children)
                {
                    sampleGroup.Add(new SampleMenuItem() { Id = sample.Id, Title = sample.Title });
                }
                _allMenuItems.Add(sampleGroup);
            }
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
