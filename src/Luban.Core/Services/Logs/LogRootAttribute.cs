using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Core.Services.Logs
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class LogRootAttribute : Attribute
    {
        public string GetLogPropertyDictMethodName { get; set; }

        public Dictionary<string, object> GetLogPropertyDict(object target)
        {
            if (string.IsNullOrEmpty(GetLogPropertyDictMethodName)) { return new Dictionary<string, object>(); }
            var method = target.GetType().GetMethod(GetLogPropertyDictMethodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (method == null) { throw new InvalidOperationException($"Method '{GetLogPropertyDictMethodName}' not found in type '{target.GetType().FullName}'."); }
            return method.Invoke(target, null) as Dictionary<string, object>;
        }
    }
}