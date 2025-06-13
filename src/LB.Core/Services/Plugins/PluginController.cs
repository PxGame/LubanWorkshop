using LB.Core.Containers;
using LB.Plugin;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text.Json;

namespace LB.Core.Services.Plugins
{
    public class PluginController : IOnResolved
    {
        public const string ConfigFileName = "config.json";

        [Inject]
        private IContainer Container { get; init; }

        [Inject]
        private IPluginService Serivce { get; init; }

        private string _folder;
        private AssemblyLoadContext _context;
        private IPlugin _plugin;
        private PluginConfig _config;

        //private FileSystemWatcher _watcher;

        public PluginController(string pluginFolder)
        {
            _folder = pluginFolder;
        }

        public void OnResolved()
        {
        }

        public void Initialize()
        {
            var configPath = Path.Combine(_folder, ConfigFileName);
            var configJsonStr = File.ReadAllText(configPath);
            _config = JsonSerializer.Deserialize<PluginConfig>(configJsonStr);

            var entryPath = Path.Combine(_folder, $"{_config.EntryName}.dll");
            if (!File.Exists(entryPath)) { throw null; }

            _context = new AssemblyLoadContext(_config.GUID, true);
            _context.Resolving += OnContextResolver;
            using var entryStream = File.OpenRead(entryPath);
            var entryAssembly = _context.LoadFromStream(entryStream);

            var entryType = entryAssembly.GetTypes().FirstOrDefault(t => t.IsClass && typeof(IPlugin).IsAssignableFrom(t), null);
            if (entryType == null) { throw null; }

            _plugin = Activator.CreateInstance(entryType) as IPlugin;
            Container.Inject(_plugin);
        }

        private Assembly OnContextResolver(AssemblyLoadContext context, AssemblyName name)
        {
            var path = Path.Combine(_folder, $"{name.Name}.dll");
            if (File.Exists(path))
            {
                using (var assemblyStream = File.OpenRead(path))
                {
                    return context.LoadFromStream(assemblyStream);
                }
            }

            return null;
        }

        public void Load()
        {
            _plugin.OnLoad();
        }

        public void Unload()
        {
            _plugin.OnUnload();
            _plugin = null;

            var contextWeak = new WeakReference(_context);
            _context.Unload();
            _context = null;

            for (int i = 0; contextWeak.IsAlive && i < 10; i++)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }
    }
}