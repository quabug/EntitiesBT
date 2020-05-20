using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;

namespace EntitiesBT.Entities
{
    public struct BlackboardDataQuery : ISharedComponentData, IEquatable<BlackboardDataQuery>
    {
        public ComponentTypeSet Set { get; }
        public EntityQuery Query { get; }

        public BlackboardDataQuery(ComponentTypeSet set, Func<IEnumerable<ComponentType>, EntityQuery> createEntityQuery)
        {
            Set = set;
            Query = createEntityQuery(Set);
        }

        public bool Equals(BlackboardDataQuery other)
        {
            return Equals(Set, other.Set);
        }

        public override bool Equals(object obj)
        {
            return obj is BlackboardDataQuery other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Set?.GetHashCode() ?? 0;
        }
    }

    public class ComponentTypeSet : IEnumerable<ComponentType>, IEquatable<ComponentTypeSet>
    {
        class ComponentTypeComparer : IEqualityComparer<ComponentType>
        {
            public bool Equals(ComponentType x, ComponentType y)
            {
                return x.TypeIndex == y.TypeIndex && x.AccessModeType == y.AccessModeType;
            }

            public int GetHashCode(ComponentType obj)
            {
                return HashCode(obj);
            }
        }

        private static int HashCode(ComponentType obj)
        {
            return obj.GetHashCode() ^ ((int) obj.AccessModeType * 39);
        }

        private readonly HashSet<ComponentType> _types;
        private readonly int _hashCode;
        
        public IEnumerator<ComponentType> GetEnumerator()
        {
            return _types.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public ComponentTypeSet(IEnumerable<ComponentType> types)
        {
            _types = new HashSet<ComponentType>(new ComponentTypeComparer());
            foreach (var type in types) Add(type);
            _hashCode = _types.Aggregate(0, (hash, type) => hash ^ HashCode(type));
        }
        
        private void Add(ComponentType type)
        {
            if (_types.Contains(type)) return;
            
            switch (type.AccessModeType)
            {
            case ComponentType.AccessMode.ReadWrite:
                _types.Add(type);
                var @readonly = ComponentType.ReadOnly(type.TypeIndex);
                if (_types.Contains(@readonly)) _types.Remove(@readonly);
                break;
            case ComponentType.AccessMode.ReadOnly:
                var readwrite = ComponentType.FromTypeIndex(type.TypeIndex);
                if (!_types.Contains(readwrite)) _types.Add(type);
                break;
            default:
                throw new ArgumentOutOfRangeException();
            }
        }
        
        public bool Equals(ComponentTypeSet other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (_types.Count != other._types.Count) return false;
            return !_types.Except(other._types, new ComponentTypeComparer()).Any();
        }
        
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is ComponentTypeSet other && Equals(other);
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override string ToString()
        {
            return string.Join(",", _types.ToArray());
        }
    }
}
