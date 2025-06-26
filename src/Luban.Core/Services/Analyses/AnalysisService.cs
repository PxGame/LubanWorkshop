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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Luban.Core.Services.Analyses
{
    internal class AnalysisService : IAnalysisService
    {
        private ILog Log { get; set; }
        private ISettingService Setting { get; set; }

        public override void OnInstanceReleased()
        {
            Log.Information($"OnInstanceReleased");
        }

        public override void OnResolved()
        {
        }

        public override async Task OnServiceInitialing()
        {
            await Task.CompletedTask;
        }

        public override async Task OnServiceInitialized()
        {
            Log = Container.Resolve<ILog>([new LogAttribute() { Tag = "诊断服务" }]);
            Log.Information($"OnServiceInitialized");
            Setting = Container.Resolve<ISettingService>();

            await Task.CompletedTask;
        }

        public override async Task OnServiceShutdown()
        {
            Log.Information($"OnServiceShutdown");
            await Task.CompletedTask;
        }
    }
}