using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HowDoISample;
    
public class SampleInfo : INotifyPropertyChanged
{
    public string Id { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    private bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected == value) return;
            _isSelected = value;
            OnPropertyChanged();
        }   
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}