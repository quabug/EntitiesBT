using System;
using JetBrains.Annotations;

namespace EntitiesBT.DebugView
{
    [AttributeUsage(AttributeTargets.Class)]
    [BaseTypeRequired(typeof(BTDebugView))]
    public class BehaviorTreeDebugViewGenericAttribute : Attribute
    {
        public readonly int NodeTypeIndex;
        public BehaviorTreeDebugViewGenericAttribute(int nodeTypeIndex = 0)
        {
            NodeTypeIndex = nodeTypeIndex;
        }
    }
}
