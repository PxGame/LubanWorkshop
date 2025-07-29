using Luban.Core;
using System;
using System.Threading.Tasks;

namespace Test
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            await Utils.Initialize();
            await Utils.Dispose();
        }
    }
}