using System;
using System.Collections.Generic;

namespace EntitiesBT.Core
{
    public class TypeRegistries<T>
    {
        private readonly List<T> _entries = new List<T>();
        private readonly Dictionary<Type, int> _indices = new Dictionary<Type, int>();
        
        public void Register(T entry)
        {
            var type = entry.GetType();
            if (!_indices.ContainsKey(type))
            {
                _indices[type] = _entries.Count;
                _entries.Add(entry);
            }
        }

        public int GetIndex<U>() where U : T
        {
            return _indices[typeof(U)];
        }
        
        public T this[int index] => _entries[index];
    }
}
