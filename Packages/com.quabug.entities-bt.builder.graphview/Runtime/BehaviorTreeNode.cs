using System;
using System.Reflection;
using EntitiesBT.Core;
using GraphExt;
using Nuwa.Blob;

namespace EntitiesBT
{
    [Serializable]
    public sealed class BehaviorTreeNode : IGraphNode, GraphExt.ITreeNode<GraphRuntime<IGraphNode>>
    {
        public DynamicBlobDataBuilder Blob;

        public Type BehaviorNodeDataType => Type.GetType(Blob.BlobDataType);
        public BehaviorNodeAttribute BehaviorNodeAttribute => BehaviorNodeDataType.GetCustomAttribute<BehaviorNodeAttribute>();
        public BehaviorNodeType BehaviorNodeType => BehaviorNodeAttribute.Type;

        public string InputPortName => "input-port";
        public string OutputPortName => "output-port";

        public bool IsPortCompatible(GraphRuntime<IGraphNode> graph, in PortId input, in PortId output) => true;
        public void OnConnected(GraphRuntime<IGraphNode> graph, in PortId input, in PortId output) {}
        public void OnDisconnected(GraphRuntime<IGraphNode> graph, in PortId input, in PortId output) {}
    }
}