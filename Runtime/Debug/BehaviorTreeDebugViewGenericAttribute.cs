using System;

namespace EntitiesBT.DebugView
{
    [AttributeUsage(AttributeTargets.Class)]
    public class BehaviorTreeDebugViewGenericAttribute : Attribute
    {
        public readonly int NodeTypeIndex;
        public BehaviorTreeDebugViewGenericAttribute(int nodeTypeIndex = 0)
        {
            NodeTypeIndex = nodeTypeIndex;
        }
    }
}
