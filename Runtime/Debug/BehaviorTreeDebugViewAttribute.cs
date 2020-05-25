using System;
using JetBrains.Annotations;

namespace EntitiesBT.DebugView
{
    [AttributeUsage(AttributeTargets.Class)]
    [BaseTypeRequired(typeof(BTDebugView))]
    public class BehaviorTreeDebugViewAttribute : Attribute
    {
        public readonly Type[] NodeTypes;

        public BehaviorTreeDebugViewAttribute(params Type[] nodeTypes)
        {
            NodeTypes = nodeTypes;
        }
    }
}
