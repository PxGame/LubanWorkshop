using LB.Core.Containers;
using LB.Core.Services.Analyses;
using LB.Core.Services.Logs;
using LB.Core.Services.Plugins;
using LB.Core.Services.Settings;
using System;

namespace LB.Core
{
    public interface IAppEntry : IDisposable
    {
        IContainer Container { get; }
    }
}