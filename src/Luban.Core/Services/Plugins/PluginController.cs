using Luban.Core.Containers;
using Luban.Plugin;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Luban.Core.Services.Plugins
{
    internal class PluginCommandRet
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    internal class PluginCommandArg
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    internal class PluginCommand
    {
        public MethodInfo Method { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public List<PluginCommandArg> Args { get; set; }
        public PluginCommandRet Ret { get; set; }
    }

    internal class PluginCommandGroup
    {
        public object Target { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<PluginCommand> Commands { get; } = new List<PluginCommand>();
    }

    internal class PluginController : IOnResolved
    {
        public const string ConfigFileName = "plugin.json";

        public string PluginName => _config?.Name;

        [Inject]
        private IContainer Container { get; init; }

        [Inject]
        private IPluginService Serivce { get; init; }

        private string _folder;
        private PluginConfig _config;
        private PluginLoadContext _pluginLoadContext;

        private IPlugin _plugin;
        private List<PluginCommandGroup> _pluginCmdGroups;

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

            _pluginLoadContext.InitializeMainAssembly();
            if (_pluginLoadContext.PluginEntryType == null) { throw null; }

            // cmd groups
            _pluginCmdGroups = LoadPluginCmdGroups(_pluginLoadContext.PluginCommandTypes.ToList());

            // plugin
            _plugin = Container.Resolve(_pluginLoadContext.PluginEntryType, [], [],
                new InjectExtraPropertyValue() {
                    { "RootFolder", _folder},
                    { "Config", _config},
                }) as IPlugin;

            await Task.CompletedTask;
        }

        private List<PluginCommandGroup> LoadPluginCmdGroups(List<Type> cmdGroupTypes)
        {
            var pluginCmdGroups = new List<PluginCommandGroup>();
            foreach (var pCmdType in cmdGroupTypes)
            {
                //cmd group
                var pCmdGroup = LoadPluginCmdGroup(pCmdType);
                if (pCmdGroup == null) { continue; }

                pluginCmdGroups.Add(pCmdGroup);
            }

            return pluginCmdGroups;
        }

        private PluginCommandGroup LoadPluginCmdGroup(Type pCmdType)
        {
            var pCmdGroupAttr = pCmdType.GetCustomAttribute<PCmdGroupAttribute>();
            if (pCmdGroupAttr == null) { return null; }

            var pCmdGroup = new PluginCommandGroup()
            {
                Target = Activator.CreateInstance(pCmdType),
                Name = pCmdGroupAttr?.Name ?? pCmdType.Name,
                Description = pCmdGroupAttr?.Description ?? pCmdType.Name
            };

            //cmds
            var methods = pCmdType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var method in methods)
            {
                PluginCommand pCmd = LoadPluginCmd(method);
                if (pCmd == null) { continue; }
                pCmdGroup.Commands.Add(pCmd);
            }

            return pCmdGroup;
        }

        private PluginCommand LoadPluginCmd(MethodInfo method)
        {
            //cmd
            var pCmdAttr = method.GetCustomAttribute<PCmdAttribute>();
            if (pCmdAttr == null) { return null; }
            var pCmd = new PluginCommand()
            {
                Name = pCmdAttr?.Name ?? method.Name,
                Description = pCmdAttr?.Description ?? method.Name,
            };

            //ret
            pCmd.Ret = LoadPluginCmdRet(method.ReturnParameter);

            //args
            pCmd.Args = LoadPluginCmdArgs(method.GetParameters());

            return pCmd;
        }

        private static PluginCommandRet LoadPluginCmdRet(ParameterInfo retParam)
        {
            var pCmdRetAttr = retParam.GetCustomAttribute<PCmdRetAttribute>();
            var pCmdRet = new PluginCommandRet()
            {
                Name = pCmdRetAttr.Name,
                Description = pCmdRetAttr.Description,
            };
            return pCmdRet;
        }

        private static List<PluginCommandArg> LoadPluginCmdArgs(ParameterInfo[] args)
        {
            var pCmdArgs = new List<PluginCommandArg>();
            foreach (var arg in args)
            {
                //arg
                var pCmdArgAttr = arg.GetCustomAttribute<PCmdArgAttribute>();
                var pCmdArg = new PluginCommandArg()
                {
                    Name = pCmdArgAttr?.Name ?? arg.Name,
                    Description = pCmdArgAttr.Description ?? arg.Name,
                };

                pCmdArgs.Add(pCmdArg);
            }

            return pCmdArgs;
        }

        public object InvokeCommand(string groupName, string cmdName, IReadOnlyDictionary<string, JObject> name2value)
        {
            var cmdGroup = _pluginCmdGroups.Find(t => t.Name == groupName);
            if (cmdGroup == null) { return null; }

            var cmd = cmdGroup.Commands.Find(t => t.Name == cmdName);
            return null;
            //if (!_pluginCmdDict.TryGetValue(name, out var method)) { return null; }
            //var paramInfos = method.GetParameters();
            //var args = new object[paramInfos.Length];

            //for (int i = 0; i < paramInfos.Length; i++)
            //{
            //    var paramInfo = paramInfos[i];
            //    object argValue = null;

            //    if (name2value.TryGetValue(paramInfo.Name, out var jsonValue))
            //    {
            //        argValue = jsonValue.ToObject(paramInfo.ParameterType);
            //    }

            //    if (argValue == null)
            //    {
            //        argValue = paramInfo.DefaultValue ?? Activator.CreateInstance(paramInfo.ParameterType);
            //    }

            //    args[i] = argValue;
            //}

            //return method.Invoke(_plugin, args);
        }

        public async Task Load()
        {
            await _plugin.OnLoad();
        }

        public async Task Unload()
        {
            await _plugin.OnUnload();
            _plugin = null;
            _pluginCmdGroups = null;

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