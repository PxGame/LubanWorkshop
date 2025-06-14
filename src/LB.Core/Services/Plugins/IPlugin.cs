using LB.Core.Containers;
using LB.Core.Services.Logs;
using LB.Core.Services.Settings;
using System.Threading.Tasks;

namespace LB.Core.Services.Plugins
{
    public abstract class IPlugin : IOnResolved
    {
        [Inject] public IServiceCollection services { get; init; }
        [Inject] public string RootFolder { get; init; }
        [Inject] public IPluginConfig Config { get; init; }

        public abstract void OnResolved();

        public abstract Task OnLoad();

        public abstract Task OnUnload();
    }
}