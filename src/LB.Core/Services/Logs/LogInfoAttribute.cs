using System;

namespace LB.Core.Services.Logs
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class LogInfoAttribute : Attribute
    {
        public string Tag { get; set; }
    }
}