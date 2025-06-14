using LB.Core.Containers;
using LB.Core.Services;
using LB.Core.Services.Plugins;
using LB.Plugin;
using PluginTestDependency;
using System;
using System.Threading.Tasks;

namespace PluginTest
{
    public class PluginEntry : IPlugin
    {
        public override void OnResolved()
        {
            Console.WriteLine($"OnResolved 5 > {RootFolder} | {this.Config.Name}");
        }

        public override async Task OnLoad()
        {
            Console.WriteLine($"OnLoad 5 > {new PluginDependency().GetName()} | {RootFolder}");
            await Task.CompletedTask;
        }

        public override async Task OnUnload()
        {
            Console.WriteLine($"OnUnload 5 > {new PluginDependency().GetName()} | {RootFolder}");
            await Task.CompletedTask;
        }
    }
}