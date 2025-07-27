// See https://aka.ms/new-console-template for more information
using Luban.Core;
using Luban.Core.Containers;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

internal class Program
{
    public class A
    {
        private string msg;

        public A(string msg)
        {
            this.msg = msg;
        }
    }

    public class B
    {
        [Inject]
        public A a { get; init; }

        public B()
        {
        }
    }

    public class C
    {
        [Inject]
        public A a { get; init; }

        [Inject]
        public B b { get; init; }

        [Inject]
        public IContainer Container { get; init; }

        public C()
        {
        }
    }

    private static async Task Main(string[] args)
    {
        await Utils.Initialize();
        await Utils.Dispose();

        //JObject obj = new JObject();

        //obj["a"] = "hello";
        //obj["b"] = 1111;
        //obj["c"] = new JObject
        //{
        //    ["d"] = "test",
        //    ["e"] = 123
        //};

        //var str = obj.ToString();
        //Console.WriteLine(str);

        //var obj2 = JObject.Parse(str);
        //obj2["a"] = "xxxx";

        //Console.WriteLine(obj2["c.d"]);

        await Task.CompletedTask;
    }
}