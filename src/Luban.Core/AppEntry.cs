using Luban.Core.Containers;
using Luban.Core.Services;
using Luban.Core.Services.Analyses;
using Luban.Core.Services.Events;
using Luban.Core.Services.Logs;
using Luban.Core.Services.Plugins;
using Luban.Core.Services.Settings;
using Luban.Services;
using Serilog;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Luban.Core
{
    public class AppEntry : IAppEntry
    {
        private bool _disposedValue = false;
        private Container _container = new Container();
        private ServiceCollection _services = new ServiceCollection();

        public IContainer Container => _container;
        public IServiceCollection Services => _services;

        [Inject]
        [Log(Tag = "程序入口")]
        public ILog Log { get; private set; }

        public AppEntry()
        {
            Container.RegisterInstance<AppEntry, IAppEntry>((_, _, _, _) => this);
        }

        public async Task Initialize()
        {
            _services.AppEntry = this;
            _services.Container = Container;

            Container.RegisterInstance<LogService, ILogService>()
                .OnResolved((r, t) => _services.Log = t as ILogService)
                .OnReleased((r) => _services.Log = null);
            Container.RegisterInstance<EventService, IEventService>()
                .OnResolved((r, t) => _services.Event = t as IEventService)
                .OnReleased((r) => _services.Event = null);
            Container.RegisterInstance<SettingService, ISettingService>()
                .OnResolved((r, t) => _services.Setting = t as ISettingService)
                .OnReleased((r) => _services.Setting = null);
            Container.RegisterInstance<AnalysisService, IAnalysisService>()
                .OnResolved((r, t) => _services.Analysis = t as IAnalysisService)
                .OnReleased((r) => _services.Analysis = null);
            Container.RegisterInstance<PluginService, IPluginService>()
                .OnResolved((r, t) => _services.Plugin = t as IPluginService)
                .OnReleased((r) => _services.Plugin = null);

            foreach (var serviceType in Services.GetServiceTypes())
            {
                Container.Resolve(serviceType, [], [], null);
            }
            Container.RegisterInstance<IServiceCollection>((_, _, _, _) => Services);

            Container.Inject(this, null);
            Log.Information($"Initialize start ...");
            foreach (var service in Services)
            {
                await service.OnServiceInitialize();
            }
            Log.Information($"Initialize end .");
        }

        public async Task Shutdown()
        {
            Log.Information($"Shutdown start ...");

            foreach (var service in Services.Reverse())
            {
                await service?.OnServiceShutdown();
            }

            Log.Information($"Shutdown end .");
        }

        #region Dispose

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                }

                Container?.Dispose();

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion Dispose
    }
}