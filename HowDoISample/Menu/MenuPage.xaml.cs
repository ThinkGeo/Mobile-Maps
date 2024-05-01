using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Text.Json;

namespace HowDoISample;

[DesignTimeVisible(false)]
[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class MenuPage
{
    private readonly Collection<GroupInfo> _allSampleGroups;
    private ObservableCollection<GroupInfo> _currentSampleGroups;

    public ObservableCollection<GroupInfo> CurrentSampleGroups
    {
        get => _currentSampleGroups;
        set
        {
            _currentSampleGroups = value;
            OnPropertyChanged();
        }
    }

    public MenuPage()
    {
        InitializeComponent();

        _allSampleGroups = [];
        _currentSampleGroups = [];
        LoadMenu();

        // Set the first item as the selected item
        CollectionViewMenu.SelectedItem = CurrentSampleGroups[0][0];
    }

    private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
    {
        var stackLayout = (Label)sender;
        var item = (TapGestureRecognizer)stackLayout.GestureRecognizers[0];
        var id = item.CommandParameter;
        var i = CurrentSampleGroups.IndexOf((GroupInfo)id);

        _allSampleGroups[i].IsExpanded = !_allSampleGroups[i].IsExpanded;
        UpdateMenu();
    }

    private void LoadMenu()
    {
        // Deserialize samples.json
        var assembly = Assembly.GetCallingAssembly();
        var stream = assembly.GetManifestResourceStream("HowDoISample.Samples.samples.json");
        if (stream == null) return;
        using (var reader = new StreamReader(stream))
        {
            var text = reader.ReadToEnd();

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var samplesJson = JsonSerializer.Deserialize<List<SampleGroupFromJson>>(text, options);

            // Translate samples.json object into the master sample menu
            foreach (var category in samplesJson)
            {
                // Make the group
                var sampleGroup = new GroupInfo { Title = category.Title, IsExpanded = false };
                foreach (var sample in category.Children)
                {
                    sampleGroup.Add(sample);

                    var pageType = Type.GetType(sample.Id);

                    if (pageType != null)
                        Routing.RegisterRoute(sample.Id, pageType);
                }
                _allSampleGroups.Add(sampleGroup);
            }
        }

        _allSampleGroups[0].IsExpanded = true;
        UpdateMenu();
    }

    private void UpdateMenu()
    {
        var updatedSampleGroups = new ObservableCollection<GroupInfo>();
        foreach (var group in _allSampleGroups)
        {
            var sampleGroup = new GroupInfo { Title = group.Title, IsExpanded = group.IsExpanded };
            if (sampleGroup.IsExpanded) sampleGroup.AddRange(group);
            updatedSampleGroups.Add(sampleGroup);
        }
        CurrentSampleGroups = updatedSampleGroups;
    }

    internal class SampleGroupFromJson
    {
        public string Title { get; set; }
        public List<SampleInfo> Children { get; set; }
    }

    private async void CollectionViewMenu_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem is not SampleInfo selectedSample)
            return;

        if (sender is not ListView listView)
            return;

        // deselect all the other menu items 
        foreach (var item in listView.ItemsSource)
        {
            var groupInfo = (GroupInfo)item;
            foreach (var sampleInfo in groupInfo)
                sampleInfo.IsSelected = false;
        }

        selectedSample.IsSelected = true;

        // Navigate to the selected page
        var targetType = Type.GetType(selectedSample.Id);
        if (targetType == null) return;

        if (Application.Current?.MainPage is not AppShell shell) return;
        await shell.NavigateFromMenu(selectedSample);
        Shell.Current.FlyoutIsPresented = false;
    }
}