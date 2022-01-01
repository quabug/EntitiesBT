using System;
using EntitiesBT.Components;
using GraphExt;

namespace EntitiesBT
{
    public interface IBehaviorTreeNode : GraphExt.ITreeNode<GraphRuntime<IBehaviorTreeNode>> {}

    [Serializable]
    public class BehaviorTreeNode : IBehaviorTreeNode
    {
        [NodeProperty(CustomFactory = typeof(Editor.BehaviorNodeAssetProperty.Factory))] public NodeAsset Data;

        [NodePort(Orientation = PortOrientation.Vertical, SerializeId = "in")] protected static BehaviorTreeNode Input;
        [NodePort(Orientation = PortOrientation.Vertical, SerializeId = "out")] protected static BehaviorTreeNode[] Output;

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