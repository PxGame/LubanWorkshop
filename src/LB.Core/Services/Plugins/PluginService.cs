using LB.Core.Containers;
using LB.Core.Services.Analyses;
using LB.Core.Services.Logs;
using LB.Core.Services.Settings;
using LB.Plugin;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

        public string AppDataPluginsFolder { get; private set; }
        public string AppPluginsFolder { get; private set; }

        private List<PluginController> controllers = new List<PluginController>();

        public PluginService()
        {
        }

        public void OnResolved()
        {
            Log.Information($"OnResolved");

            AppDataPluginsFolder = Path.Combine(Utils.AppDataFolder, "Plugins");
            AppPluginsFolder = Path.Combine(Utils.AppFolder, "Plugins");
        }

        public void OnInstanceReleased()
        {
            Log.Information($"OnInstanceReleased");
        }

        public async Task OnServiceInitialize()
        {
            Log.Information($"OnServiceInitialize");

            await LoadAll();
            await Task.CompletedTask;
        }

        private async Task LoadAll()
        {
            await LoadAllFromFolder(AppPluginsFolder);
            await LoadAllFromFolder(AppDataPluginsFolder);
        }

        private async Task LoadAllFromFolder(string pluginsFolder)
        {
            if (!Directory.Exists(pluginsFolder)) { return; }
            var folders = Directory.GetDirectories(pluginsFolder);
            foreach (var folder in folders)
            {
                await LoadPluginFromFolder(folder);
            }
        }

        private async Task<PluginController> LoadPluginFromFolder(string folder)
        {
            return await Load(folder);
        }

        public async Task OnServiceShutdown()
        {
            Log.Information($"OnServiceShutdown");

            await UnloadAll();
            await Task.CompletedTask;
        }

        public async Task<PluginController> Load(string pluginFolder)
        {
            var controller = new PluginController(pluginFolder);
            Container.Inject(controller);
            await controller.Initialize();
            await controller.Load();
            controllers.Add(controller);
            return controller;
        }

        public async Task Unload(PluginController controller)
        {
            if (controller == null) { return; }

            controllers.Remove(controller);
            await controller.Unload();

            await Task.CompletedTask;
        }

        public async Task UnloadAll()
        {
            while (controllers.Count > 0)
            {
                await Unload(controllers[0]);
            }
        }
    }
}