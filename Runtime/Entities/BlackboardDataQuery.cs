using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;

namespace EntitiesBT.Entities
{
    public struct BlackboardDataQuery : ISharedComponentData, IEquatable<BlackboardDataQuery>
    {
        public ISet<ComponentType> Value;
        // TODO: https://forum.unity.com/threads/entityquery-cannot-be-used-in-isharedcomponentdata.850255/
        // public EntityQuery EntityQuery { get; set; }

        public bool Equals(BlackboardDataQuery other)
        {
            return Equals(Value, other.Value);
        }

        bool Equals(ISet<ComponentType> lhs, ISet<ComponentType> rhs)
        {
            if (lhs == null && rhs == null) return true;
            if (lhs == null || rhs == null) return false;
            if (lhs.Count != rhs.Count) return false;
            return !lhs.Except(rhs).Any();
        }

        public override bool Equals(object obj)
        {
            return obj is BlackboardDataQuery other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Value?.Aggregate(0, (hash, type) => hash ^ (type.GetHashCode() * 397)) ?? 0;
        }
    }
}
