using Luban.Core.Containers;
using Luban.Core.Services.Analyses;
using Luban.Core.Services.Logs;
using Luban.Core.Services.Plugins;
using Luban.Core.Services.Settings;
using System;

namespace Luban.Core
{
    public interface IAppEntry : IDisposable
    {
        IContainer Container { get; }
    }
}