using Luban.Services;

namespace Luban.Core.Services.Plugins
{
    public abstract class IPluginService : IService
    {
        public abstract object InvokeCommand(string pluginName, string cmdName, object[] args);
    }
}