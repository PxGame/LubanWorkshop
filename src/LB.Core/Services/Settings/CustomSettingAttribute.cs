using System;
using System.IO;

namespace LB.Core.Services.Settings
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class CustomSettingAttribute : Attribute
    {
        public string Name { get; private set; }

        public string SubPath { get; set; }

        public bool IsAppFolder { get; set; }

        public string GetFirstSubPathMethodName { get; set; }

        public string RelativePath => string.IsNullOrEmpty(SubPath) ? Name : Path.Combine(SubPath, Name).StandardizedPath();

        public CustomSettingAttribute(string name)
        {
            Name = name;
        }
    }
}