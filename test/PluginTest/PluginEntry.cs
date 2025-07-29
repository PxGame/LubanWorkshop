using Luban.Core.Containers;
using Luban.Core.Services;
using Luban.Core.Services.Plugins;
using Luban.Plugin;
using PluginTestDependency;
using System;
using System.Threading.Tasks;

namespace PluginTest
{
    public class PluginEntry : IPluginEntry
    {
        public override async Task OnLoad()
        {
            await base.OnLoad();
            Log.Information($"OnLoad Call Dependency : {new PluginDependency().GetName()}");
        }

        public override async Task OnUnload()
        {
            await base.OnUnload();
            Log.Information($"OnUnload Call Dependency : {new PluginDependency().GetName()}");
        }
    }
}