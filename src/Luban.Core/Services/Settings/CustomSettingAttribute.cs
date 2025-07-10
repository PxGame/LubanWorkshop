using System;
using System.IO;

namespace Luban.Core.Services.Settings
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class CustomSettingAttribute : Attribute
    {
        public string Name { get; private set; }

        public string SubPath { get; set; }

        public bool IsAppFolder { get; set; }

        public string RelativePath => string.IsNullOrEmpty(SubPath) ? Name : Utils.PathCombine(SubPath, Name);

        public CustomSettingAttribute(string name)
        {
            Name = name;
        }
    }
}