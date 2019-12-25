using System;
using EntitiesBT.Core;
using Unity.Entities;

namespace EntitiesBT
{
    public readonly struct BehaviorNodeFactoryComponent : ISharedComponentData, IEquatable<BehaviorNodeFactoryComponent>
    {
        public readonly IBehaviorNodeFactory Value;
        public BehaviorNodeFactoryComponent(IBehaviorNodeFactory factory) => Value = factory;

        public bool Equals(BehaviorNodeFactoryComponent other)
        {
            return Equals(Value, other.Value);
        }

        public override bool Equals(object obj)
        {
            return obj is BehaviorNodeFactoryComponent other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (Value != null ? Value.GetHashCode() : 0);
        }
    }
}
