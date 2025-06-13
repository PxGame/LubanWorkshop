using LB.Core.Containers;
using LB.Core.Services.Analyses;
using LB.Core.Services.Logs;
using LB.Core.Services.Plugins;
using LB.Core.Services.Settings;
using LB.Services;
using System;
using System.Collections;
using System.Collections.Generic;

namespace LB.Core.Services
{
    public class ServiceCollection : IServiceCollection
    {
        public IAppEntry AppEntry { get; set; }
        public IContainer Container { get; set; }

        public ILogService Log { get; set; }
        public ISettingService Setting { get; set; }
        public IAnalysisService Analysis { get; set; }
        public IPluginService Plugin { get; set; }

        public Type[] GetServiceTypes()
        {
            return [
                    typeof(ILogService),
                    typeof(ISettingService),
                    typeof(IAnalysisService),
                    typeof(IPluginService)
                ];
        }

        public IEnumerator<IService> GetEnumerator()
        {
            yield return Log;
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