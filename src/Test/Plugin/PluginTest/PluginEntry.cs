using LB.Core.Containers;
using LB.Core.Services;
using LB.Core.Services.Plugins;
using LB.Plugin;
using PluginTestDependency;
using System;
using System.Threading.Tasks;

namespace PluginTest
{
    public class PluginUserSetting
    {
        public string Name { get; set; } = "PluginTest";
    }

    public class PluginEntry : IPlugin<PluginUserSetting>
    {
        public override void OnResolved()
        {
            Log.Information($"OnResolved 5 > {RootFolder} | {this.Config.Name}");
        }

        public override async Task OnLoad()
        {
            Log.Information($"OnLoad 5 > {new PluginDependency().GetName()} | {RootFolder}");
            await Task.CompletedTask;
        }

        public override async Task OnUnload()
        {
            await base.OnUnload();
            Log.Information($"OnUnload 5 > {new PluginDependency().GetName()} | {RootFolder}");
            await Task.CompletedTask;
        }
    }
}