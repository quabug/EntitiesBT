using System;
using JetBrains.Annotations;

namespace Nuwa
{
    [AttributeUsage(AttributeTargets.Field)]
    [BaseTypeRequired(typeof(string))]
    public class AsGameObjectNameAttribute : MultiPropertyAttribute
    {
        public string Default;
        public string NamePatter;
    }
}