using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Unity.Entities;

namespace EntitiesBT.Core
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public abstract class ComponentAccessorAttribute : Attribute
    {
        protected readonly Type[] _types;
        public abstract IEnumerable<ComponentType> Types { get; }
        public ComponentAccessorAttribute([NotNull, ItemNotNull] Type[] types) => _types = types;
    }
    
    public class ReadOnlyAttribute : ComponentAccessorAttribute
    {
        public ReadOnlyAttribute([NotNull, ItemNotNull] params Type[] types) : base(types) {}
        public override IEnumerable<ComponentType> Types =>
            _types.Select(t => new ComponentType(t, ComponentType.AccessMode.ReadOnly));
    }
    
    public class ReadWriteAttribute : ComponentAccessorAttribute
    {
        public ReadWriteAttribute([NotNull, ItemNotNull] params Type[] types) : base(types) {}
        public override IEnumerable<ComponentType> Types =>
            _types.Select(t => new ComponentType(t, ComponentType.AccessMode.ReadWrite));
    }
    
    public class OptionalAttribute : ComponentAccessorAttribute
    {
        public OptionalAttribute() : base(Array.Empty<Type>()) {}
        public override IEnumerable<ComponentType> Types => Enumerable.Empty<ComponentType>();
    }

    public interface IRuntimeComponentAccessor
    {
        IEnumerable<ComponentType> ComponentAccessList { [Pure] get; }
    }
}
