using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Luban.Core;
using System.Runtime.InteropServices.JavaScript;
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
        await Task.Run(async () =>
        {
            Dispatcher.UIThread.Post(() => { Greeting = $"AppEntry start ."; });
            await Utils.Initialize();
            Dispatcher.UIThread.Post(() => { Greeting = $"Initialize completed ."; });
            await Utils.Dispose();
            Dispatcher.UIThread.Post(() => { Greeting = $"Dispose completed ."; });
        });
    }
}