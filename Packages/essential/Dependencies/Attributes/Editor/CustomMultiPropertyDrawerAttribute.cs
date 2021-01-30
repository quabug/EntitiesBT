using System;

namespace EntitiesBT.Attributes.Editor
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
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