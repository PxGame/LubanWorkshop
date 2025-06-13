namespace LB.Core.Services.Plugins
{
    public interface IPluginConfig
    {
        string Author { get; set; }
        string Description { get; set; }
        string DisplayName { get; set; }
        string EntryName { get; set; }
        string GUID { get; set; }
        string Version { get; set; }
    }
}