using System;
using JetBrains.Annotations;

namespace EntitiesBT.Attributes.Editor
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    [BaseTypeRequired(typeof(IMultiPropertyDrawer))]
    public class CustomMultiPropertyDrawerAttribute : Attribute
    {
        internal readonly Type Type;
        internal readonly bool UseForChildren;

        public CustomMultiPropertyDrawerAttribute(Type type, bool useForChildren = false)
        {
            Type = type;
            UseForChildren = useForChildren;
        }
    }
}