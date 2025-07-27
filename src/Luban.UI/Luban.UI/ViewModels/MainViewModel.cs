using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Luban.Core;
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

        await Task.Run(async () =>
        {
            Dispatcher.UIThread.Post(() => { Greeting = $"Initialize completed ."; });
            await Utils.Initialize();

            var remoteText = await Utils.Services.Storage.ReadFileText(Core.Services.Storages.FileStorageType.RemoteFolder, "./luban.txt");

            Dispatcher.UIThread.Post(() => { Greeting = remoteText != null ? $"Read remote file completed ! => {remoteText}" : "Read remote file failed !"; });
        });
    }
}