using Luban.Core.Containers;
using Luban.Core.Services.Logs;
using Luban.Core.Services.Plugins;
using Luban.Core.Services.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Core.Services.Analyses
{
    internal class AnalysisService : IAnalysisService
    {
        [Inject]
        [Log(Tag = "诊断服务")]
        private ILog Log { get; init; }

        [Inject]
        private ISettingService Setting { get; init; }

        public void OnInstanceReleased()
        {
            Log.Information($"OnInstanceReleased");
        }

        public void OnResolved()
        {
            Log.Information($"OnResolved");
        }

        public async Task OnServiceInitialize()
        {
            Log.Information($"OnServiceInitialize");
            await Task.CompletedTask;
        }

        public async Task OnServiceShutdown()
        {
            Log.Information($"OnServiceShutdown");
            await Task.CompletedTask;
        }
    }
}