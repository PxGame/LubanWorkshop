// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.Metrics;
using System;
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;
using System.Reflection;
using Autofac.Core;
using MediatR;
using LB.Core;
using LB.Core.Containers;

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