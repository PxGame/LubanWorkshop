using Luban.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Luban.Core.Services.Plugins
{
    public abstract class IPluginService : IService
    {
        public abstract Task<T> InvokeCmdAsync<T>(string pluginName, string groupName, string cmdName, Dictionary<string, object> args);
    }
}