using System;

namespace Luban.Core.Services.Logs
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class LogAttribute : Attribute
    {
        public string Tag { get; set; }
    }
}