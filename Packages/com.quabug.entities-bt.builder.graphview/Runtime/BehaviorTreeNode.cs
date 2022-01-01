using System;
using EntitiesBT.Editor;
using GraphExt;
using Nuwa.Blob;

namespace EntitiesBT
{
    public interface IBehaviorTreeNode : GraphExt.ITreeNode<GraphRuntime<IBehaviorTreeNode>> {}

    [Serializable]
    public class BehaviorTreeNode : IBehaviorTreeNode
    {
        [NodePort(Orientation = PortOrientation.Vertical, SerializeId = "in")] protected static IBehaviorTreeNode Input;
        [NodePort(Orientation = PortOrientation.Vertical, SerializeId = "out")] protected static IBehaviorTreeNode[] Output;
        [NodeProperty(CustomFactory = typeof(BehaviorBlobDataProperty.Factory))] public DynamicBlobDataBuilder Blob;
        [NodeProperty(CustomFactory = typeof(BehaviorNodeTypeClassProperty.Factory))] public Type BehaviorNodeType => Type.GetType(Blob.BlobDataType);

        public string InputPortName => nameof(Input);
        public string OutputPortName => nameof(Output);

        public bool IsPortCompatible(GraphRuntime<IBehaviorTreeNode> graph, in PortId input, in PortId output)
        {
            return true;
        }

        public void OnConnected(GraphRuntime<IBehaviorTreeNode> graph, in PortId input, in PortId output)
        {
        }

        public void OnDisconnected(GraphRuntime<IBehaviorTreeNode> graph, in PortId input, in PortId output)
        {
        }
    }
}