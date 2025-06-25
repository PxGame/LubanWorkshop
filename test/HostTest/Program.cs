// See https://aka.ms/new-console-template for more information
using System.Threading.Tasks;
using Luban.Core;
using Luban.Core.Containers;

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
        AppEntry entry = new AppEntry();

        await entry.Initialize();
        await entry.Shutdown();

        entry.Dispose();
    }
}