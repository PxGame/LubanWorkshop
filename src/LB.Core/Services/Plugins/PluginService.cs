using LB.Core.Containers;
using LB.Core.Services.Analyses;
using LB.Core.Services.Logs;
using LB.Core.Services.Settings;
using LB.Plugin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace LB.Core.Services.Plugins
{
    internal class PluginService : IPluginService, IOnResolved
    {
        [Inject]
        private IContainer Container { get; init; }

        [Inject]
        [Log(Tag = "插件服务")]
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

            Container.RegisterType<PluginController>();
            Container.RegisterType(typeof(IPlugin), OnCreatePlugin, false, null, true);
        }

        private object OnCreatePlugin(IRegistration regist, Type type, List<object> extraInfos, object[] args)
        {
            return Activator.CreateInstance(type);
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
            await LoadAllFromFolder(Utils.AppPluginsFolder);
            await LoadAllFromFolder(Utils.AppDataPluginsFolder);
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
            var controller = Container.Resolve<PluginController>(null, [pluginFolder]);
            await controller.Initialize();
            await controller.Load();
            controllers.Add(controller);
            return controller;
        }

        public async Task Unload(PluginController controller)
        {
            controllers.Remove(controller);
            if (controller == null) { return; }
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