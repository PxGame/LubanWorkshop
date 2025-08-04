using Luban.Core.Containers;
using Luban.Core.Services.Analyses;
using Luban.Core.Services.Logs;
using Luban.Core.Services.Settings;
using Luban.Plugin;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Luban.Core.Services.Plugins
{
    internal class PluginService : IPluginService, IOnResolved
    {
        private ISettingService Setting { get; set; }
        private IAnalysisService Analysis { get; set; }

        private List<PluginController> controllers = new List<PluginController>();
        private Dictionary<string, PluginController> name2Ctrl = new Dictionary<string, PluginController>();

        public PluginService()
        {
        }

        public override object InvokeCommand(string pluginName, string cmdName, IReadOnlyDictionary<string, object> args)
        {
            if (!name2Ctrl.TryGetValue(pluginName, out var controller)) { return null; }

            Dictionary<string, JObject> jsonArgs = new Dictionary<string, JObject>();

            if (args != null)
            {
                foreach (var kv in args)
                {
                    jsonArgs[kv.Key] = JObject.FromObject(kv.Value);
                }
            }

            return controller.InvokeCommand(cmdName, cmdName, jsonArgs);
        }

        public override void OnResolved()
        {
            Container.RegisterType<PluginController>();
            Container.RegisterType(typeof(IPluginEntry), OnCreatePlugin, false, null, true);
        }

        private object OnCreatePlugin(IRegistration regist, Type type, List<object> extraInfos, object[] args)
        {
            return Activator.CreateInstance(type);
        }

        public override void OnInstanceReleased()
        {
            Log.Information($"OnInstanceReleased");
        }

        public override async Task OnServiceInitialing()
        {
            await base.OnServiceInitialing();
            Setting = Container.Resolve<ISettingService>();
            Analysis = Container.Resolve<IAnalysisService>();

            await Task.CompletedTask;
        }

        public override async Task OnServiceInitialized()
        {
            await base.OnServiceInitialized();
            Log.Information($"OnServiceInitialized");

            if (!Utils.IsWeb)
            {
                await LoadAll();
            }
            await Task.CompletedTask;
        }

        private async Task LoadAll()
        {
            await LoadAllFromFolder(Utils.AppPluginsFolder);
            await LoadAllFromFolder(Utils.UserPluginsFolder);
            await Task.CompletedTask;
        }

        private async Task LoadAllFromFolder(string pluginsFolder)
        {
            if (!Directory.Exists(pluginsFolder)) { return; }
            var folders = Directory.GetDirectories(pluginsFolder);
            foreach (var folder in folders)
            {
                await LoadPluginFromFolder(folder.StandardizedPath());
            }
        }

        private async Task<PluginController> LoadPluginFromFolder(string folder)
        {
            return await Load(folder);
        }

        public override async Task OnServiceShutdown()
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
            name2Ctrl[controller.PluginName] = controller;
            return controller;
        }

        public async Task Unload(PluginController controller)
        {
            name2Ctrl.Remove(controller.PluginName);
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