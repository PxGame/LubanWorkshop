using Luban.Plugin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Core.Services.Plugins
{
    internal class PluginLoadContext : AssemblyLoadContext
    {
        private readonly AssemblyLoadContext _defaultLoadContext;
        private readonly string _appFolder;
        private readonly string _pluginFolder;
        private readonly PluginConfig _pluginConfig;
        private readonly string _mainAssemblyPath;
        private readonly AssemblyDependencyResolver _dependencyResolver;

        public PluginLoadContext(
            string pluginFolder,
            PluginConfig pluginConfig
            ) : base(pluginConfig.Name, true)
        {
            _defaultLoadContext = AssemblyLoadContext.GetLoadContext(Assembly.GetExecutingAssembly()) ?? AssemblyLoadContext.Default;

            _pluginFolder = pluginFolder;

            _pluginConfig = pluginConfig;

            _mainAssemblyPath = Utils.PathCombine(_pluginFolder, _pluginConfig.EntryName + ".dll");
            _dependencyResolver = new AssemblyDependencyResolver(_mainAssemblyPath);
        }

        public Type LoadPluginEntryType()
        {
            var mainAssembly = LoadAssemblyFromFilePath(_mainAssemblyPath);

            var pluginType = mainAssembly.GetTypes()
                .FirstOrDefault(t => t.IsClass && typeof(IPlugin).IsAssignableFrom(t), null);

            return pluginType;
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            if (_defaultLoadContext != null)
            {
                try
                {
                    var defaultAssembly = _defaultLoadContext.LoadFromAssemblyName(assemblyName);
                    if (defaultAssembly != null)
                    {
                        return defaultAssembly;
                    }
                }
                catch (Exception)
                {
                }
            }

            var resolvedPath = _dependencyResolver.ResolveAssemblyToPath(assemblyName);
            if (!string.IsNullOrEmpty(resolvedPath) && File.Exists(resolvedPath))
            {
                return LoadAssemblyFromFilePath(resolvedPath);
            }

            return null;
        }

        public Assembly LoadAssemblyFromFilePath(string path)
        {
            using var file = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            var pdbPath = Path.ChangeExtension(path, ".pdb");
            if (File.Exists(pdbPath))
            {
                using var pdbFile = File.Open(pdbPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                return LoadFromStream(file, pdbFile);
            }
            return LoadFromStream(file);
        }
    }
}