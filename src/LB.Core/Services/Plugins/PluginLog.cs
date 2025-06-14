using LB.Core.Services.Logs;
using Serilog;

namespace LB.Core.Services.Plugins
{
    public interface IPluginLog<T> : ILog<T>
    {
    }

    internal class PluginLog<T> : Log<T>, IPluginLog<T>
    {
        public PluginLog(ILogger rootLogger, string tag) : base(rootLogger, tag)
        {
        }
    }
}