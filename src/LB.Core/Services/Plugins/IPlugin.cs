using LB.Core.Containers;
using LB.Core.Services.Logs;
using LB.Core.Services.Settings;
using System.Threading.Tasks;

namespace LB.Core.Services.Plugins
{
    public abstract class IPlugin
    {
        [Inject] public IServiceCollection services { get; init; }
        [Inject] public string RootFolder { get; init; }
        [Inject] public IPluginConfig Config { get; init; }

        public abstract Task OnLoad();

        public abstract Task OnUnload();
    }

    public abstract class IPlugin<SettingType, PluginType> : IPlugin
    {
        [Inject]
        [CustomSetting("setting.json", SubPath = "")]
        public IPluginCustomSetting<SettingType> UserSetting { get; init; }

        [Inject]
        public IPluginLog<PluginType> Log { get; init; }
    }
}