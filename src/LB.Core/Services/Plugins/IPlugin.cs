using LB.Core.Containers;
using LB.Core.Services.Logs;
using LB.Core.Services.Settings;
using System.Threading.Tasks;

namespace LB.Core.Services.Plugins
{
    public interface IPlugin
    {
        [Inject] IServiceCollection services { get; init; }

        string folder { get; }
        IPluginConfig config { get; }

        Task OnLoad();

        Task OnUnload();
    }

    public interface IPlugin<SettingType, PluginType> : IPlugin
    {
        [Inject]
        [CustomSetting("setting.json", SubPath = "")]
        IPluginCustomSetting<SettingType> UserSetting { get; init; }

        [Inject]
        IPluginLog<PluginType> Log { get; init; }
    }
}