using System;

namespace EntitiesBT.DebugView
{
    [AttributeUsage(AttributeTargets.Class)]
    public class BehaviorTreeDebugViewAttribute : Attribute
    {
        public readonly Type[] NodeTypes;

        public BehaviorTreeDebugViewAttribute(params Type[] nodeTypes)
        {
            NodeTypes = nodeTypes;
        }
    }
}
