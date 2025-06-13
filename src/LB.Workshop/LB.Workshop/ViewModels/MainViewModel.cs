using CommunityToolkit.Mvvm.ComponentModel;

namespace LB.Workshop.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _greeting = "Welcome to Avalonia!";
}
