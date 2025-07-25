using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Luban.UI.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _greeting = "Welcome to Avalonia!";

    [ObservableProperty]
    private int _value = 0;

    [RelayCommand]
    private void OnClick()
    {
        Value++;
        Greeting = $"You clicked {Value} times!";
    }
}