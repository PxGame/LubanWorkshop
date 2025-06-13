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
        public string folder { get; set; }
        public IServiceCollection services { get; init; }

        public async Task OnLoad()
        {
            Console.WriteLine($"OnLoad 5 > {new PluginDependency().GetName()} | {folder}");
            await Task.CompletedTask;
        }

        public async Task OnUnload()
        {
            Console.WriteLine($"OnUnload 5 > {new PluginDependency().GetName()} | {folder}");
            await Task.CompletedTask;
        }
    }
}