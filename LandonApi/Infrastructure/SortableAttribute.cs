using System;

namespace LandonApi.Infrastructure
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class SortableAttribute : Attribute
    {
        public string EntityProperty { get; set; }
        public bool Default { get; set; }
    }
}
