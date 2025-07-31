using Luban.Services;
using System.Collections.Generic;

namespace Luban.Core.Services.Plugins
{
    public abstract class IPluginService : IService
    {
        public abstract object InvokeCommand(string pluginName, string cmdName, IReadOnlyDictionary<string, object> args);
    }
}