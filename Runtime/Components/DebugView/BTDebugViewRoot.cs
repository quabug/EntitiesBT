#if UNITY_EDITOR

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
        private static readonly ILookup<int, Type> _NODE_COMPONENTS_MAP;
        private static readonly Dictionary<int, Type> _BEHAVIOR_NODE_ID_TYPE_MAP = new Dictionary<int, Type>();

        static BTDebugViewRoot()
        {
            List<(int nodeTypeId, Type debugViewType)> debugViews = new List<(int nodeTypeId, Type debugViewType)>();
            
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

            _NODE_COMPONENTS_MAP = debugViews.ToLookup(t => t.nodeTypeId, t => t.debugViewType);

            void RegisterDebugView(Type nodeType, Type debugViewType)
            {
                var nodeTypeId = nodeType.GetCustomAttribute<BehaviorNodeAttribute>().Id;
                debugViews.Add((nodeTypeId, debugViewType));
            }
        }
        
        private INodeBlob _blob;
        private IBlackboard _blackboard;
        private List<GameObject> _views;

        public void Init(INodeBlob blob, IBlackboard blackboard)
        {
            _blob = blob;
            _blackboard = blackboard;
            
            _views = new List<GameObject>(_blob.Count);
            for (var i = 0; i < _blob.Count; i++)
            {
                var nodeTypeId = _blob.GetTypeId(i);
                var debugViewTypeList = _NODE_COMPONENTS_MAP[nodeTypeId];
                
                var nodeGameObject = new GameObject();
                _views.Add(nodeGameObject);
                nodeGameObject.name = $"{_BEHAVIOR_NODE_ID_TYPE_MAP[nodeTypeId].Name}";
                if (debugViewTypeList.Any())
                {
                    foreach (var viewType in debugViewTypeList)
                        nodeGameObject.AddComponent(viewType);
                }
                else
                {
                    nodeGameObject.name = $"?? {nodeGameObject.name}";
                    nodeGameObject.AddComponent<BTDebugView>();
                }
                
                var parentIndex = _blob.ParentIndex(i);
                var parent = parentIndex >= 0 ? _views[parentIndex].transform : transform;
                nodeGameObject.transform.SetParent(parent);
            }
            
            for (var i = 0; i < _views.Count; i++)
            {
                foreach (var view in _views[i].GetComponents<IBTDebugView>())
                    view.InitView(_blob, _blackboard, i);
            }
        }

        private void Update()
        {
            if (_views == null) return;
            for (var i = 0; i < _views.Count; i++)
            {
                foreach (var view in _views[i].GetComponents<IBTDebugView>())
                    view.TickView(_blob, _blackboard, i);
            }
        }
    }
}

#endif
