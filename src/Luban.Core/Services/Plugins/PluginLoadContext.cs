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
        private readonly string _pluginFolder;

        private readonly PluginConfig _pluginConfig;
        private readonly string _mainAssemblyPath;
        private readonly AssemblyDependencyResolver _dependencyResolver;

        public PluginLoadContext(
            string pluginFolder,
            PluginConfig pluginConfig
            ) : base(pluginConfig.Name, true)
        {
            _pluginFolder = pluginFolder;
            _pluginConfig = pluginConfig;

            _mainAssemblyPath = Utils.PathCombine(_pluginFolder, _pluginConfig.EntryName + ".dll");
            _dependencyResolver = new AssemblyDependencyResolver(_mainAssemblyPath);
        }

        public (Type entryType, List<Type> cmdTypes) LoadMainTypes()
        {
            var mainAssembly = LoadAssemblyFromFilePath(_mainAssemblyPath);

            Type pluginEntryType = null;
            List<Type> pluginCommandTypes = new List<Type>();

            var types = mainAssembly.GetTypes();

            for (int i = 0; i < types.Length; i++)
            {
                var type = types[i];
                if (typeof(IPluginEntry).IsAssignableFrom(type))
                {
                    pluginEntryType = type;
                }
                if (type.GetCustomAttribute<PCmdGroupAttribute>() != null)
                {
                    pluginCommandTypes.Add(type);
                }
            }

            return (pluginEntryType, pluginCommandTypes);
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            if (AssemblyLoadContext.Default != null)
            {
                try
                {
                    var defaultAssembly = AssemblyLoadContext.Default.LoadFromAssemblyName(assemblyName);
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