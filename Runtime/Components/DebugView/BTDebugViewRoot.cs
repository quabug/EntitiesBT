using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EntitiesBT.Core;
using UnityEngine;

namespace EntitiesBT.Components.DebugView
{
    public class BTDebugViewRoot : MonoBehaviour
    {
        private static readonly Dictionary<int, Type> _NODE_COMPONENTS_MAP = new Dictionary<int, Type>();
        private static readonly Dictionary<int, string> _NODE_NAME_MAP = new Dictionary<int, string>();

        static BTDebugViewRoot()
        {
            // AppDomain.CurrentDomain.GetAssemblies()
            //     .SelectMany(assembly => assembly.GetTypes())
            //     .Where(type => type.IsSubclassOf(typeof(BTNode)))
            foreach (var type in AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes()))
            {
                var attribute = type.GetCustomAttribute<BehaviorNodeAttribute>();
                if (attribute == null) continue;
                _NODE_NAME_MAP[attribute.Id] = type.Name;
            }
        }
        
        private INodeBlob _blob;
        private IBlackboard _blackboard;

        public void Init(INodeBlob blob, IBlackboard blackboard)
        {
            _blob = blob;
            _blackboard = blackboard;
            
            var nodes = new List<GameObject>(_blob.Count);
            for (var i = 0; i < _blob.Count; i++)
            {
                var nodeType = _blob.GetTypeId(i);
                _NODE_COMPONENTS_MAP.TryGetValue(nodeType, out var debugViewType);
                var debugName = debugViewType == null ? $"[CannotFound] {_NODE_NAME_MAP[nodeType]}" : debugViewType.Name;
                var nodeGameObject = new GameObject();
                nodes.Add(nodeGameObject);
                nodeGameObject.name = debugName;
                var parentIndex = _blob.ParentIndex(i);
                var parent = parentIndex >= 0 ? nodes[parentIndex].transform : transform;
                nodeGameObject.transform.SetParent(parent);
            }
        }
    }
}
