using System;
using GraphExt;

namespace EntitiesBT
{
    public abstract class GraphNode : INode<GraphRuntime<GraphNode>>
    {
        public abstract Type ComponentType { get; }

        public bool IsPortCompatible(GraphRuntime<GraphNode> graph, in PortId input, in PortId output) => true;
        public void OnConnected(GraphRuntime<GraphNode> graph, in PortId input, in PortId output) {}
        public void OnDisconnected(GraphRuntime<GraphNode> graph, in PortId input, in PortId output) {}
    }
}