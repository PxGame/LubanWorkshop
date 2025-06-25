using Luban.Core.Containers;
using Luban.Core.Services;
using Luban.Core.Services.Plugins;
using Luban.Plugin;
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
            Log.Information($"OnResolved 5 > {RootFolder} | {this.Config.Name} | {UserSetting.Data.Name}");

            UserSetting.Data.Name = UserSetting.Data.Name + "0";
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