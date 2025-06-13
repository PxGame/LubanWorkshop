using LB.Core.Containers;
using LB.Core.Services.Analyses;
using LB.Core.Services.Logs;
using LB.Core.Services.Settings;
using LB.Plugin;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace LB.Core.Services.Plugins
{
    public interface IPluginCustomSetting<T> : ICustomSetting<T>
    {
    }

    internal class PluginCustomSetting<T> : CustomSetting<T>, IPluginCustomSetting<T>
    {
        public PluginCustomSetting(string relativePath) : base(relativePath, false)
        {
        }
    }

    public interface IPluginLog<T> : ILog<T>
    {
    }

    internal class PluginLog<T> : Log<T>, IPluginLog<T>
    {
        public PluginLog(ILogger rootLogger, string tag) : base(rootLogger, tag)
        {
        }
    }

    public class PluginService : IPluginService, IOnResolved
    {
        [Inject]
        private IContainer Container { get; init; }

        [Inject]
        [LogInfo(Tag = "插件服务")]
        private ILog Log { get; init; }

        [Inject]
        private ISettingService Setting { get; init; }

        [Inject]
        private IAnalysisService Analysis { get; init; }

        private List<PluginController> controllers = new List<PluginController>();

        public PluginService()
        {
        }

        public void OnResolved()
        {
            Log.Information($"OnResolved");
        }

        public void OnInstanceReleased()
        {
            Log.Information($"OnInstanceReleased");
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

        public PluginController Load(string pluginFolder)
        {
            var controller = new PluginController(pluginFolder);
            Container.Inject(controller);
            controller.Initialize();
            controllers.Add(controller);
            controller.Load();
            return controller;
        }

        public bool Unload(PluginController controller)
        {
            if (controller == null) { return false; }

            controllers.Remove(controller);
            controller.Unload();

            return true;
        }

        public void UnloadAll()
        {
            while (controllers.Count > 0)
            {
                Unload(controllers[0]);
            }
        }
    }
}