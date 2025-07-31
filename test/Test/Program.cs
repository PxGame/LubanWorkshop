using Luban.Core;
using Luban.Core.Services.Plugins;
using System;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Test
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            await Utils.Initialize();

            var config = Utils.Services.Plugin.InvokeCommand("com.luban.plugin.common", "GetPluginConfig", null) as IPluginConfig;

            if (config != null)
            {
                Utils.Services.Log.Log.Information($"Plugin Name: {config.Name}");
                Utils.Services.Log.Log.Information($"Plugin Version: {config.Version}");
                Utils.Services.Log.Log.Information($"Plugin Description: {config.Description}");
            }
            else
            {
                Utils.Services.Log.Log.Information("Failed to retrieve plugin configuration.");
            }

            await Utils.Dispose();

            Console.WriteLine("End !");
        }
    }
}