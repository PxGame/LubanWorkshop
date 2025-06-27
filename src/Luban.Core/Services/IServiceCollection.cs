using Luban.Core.Containers;
using Luban.Core.Services.Analyses;
using Luban.Core.Services.Events;
using Luban.Core.Services.Logs;
using Luban.Core.Services.Plugins;
using Luban.Core.Services.Settings;
using Luban.Core.Services.Storages;
using Luban.Services;
using System;
using System.Collections.Generic;

namespace Luban.Core.Services
{
    public interface IServiceCollection : IEnumerable<IService>
    {
        IAnalysisService Analysis { get; }
        ILogService Log { get; }
        IEventService Event { get; set; }
        IPluginService Plugin { get; }
        ISettingService Setting { get; }
        IStorageService Storage { get; set; }

        Type[] GetServiceTypes();
    }
}