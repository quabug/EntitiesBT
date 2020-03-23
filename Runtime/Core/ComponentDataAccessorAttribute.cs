using System;
using Unity.Entities;

namespace EntitiesBT.Core
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter)]
    public class ComponentDataReadOnlyAccessorAttribute : Attribute {}
    
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter)]
    public class ComponentDataReadWriteAccessorAttribute : Attribute {}

    [AttributeUsage(AttributeTargets.Method)]
    public class ComponentDataAccessorsAttribute : Attribute
    {
        public ComponentType[] Accessors;
        public ComponentDataAccessorsAttribute(params ComponentType[] accessors)
        {
            Accessors = accessors;
        }
    }
}
