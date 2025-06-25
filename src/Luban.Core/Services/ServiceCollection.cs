using Luban.Core.Containers;
using Luban.Core.Services.Analyses;
using Luban.Core.Services.Events;
using Luban.Core.Services.Logs;
using Luban.Core.Services.Plugins;
using Luban.Core.Services.Settings;
using Luban.Services;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Luban.Core.Services
{
    public class ServiceCollection : IServiceCollection
    {
        public IAppEntry AppEntry { get; set; }
        public IContainer Container { get; set; }

        public ILogService Log { get; set; }
        public IEventService Event { get; set; }
        public ISettingService Setting { get; set; }
        public IAnalysisService Analysis { get; set; }
        public IPluginService Plugin { get; set; }

        public Type[] GetServiceTypes()
        {
            return [
                    typeof(ILogService),
                    typeof(IEventService),
                    typeof(ISettingService),
                    typeof(IAnalysisService),
                    typeof(IPluginService)
                ];
        }

        public IEnumerator<IService> GetEnumerator()
        {
            yield return Log;
            yield return Event;
            yield return Setting;
            yield return Analysis;
            yield return Plugin;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}