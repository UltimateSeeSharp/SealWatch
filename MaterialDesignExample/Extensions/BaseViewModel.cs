using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SealWatch.Wpf.Extensions;

/// <summary>
/// Provides functionality for every ViewModel 
/// to implement OnPropertyChanged
/// </summary>
public class BaseViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public void OnPropertyChanged([CallerMemberName] string propertyname = null!)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
    }
}
