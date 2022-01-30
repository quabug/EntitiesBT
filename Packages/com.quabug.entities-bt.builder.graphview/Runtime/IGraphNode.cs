using System.Collections.Generic;
using GraphExt;

namespace EntitiesBT
{
    public interface IGraphNode : INode<GraphRuntime<IGraphNode>>
    {
        IReadOnlySet<EdgeId> GetEdges(GraphRuntime<IGraphNode> graph);
        bool IsPortCompatible(GameObjectNodes<IGraphNode, GraphNodeComponent> data, in PortId input, in PortId output);
        void OnConnected(GameObjectNodes<IGraphNode, GraphNodeComponent> data, in EdgeId edge);
        void OnDisconnected(GameObjectNodes<IGraphNode, GraphNodeComponent> data, in EdgeId edge);
    }

    public abstract class BaseGraphNode : IGraphNode
    {
        public virtual bool IsPortCompatible(GraphRuntime<IGraphNode> graph, in PortId input, in PortId output)
        {
            return true;
        }

        public virtual void OnConnected(GraphRuntime<IGraphNode> graph, in PortId input, in PortId output)
        {
        }

        public virtual void OnDisconnected(GraphRuntime<IGraphNode> graph, in PortId input, in PortId output)
        {
        }

        public abstract IReadOnlySet<EdgeId> GetEdges(GraphRuntime<IGraphNode> graph);

        public bool IsPortCompatible(GameObjectNodes<IGraphNode, GraphNodeComponent> data, in PortId input, in PortId output)
        {
            throw new System.NotImplementedException();
        }

        public void OnConnected(GameObjectNodes<IGraphNode, GraphNodeComponent> data, in EdgeId edge)
        {
            throw new System.NotImplementedException();
        }

        public void OnDisconnected(GameObjectNodes<IGraphNode, GraphNodeComponent> data, in EdgeId edge)
        {
            throw new System.NotImplementedException();
        }
    }
}