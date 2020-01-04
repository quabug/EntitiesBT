using System;
using System.Collections.Generic;
using System.Linq;
using Entities;
using Unity.Entities;

namespace EntitiesBT.Entities
{
    public class MainThreadOnlyBlackboard : IComponentData
    {
        public EntityMainThreadBlackboard Value;
    }
    
    public struct JobBlackboard : IComponentData
    {
        public EntityJobChunkBlackboard Value;
    }
    
    public struct JobBehaviorTreeTag : IComponentData {}

    public struct BlackboardDataQuery : ISharedComponentData, IEquatable<BlackboardDataQuery>
    {
        private HashSet<Type> _readOnlyTypes;
        private HashSet<Type> _readWriteTypes;

        public IEnumerable<Type> ReadOnlyTypes => _readOnlyTypes ?? Enumerable.Empty<Type>();
        public IEnumerable<Type> ReadWriteTypes => _readWriteTypes ?? Enumerable.Empty<Type>();
        public int Count => (_readOnlyTypes?.Count ?? 0) + (_readWriteTypes?.Count ?? 0);
        
        public void AddReadOnly(Type type)
        {
            if (_readOnlyTypes == null) _readOnlyTypes = new HashSet<Type>();
            if (!_readOnlyTypes.Contains(type) && (_readWriteTypes == null || !_readWriteTypes.Contains(type)))
                _readOnlyTypes.Add(type);
        }

        public void AddReadWrite(Type type)
        {
            if (_readWriteTypes == null) _readWriteTypes = new HashSet<Type>();
            if (_readOnlyTypes != null && _readOnlyTypes.Contains(type)) _readOnlyTypes.Remove(type);
            if (!_readWriteTypes.Contains(type)) _readWriteTypes.Add(type);
        }

        public bool Equals(BlackboardDataQuery other)
        {
            return Equals(_readOnlyTypes, other._readOnlyTypes) && Equals(_readWriteTypes, other._readWriteTypes);
        }

        private bool Equals(ISet<Type> lhs, ISet<Type> rhs)
        {
            if (lhs == rhs) return true;
            if (lhs == null || rhs == null) return false;
            if (lhs.Count != rhs.Count) return false;
            return !lhs.Except(rhs).Any();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != GetType()) return false;
            return Equals((BlackboardDataQuery) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var readOnlyHash = ReadOnlyTypes.Aggregate(0, (hash, type) => hash ^ (type.GetHashCode() * 397));
                var readWriteHash = ReadWriteTypes.Aggregate(0, (hash, type) => hash ^ (type.GetHashCode() * 398));
                return readOnlyHash ^ readWriteHash;
            }
        }
    }
}
