using Luban.Core;
using Luban.Core.Services.Plugins;
using System;
using System.Collections.Generic;
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

            var result = await Utils.Services.Plugin.InvokeCmdAsync<string>("com.luban.plugin.common", "common",
                "test",
                new Dictionary<string, object>()
                    {
                        { "A", 123 },
                        { "B", "Hello" },
                        { "C", true }
                    }
                );

            Console.WriteLine($"result : {result}");

            await Utils.Dispose();

            Console.WriteLine("End !");
        }
    }
}