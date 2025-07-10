using Luban.Core.Containers;
using Luban.Plugin;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;

namespace Luban.Core.Services.Plugins
{
    public class PluginController : IOnResolved
    {
        public const string ConfigFileName = "plugin.json";

        [Inject]
        private IContainer Container { get; init; }

        [Inject]
        private IPluginService Serivce { get; init; }

        private string _folder;
        private IPlugin _plugin;
        private PluginConfig _config;
        private PluginLoadContext _pluginLoadContext;

        //private FileSystemWatcher _watcher;

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

            _plugin = Container.Resolve(entryType, [], [],
                new InjectExtraPropertyValue() {
                    { "RootFolder", _folder},
                    { "Config", _config},
                }) as IPlugin;

            await Task.CompletedTask;
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