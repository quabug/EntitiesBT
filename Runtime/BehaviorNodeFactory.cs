using System;
using System.Collections.Generic;
using EntitiesBT.Core;

namespace EntitiesBT
{
    public class BehaviorNodeFactory : IBehaviorNodeFactory
    {
        private readonly List<Func<IBehaviorNode>> _creators = new List<Func<IBehaviorNode>>();
        private readonly Dictionary<Type, int> _nodeTypes = new Dictionary<Type, int>();

        public void Register<T>(Func<IBehaviorNode> creator) where T : IBehaviorNode
        {
            var type = typeof(T);
            if (!_nodeTypes.ContainsKey(type))
            {
                _nodeTypes[type] = _creators.Count;
                _creators.Add(creator);
            }
        }
        
        public void Register<T>() where T : IBehaviorNode, new()
        {
            Register<T>(() => new T());
        }

        public int GetTypeId<T>() where T : IBehaviorNode
        {
            return GetTypeId(typeof(T));
        }
        
        public int GetTypeId(Type type)
        {
            return _nodeTypes[type];
        }
        
        public IBehaviorNode Create(int nodeType)
        {
            return _creators[nodeType]();
        }
    }
}
