using System;
using System.Collections.Generic;
using System.Linq;
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
}