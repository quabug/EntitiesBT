using System;
using System.Reflection;
using EntitiesBT.Core;
using GraphExt;
using Nuwa.Blob;

namespace EntitiesBT
{
    [Serializable]
    public class BehaviorNode : GraphNode, GraphExt.ITreeNode<GraphRuntime<GraphNode>>
    {
        public override Type ComponentType => typeof(BehaviorNodeComponent);

        public DynamicBlobDataBuilder Blob;

        public Type BehaviorNodeDataType => Type.GetType(Blob.BlobDataType);
        public BehaviorNodeAttribute BehaviorNodeAttribute => BehaviorNodeDataType.GetCustomAttribute<BehaviorNodeAttribute>();
        public BehaviorNodeType BehaviorNodeType => BehaviorNodeAttribute.Type;

        public string InputPortName => "input-port";
        public string OutputPortName => "output-port";
    }
}