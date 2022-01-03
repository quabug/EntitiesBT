using System;
using System.Reflection;
using EntitiesBT.Core;
using GraphExt;
using Nuwa.Blob;

namespace EntitiesBT
{
    [Serializable]
    public sealed class BehaviorTreeNode : GraphExt.ITreeNode<GraphRuntime<BehaviorTreeNode>>
    {
        public DynamicBlobDataBuilder Blob;

        public Type BehaviorNodeDataType => Type.GetType(Blob.BlobDataType);
        public BehaviorNodeAttribute BehaviorNodeAttribute => BehaviorNodeDataType.GetCustomAttribute<BehaviorNodeAttribute>();
        public BehaviorNodeType BehaviorNodeType => BehaviorNodeAttribute.Type;

        public string InputPortName => "input-port";
        public string OutputPortName => "output-port";

        public bool IsPortCompatible(GraphRuntime<BehaviorTreeNode> graph, in PortId input, in PortId output) => true;
        public void OnConnected(GraphRuntime<BehaviorTreeNode> graph, in PortId input, in PortId output) {}
        public void OnDisconnected(GraphRuntime<BehaviorTreeNode> graph, in PortId input, in PortId output) {}
    }
}