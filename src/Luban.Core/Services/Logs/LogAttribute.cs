using System;
using System.Collections.Generic;
using System.Reflection;

namespace Luban.Core.Services.Logs
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class LogAttribute : Attribute
    {
        public string Tag { get; set; }
        public string CustomPropertyMethodName { get; set; }

        public Dictionary<string, object> GetCustomProperty(object target)
        {
            if (string.IsNullOrEmpty(CustomPropertyMethodName)) { return new Dictionary<string, object>(); }
            var method = target.GetType().GetMethod(CustomPropertyMethodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (method == null) { throw new InvalidOperationException($"Method '{CustomPropertyMethodName}' not found in type '{target.GetType().FullName}'."); }
            return method.Invoke(target, null) as Dictionary<string, object>;
        }
    }
}