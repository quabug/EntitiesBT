#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using EntitiesBT.Core;
using UnityEngine;

namespace EntitiesBT.Components.DebugView
{
    public class BTDebugViewRoot : MonoBehaviour
    {
        private static readonly Dictionary<int, Type> _NODE_COMPONENTS_MAP = new Dictionary<int, Type>();
        private static readonly Dictionary<int, Type> _BEHAVIOR_NODE_ID_TYPE_MAP = new Dictionary<int, Type>();

        static BTDebugViewRoot()
        {
            foreach (var type in AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes()))
            {
                var behaviorNodeAttribute = type.GetCustomAttribute<BehaviorNodeAttribute>();
                if (behaviorNodeAttribute != null)
                {
                    _BEHAVIOR_NODE_ID_TYPE_MAP[behaviorNodeAttribute.Id] = type;
                    continue;
                }

                if (!typeof(IBTDebugView).IsAssignableFrom(type)) continue;
                
                var debugViewAttribute = type.GetCustomAttribute<BehaviorTreeDebugViewAttribute>();
                if (debugViewAttribute != null)
                {
                    foreach (var nodeType in debugViewAttribute.NodeTypes)
                        RegisterDebugView(nodeType, type);
                    continue;
                }
                
                var baseType = type.BaseType;
                while (baseType != null)
                {
                    if (baseType.IsGenericType)
                    {
                        var attribute = baseType.GetCustomAttribute<BehaviorTreeDebugViewGenericAttribute>();
                        if (attribute != null)
                        {
                            RegisterDebugView(baseType.GetGenericArguments()[attribute.NodeTypeIndex], type);
                            break;
                        }
                    }
                    baseType = baseType.BaseType;
                }
            }

            void RegisterDebugView(Type nodeType, Type debugViewType)
            {
                var nodeTypeId = nodeType.GetCustomAttribute<BehaviorNodeAttribute>().Id;
                if (_NODE_COMPONENTS_MAP.TryGetValue(nodeTypeId, out var existViewType))
                    if (existViewType != debugViewType)
                        throw new DuplicateNameException($"{nodeType.Name} already has debug view {existViewType.Name}, cannot register {debugViewType.Name}");
                _NODE_COMPONENTS_MAP[nodeTypeId] = debugViewType;
            }
        }
        
        private INodeBlob _blob;
        private IBlackboard _blackboard;
        private List<IBTDebugView> _views;

        public void Init(INodeBlob blob, IBlackboard blackboard)
        {
            _blob = blob;
            _blackboard = blackboard;
            
            _views = new List<IBTDebugView>(_blob.Count);
            for (var i = 0; i < _blob.Count; i++)
            {
                var nodeTypeId = _blob.GetTypeId(i);
                _NODE_COMPONENTS_MAP.TryGetValue(nodeTypeId, out var debugViewType);
                var debugName = debugViewType == null 
                    ? $"? {_BEHAVIOR_NODE_ID_TYPE_MAP[nodeTypeId].Name}"
                    : debugViewType.Name
                ;
                
                var nodeGameObject = new GameObject();
                nodeGameObject.name = debugName;
                if (debugViewType == null)
                {
                    nodeGameObject.name = $"? {_BEHAVIOR_NODE_ID_TYPE_MAP[nodeTypeId].Name}";
                    _views.Add(nodeGameObject.AddComponent<BTDebugView>());
                }
                else
                {
                    _views.Add((IBTDebugView)nodeGameObject.AddComponent(debugViewType));
                }
                
                var parentIndex = _blob.ParentIndex(i);
                var parent = parentIndex >= 0 ? ((Component)_views[parentIndex]).transform : transform;
                nodeGameObject.transform.SetParent(parent);
            }
        }

        private void Update()
        {
            if (_views == null) return;
            for (var i = 0; i < _views.Count; i++) _views[i].Tick(_blob, _blackboard, i);
        }
    }
}

#endif
