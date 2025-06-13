using LB.Core.Containers;
using LB.Core.Services.Analyses;
using LB.Core.Services.Logs;
using LB.Core.Services.Plugins;
using LB.Core.Services.Settings;
using LB.Services;
using System;
using System.Collections.Generic;

namespace LB.Core.Services
{
    public interface IServiceCollection : IEnumerable<IService>
    {
        IAppEntry AppEntry { get; }
        IContainer Container { get; }

        IAnalysisService Analysis { get; }
        ILogService Log { get; }
        IPluginService Plugin { get; }
        ISettingService Setting { get; }

        Type[] GetServiceTypes();
    }
}