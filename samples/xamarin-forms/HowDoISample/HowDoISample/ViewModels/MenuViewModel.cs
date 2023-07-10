using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using HowDoISample.Models;
using Newtonsoft.Json;

namespace HowDoISample.ViewModels
{
    internal class MenuViewModel : INotifyPropertyChanged
    {
        private readonly ObservableCollection<MenuGroup> _allMenuItems;

        public MenuViewModel()
        {
            _allMenuItems = new ObservableCollection<MenuGroup>();
            SampleMenuItems = new ObservableCollection<MenuGroup>();
            LoadMenu();
        }

        public ObservableCollection<MenuGroup> SampleMenuItems { get; set; }

        public void ToggleGroupExpanded(int index)
        {
            _allMenuItems[index].IsExpanded = !_allMenuItems[index].IsExpanded;
            UpdateMenu();
        }

        private void LoadMenu()
        {
            List<SampleCategory> samplesJson;

            // Deserialize samples.json
            var assembly = typeof(MenuViewModel).GetTypeInfo().Assembly;
            var stream = assembly.GetManifestResourceStream("HowDoISample.samples.json");
            using (var reader = new StreamReader(stream))
            {
                var text = reader.ReadToEnd();
                samplesJson = JsonConvert.DeserializeObject<List<SampleCategory>>(text);
            }

            // Translate samples.json object into the master sample menu
            foreach (var category in samplesJson)
            {
                // Make the group
                var sampleGroup = new MenuGroup { Title = category.Title, IsExpanded = false };
                foreach (var sample in category.Children)
                    sampleGroup.Add(sample);
                _allMenuItems.Add(sampleGroup);
            }

            _allMenuItems[0].IsExpanded = true;
            UpdateMenu();
        }

        private void UpdateMenu()
        {
            var updatedMenu = new ObservableCollection<MenuGroup>();
            foreach (var group in _allMenuItems)
            {
                var sampleGroup = new MenuGroup { Title = group.Title, IsExpanded = group.IsExpanded };
                if (sampleGroup.IsExpanded)
                    foreach (var sample in @group)
                        sampleGroup.Add(sample);
                updatedMenu.Add(sampleGroup);
            }

            SampleMenuItems = updatedMenu;
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