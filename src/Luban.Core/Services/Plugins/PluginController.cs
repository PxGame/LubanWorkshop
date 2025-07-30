using Luban.Core.Containers;
using Luban.Plugin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Luban.Core.Services.Plugins
{
    public class PluginController : IOnResolved
    {
        public const string ConfigFileName = "plugin.json";

        public string PluginName => _config?.Name;

        [Inject]
        private IContainer Container { get; init; }

        [Inject]
        private IPluginService Serivce { get; init; }

        private string _folder;
        private IPlugin _plugin;
        private PluginConfig _config;
        private PluginLoadContext _pluginLoadContext;
        private Dictionary<string, MethodInfo> _pluginCmdDict;

        public PluginController(string pluginFolder)
        {
            _folder = pluginFolder;
        }

        public void OnResolved()
        {
        }

        public async Task Initialize()
        {
            var configPath = Utils.PathCombine(_folder, ConfigFileName);
            if (!File.Exists(configPath)) { throw null; }
            var configJsonStr = File.ReadAllText(configPath);
            _config = JsonConvert.DeserializeObject<PluginConfig>(configJsonStr);

            _pluginLoadContext = new PluginLoadContext(
                _folder,
                _config
            );

            var entryType = _pluginLoadContext.LoadPluginEntryType();
            if (entryType == null) { throw null; }

            var methods = entryType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            _pluginCmdDict = new Dictionary<string, MethodInfo>();
            foreach (var method in methods)
            {
                var pluginCmdAttr = method.GetCustomAttribute<PluginCommandAttribute>(true);
                if (pluginCmdAttr == null) { continue; }

                var name = string.IsNullOrEmpty(pluginCmdAttr.Name) ? method.Name : pluginCmdAttr.Name;
                _pluginCmdDict[name] = method;
            }

            _plugin = Container.Resolve(entryType, [], [],
                new InjectExtraPropertyValue() {
                    { "RootFolder", _folder},
                    { "Config", _config},
                }) as IPlugin;

            await Task.CompletedTask;
        }

        public object InvokeCommand(string name, object[] args)
        {
            if (!_pluginCmdDict.TryGetValue(name, out var method)) { return null; }
            return method.Invoke(_plugin, args);
        }

        public async Task Load()
        {
            await _plugin.OnLoad();
        }

        public async Task Unload()
        {
            await _plugin.OnUnload();
            _plugin = null;

            var contextWeak = new WeakReference(_pluginLoadContext);
            _pluginLoadContext.Unload();
            _pluginLoadContext = null;

            for (int i = 0; contextWeak.IsAlive && i < 10; i++)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            await Task.CompletedTask;
        }
    }
}