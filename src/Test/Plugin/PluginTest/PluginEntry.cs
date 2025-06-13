using LB.Core.Containers;
using LB.Core.Services;
using LB.Core.Services.Plugins;
using LB.Plugin;
using PluginTestDependency;
using System;

namespace PluginTest
{
    public class PluginEntry : IPlugin
    {
        public string folder { get; set; }
        public IServiceCollection services { get; init; }

        public void OnLoad()
        {
            Console.WriteLine($"OnLoad 5 > {new PluginDependency().GetName()}");
        }

        public void OnUnload()
        {
            Console.WriteLine($"OnUnload 5 > {new PluginDependency().GetName()}");
        }
    }
}