using Luban.Core.Containers;
using Luban.Core.Services.Analyses;
using Luban.Core.Services.Events;
using Luban.Core.Services.Logs;
using Luban.Core.Services.Plugins;
using Luban.Core.Services.Settings;
using Luban.Core.Services.Storages;
using Luban.Services;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Luban.Core.Services
{
    public class ServiceCollection : IServiceCollection
    {
        public ILogService Log { get; set; }
        public IEventService Event { get; set; }
        public ISettingService Setting { get; set; }
        public IStorageService Storage { get; set; }
        public IAnalysisService Analysis { get; set; }
        public IPluginService Plugin { get; set; }

        public Type[] GetServiceTypes()
        {
            return [
                    typeof(ILogService),
                    typeof(IEventService),
                    typeof(ISettingService),
                    typeof(IStorageService),
                    typeof(IAnalysisService),
                    typeof(IPluginService)
                ];
        }

        public IEnumerator<IService> GetEnumerator()
        {
            yield return Log;
            yield return Event;
            yield return Setting;
            yield return Storage;
            yield return Analysis;
            yield return Plugin;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}