using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Core.Services.Plugins
{
    public class PluginCommandBaseAttribute : Attribute
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class PCmdGroupAttribute : PluginCommandBaseAttribute
    {
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class PCmdAttribute : PluginCommandBaseAttribute
    {
    }

    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class PCmdArgAttribute : PluginCommandBaseAttribute
    {
    }

    [AttributeUsage(AttributeTargets.ReturnValue, AllowMultiple = false, Inherited = true)]
    public class PCmdRetAttribute : PluginCommandBaseAttribute
    {
    }

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

            if (typeof(Task).IsAssignableFrom(Ret.Type))
            {
                var task = result as Task;
                if (task == null) { throw new InvalidOperationException("Method does not return a Task or Task<T>."); }
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
                if (result == null) { return null; }
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
}