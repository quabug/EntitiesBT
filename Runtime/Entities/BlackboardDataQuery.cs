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
            var lhsCount = lhs?.Count ?? 0;
            var rhsCount = rhs?.Count ?? 0;
            if (lhsCount == 0 && rhsCount == 0) return true;
            if (lhsCount != rhsCount) return false;
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
