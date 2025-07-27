using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;

namespace Luban.UI.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _greeting = "Welcome to Avalonia!";

    private int _count;

    [RelayCommand]
    private void OnAddCountClick()
    {
        _count++;
        Greeting = $"You clicked {_count} times.";
    }

    [RelayCommand]
    private async Task OnReadRemote()
    {
        Greeting = "start ...";
        for (int i = 0; i < 4; i++)
        {
            await Task.Delay(1000);

            Greeting = $"wait {i}s ...";
        }

        Greeting = "completed !";
    }
}