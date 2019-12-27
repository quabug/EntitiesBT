using System;
using System.Collections.Generic;

namespace EntitiesBT.Core
{
    public class Registries<T>
    {
        private readonly List<T> _registries = new List<T>();
        private readonly Dictionary<Type, int> _indices = new Dictionary<Type, int>();
        
        public void Register(T node)
        {
            var type = node.GetType();
            if (!_indices.ContainsKey(type))
            {
                _indices[type] = _registries.Count;
                _registries.Add(node);
            }
        }

        public int GetIndex<U>() where U : T
        {
            return _indices[typeof(U)];
        }
        
        public T this[int index] => _registries[index];
    }
}
