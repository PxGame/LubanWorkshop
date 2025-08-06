using Luban.Core.Containers;
using Luban.Plugin;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Core.Services.Plugins
{
    internal class PluginCommandRet
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Type Type { get; set; }

        public override string ToString()
        {
            return $"{Name}({Description})";
        }
    }

    internal class PluginCommandArg
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Type Type { get; set; }

        public override string ToString()
        {
            return $"{Name}({Description})";
        }
    }

    internal class PluginCommand
    {
        public MethodInfo Method { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public List<PluginCommandArg> Args { get; set; }
        public PluginCommandRet Ret { get; set; }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine($"Command : {Name}");
            builder.AppendLine($"Description : {Description}");
            if (Args != null && Args.Count > 0)
            {
                builder.AppendLine("Arguments : ");
                for (int i = 0; i < Args.Count; i++)
                {
                    var arg = Args[i];
                    builder.AppendLine($"  {i} - {arg}");
                }
            }
            if (Ret != null)
            {
                builder.AppendLine($"Return : ");
                builder.AppendLine($"  {Ret}");
            }
            return builder.ToString();
        }

        internal async Task<JToken> InvokeAsync(object target, Dictionary<string, JToken> name2value)
        {
            if (Method == null) { throw new InvalidOperationException("Method is not set."); }

            var argObjs = new object[Args.Count];

            for (int i = 0; i < Args.Count; i++)
            {
                var arg = Args[i];
                if (name2value.TryGetValue(arg.Name, out var value))
                {
                    argObjs[i] = value == null ? null : value.ToObject(arg.Type);
                }
                else
                {
                    throw new ArgumentException($"Argument '{arg.Name}' is required but not provided.");
                }
            }

            var result = Method.Invoke(target, argObjs);
            if (result == null) { return null; }

            if (typeof(Task).IsAssignableFrom(Ret.Type))
            {
                var task = result as Task;
                await task;

                if (Ret.Type.IsGenericType && Ret.Type.GetGenericTypeDefinition() == typeof(Task<>))
                {
                    var taskResult = Ret.Type.GetProperty("Result").GetValue(task);
                    if (taskResult == null) { return null; }
                    return JToken.FromObject(taskResult);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return JToken.FromObject(result);
            }
        }
    }

    internal class PluginCommandGroup
    {
        public object Target { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<PluginCommand> Commands { get; } = new List<PluginCommand>();

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine($"Command Group : {Name}");
            builder.AppendLine($"Description : {Description}");
            if (Commands != null && Commands.Count > 0)
            {
                builder.AppendLine("Commands :");
                foreach (var cmd in Commands)
                {
                    using var reader = new StringReader(cmd.ToString());
                    do
                    {
                        var line = reader.ReadLine();
                        if (line == null) { break; }
                        builder.AppendLine("  " + line);
                    } while (true);
                    builder.AppendLine();
                }
            }
            return builder.ToString();
        }
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
            var methods = pCmdType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
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
            foreach (var arg in args)
            {
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
            var cmdGroup = _pluginCmdGroups.Find(t => t.Name == groupName);
            if (cmdGroup == null) { throw new Exception(); }

            var cmd = cmdGroup.Commands.Find(t => t.Name == cmdName);
            if (cmdGroup == null) { throw new Exception(); }

            Dictionary<string, JToken> jsonArgs = new Dictionary<string, JToken>();

            if (args != null)
            {
                foreach (var kv in args)
                {
                    jsonArgs[kv.Key] = kv.Value == null ? null : JToken.FromObject(kv.Value);
                }
            }

            var resultJson = await cmd.InvokeAsync(cmdGroup.Target, jsonArgs);
            if (resultJson == null) { return default(T); }

            return resultJson.ToObject<T>();
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