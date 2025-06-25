using Luban.Core.Containers;
using Luban.Core.Services.Analyses;
using Luban.Core.Services.Logs;
using Luban.Core.Services.Plugins;
using Luban.Core.Services.Settings;
using Luban.Services;
using System;
using System.Collections.Generic;

namespace Luban.Core.Services
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