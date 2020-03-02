using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EntitiesBT.Core;

namespace EntitiesBT.DebugView
{
    public static class DebugComponentLookUp
    {
        public static readonly ILookup<int, Type> NODE_COMPONENTS_MAP;
        public static readonly Dictionary<int, Type> BEHAVIOR_NODE_ID_TYPE_MAP = new Dictionary<int, Type>();

        static DebugComponentLookUp()
        {
            var debugViews = new List<(int nodeTypeId, Type debugViewType)>();
            
            foreach (var type in AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes()))
            {
                var behaviorNodeAttribute = type.GetCustomAttribute<BehaviorNodeAttribute>();
                if (behaviorNodeAttribute != null)
                {
                    BEHAVIOR_NODE_ID_TYPE_MAP[behaviorNodeAttribute.Id] = type;
                    continue;
                }

                if (!type.IsSubclassOf(typeof(BTDebugView))) continue;
                
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

            NODE_COMPONENTS_MAP = debugViews.ToLookup(t => t.nodeTypeId, t => t.debugViewType);

            void RegisterDebugView(Type nodeType, Type debugViewType)
            {
                var nodeTypeId = nodeType.GetCustomAttribute<BehaviorNodeAttribute>().Id;
                debugViews.Add((nodeTypeId, debugViewType));
            }
        }
        
    }
}
