using System;
using System.Collections.Generic;
using EntitiesBT.Core;

namespace EntitiesBT
{
    public class BehaviorNodeFactory : IBehaviorNodeFactory
    {
        private List<Func<IBehaviorNode>> _creators = new List<Func<IBehaviorNode>>();
        private Dictionary<Type, int> _nodeTypes = new Dictionary<Type, int>();
        
        public BehaviorNodeFactory()
        {
        }

        void Register<T>(Func<IBehaviorNode> creator) where T : IBehaviorNode
        {
            var type = typeof(T);
            if (!_nodeTypes.ContainsKey(type))
            {
                _nodeTypes[type] = _creators.Count;
                _creators.Add(creator);
            }
        }
        
        public IBehaviorNode Create(int nodeType)
        {
            return _creators[nodeType]();
        }
    }
}
