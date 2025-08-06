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
        public class ResultData
        {
            public int a3;
            public string b3;
            public bool c3;

            public override string ToString()
            {
                return $"a3: {a3}, b3: {b3}, c3: {c3}";
            }
        }

        private static async Task Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            await Utils.Initialize();

            //var result = await Utils.Services.Plugin.InvokeCmdAsync<string>("com.luban.plugin.common", "common",
            //    "test",
            //    new Dictionary<string, object>()
            //        {
            //            { "A", 123 },
            //            { "B", "Hello" },
            //            { "C", true }
            //        }
            //    );

            //Console.WriteLine($"result : {result}");

            try
            {
                var result2 = await Utils.Services.Plugin.InvokeCmdAsync<ResultData>("com.luban.plugin.common", "common",
                    "test2",
                    new Dictionary<string, object>()
                        {
                        { "a", 123 },
                        { "b", null },
                        { "C2", true }
                        }
                    );
                Console.WriteLine($"result2 : {result2}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            await Utils.Dispose();

            Console.WriteLine("End !");
        }
    }
}