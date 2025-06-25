using Luban.Core.Containers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Core.Services.Settings
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class CustomSettingRootAttribute : Attribute
    {
        public string SubPath { get; set; }
        public string GetNextSubPathMethodName { get; set; }

        internal string GetNextSubPath(object target)
        {
            var result = (SubPath ?? string.Empty).StandardizedPath(); ;
            if (!string.IsNullOrEmpty(GetNextSubPathMethodName))
            {
                var nextSubPathMethod = target.GetType().GetMethod(GetNextSubPathMethodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (nextSubPathMethod == null) { throw new ContainerException($"未找到方法: {GetNextSubPathMethodName} 在 {target.GetType().FullName}"); }
                var nextSubPath = nextSubPathMethod.Invoke(target, null) as string ?? string.Empty;
                result = Path.Combine(result, nextSubPath).StandardizedPath();
            }

            return result;
        }
    }
}