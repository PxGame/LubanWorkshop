using LB.Core.Services.Plugins;

namespace LB.Plugin
{
    public class PluginConfig : IPluginConfig
    {
        public string EntryName { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }
        public string Author { get; set; }
        public string DisplayName { get; set; }
        public string GUID { get; set; }
    }
}