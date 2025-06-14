using LB.Core.Services.Settings;

namespace LB.Core.Services.Plugins
{
    public interface IPluginCustomSetting<T> : ICustomSetting<T>
    {
    }

    internal class PluginCustomSetting<T> : CustomSetting<T>, IPluginCustomSetting<T>
    {
        public PluginCustomSetting(string relativePath, bool isAppFolder) : base(relativePath, isAppFolder)
        {
        }
    }
}