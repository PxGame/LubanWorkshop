using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Core.Services.Plugins
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class PluginCommandAttribute : Attribute
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}