using LB.Core.Containers;
using LB.Core.Services.Logs;

namespace LB.Core.Services.Plugins
{
    public interface IPlugin
    {
        [Inject] IServiceCollection services { get; init; }

        string folder { get; }

        void OnLoad();

        void OnUnload();
    }

    public interface IPlugin<SettingType> : IPlugin
    {
        [Inject]
        IPluginCustomSetting<SettingType> Setting { get; init; }

        [Inject]
        IPluginLog<SettingType> Log { get; init; }
    }
}