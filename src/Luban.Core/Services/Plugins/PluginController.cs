using Luban.Core.Containers;
using Luban.Plugin;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Luban.Core.Services.Plugins
{
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

        private JsonSerializerSettings _jsonSettings;
        private JsonSerializer _jsonSerializer;

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

            _jsonSettings = new JsonSerializerSettings()
            {
                ContractResolver = new DefaultContractResolver()
            };
            _jsonSerializer = JsonSerializer.Create(_jsonSettings);

            _pluginLoadContext = new PluginLoadContext(
                _folder,
                _config
            );

            using (_pluginLoadContext.EnterContextualReflection())
            {
                var mainTypes = _pluginLoadContext.LoadMainTypes();
                if (mainTypes.entryType == null) { throw null; }

                // cmd groups
                _pluginCmdGroups = LoadPluginCmdGroups(mainTypes.cmdTypes);

                // plugin
                _plugin = Container.Resolve(mainTypes.entryType, [], [],
                    new InjectExtraPropertyValue() {
                    { "RootFolder", _folder},
                    { "Config", _config},
                    }) as IPlugin;
            }

            await Task.CompletedTask;
        }

        private List<PluginCommandGroup> LoadPluginCmdGroups(List<Type> cmdGroupTypes)
        {
            var pluginCmdGroups = new List<PluginCommandGroup>();

            for (int i = 0; i < cmdGroupTypes.Count; i++)
            {
                var pCmdType = cmdGroupTypes[i];
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
            var methods = pCmdType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            for (var i = 0; i < methods.Length; i++)
            {
                var method = methods[i];
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
                Method = method,
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
                Name = pCmdRetAttr?.Name ?? retParam.Name,
                Description = pCmdRetAttr?.Description ?? retParam.Name,
                Type = retParam.ParameterType
            };
            return pCmdRet;
        }

        private static List<PluginCommandArg> LoadPluginCmdArgs(ParameterInfo[] args)
        {
            var pCmdArgs = new List<PluginCommandArg>();
            for (int i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                //arg
                var pCmdArgAttr = arg.GetCustomAttribute<PCmdArgAttribute>();
                var pCmdArg = new PluginCommandArg()
                {
                    Name = pCmdArgAttr?.Name ?? arg.Name,
                    Description = pCmdArgAttr?.Description ?? arg.Name,
                    Type = arg.ParameterType
                };

                pCmdArgs.Add(pCmdArg);
            }

            return pCmdArgs;
        }

        public async Task<T> InvokeCmdAsync<T>(string groupName, string cmdName, Dictionary<string, object> args)
        {
            using (_pluginLoadContext.EnterContextualReflection())
            {
                JToken resultJson = null;
                var cmdGroup = _pluginCmdGroups.Find(t => t.Name == groupName);
                if (cmdGroup == null) { throw new Exception(); }

                var cmd = cmdGroup.Commands.Find(t => t.Name == cmdName);
                if (cmd == null) { throw new Exception(); }

                Dictionary<string, JToken> jsonArgs = new Dictionary<string, JToken>();
                if (args != null)
                {
                    foreach (var kv in args)
                    {
                        jsonArgs[kv.Key] = kv.Value == null ? null : JToken.FromObject(kv.Value, _jsonSerializer);
                    }
                }

                resultJson = await cmd.InvokeAsync(cmdGroup.Target, jsonArgs, _jsonSerializer);
                if (resultJson == null) { return default(T); }
                return resultJson.ToObject<T>(_jsonSerializer);
            }
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

            _jsonSettings = null;
            _jsonSerializer = null;

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