using System;

namespace LB.Core.Containers
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class InjectAttribute : Attribute
    {
        public Type fromType { get; set; }
        public bool IgnoreFailed { get; set; }
    }
}