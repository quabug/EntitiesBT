using System;
using System.Collections.Generic;
using GraphExt;
using UnityEngine;

namespace EntitiesBT
{
    public interface IGraphNode : INode<GraphRuntime<IGraphNode>>
    {
        GraphNodeComponent GraphNodeComponent { get; set; }
        IReadOnlySet<EdgeId> GetEdges(GraphRuntime<IGraphNode> graph);
        bool IsPortCompatible(GameObjectNodes<IGraphNode, GraphNodeComponent> data, in PortId input, in PortId output);
        void OnConnected(GameObjectNodes<IGraphNode, GraphNodeComponent> data, in EdgeId edge);
        void OnDisconnected(GameObjectNodes<IGraphNode, GraphNodeComponent> data, in EdgeId edge);

#if UNITY_EDITOR
        IEnumerable<PortData> FindNodePorts(UnityEditor.SerializedProperty thisProperty);
        IEnumerable<INodeProperty> CreateNodeProperties(UnityEditor.SerializedProperty thisProperty);
#endif
    }

    [Serializable]
    public abstract class BaseGraphNode : IGraphNode
    {
        [field: SerializeField] public GraphNodeComponent GraphNodeComponent { get; set; }

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
        public abstract bool IsPortCompatible(GameObjectNodes<IGraphNode, GraphNodeComponent> data, in PortId input, in PortId output);
        public abstract void OnConnected(GameObjectNodes<IGraphNode, GraphNodeComponent> data, in EdgeId edge);
        public abstract void OnDisconnected(GameObjectNodes<IGraphNode, GraphNodeComponent> data, in EdgeId edge);

#if UNITY_EDITOR
        public abstract IEnumerable<PortData> FindNodePorts(UnityEditor.SerializedProperty thisProperty);
        public abstract IEnumerable<INodeProperty> CreateNodeProperties(UnityEditor.SerializedProperty thisProperty);
#endif
    }
}