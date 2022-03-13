using System;
using System.Collections.Generic;
using GraphExt;
using Nuwa;
using UnityEngine;

namespace EntitiesBT
{
    public abstract class GraphNodeComponent : MonoBehaviour, INodeComponent<GraphNode, GraphNodeComponent>
    {
        [SerializeField, ReadOnly, UnityDrawProperty] protected string _Id;
        public NodeId Id { get => Guid.Parse(_Id); set => _Id = value.ToString(); }

        [SerializeField] protected Vector2 _Position;
        public virtual Vector2 Position { get => _Position; set => _Position = value; }

        public INodeComponent.NodeComponentConnect OnNodeComponentConnect { get; set; }
        public INodeComponent.NodeComponentDisconnect OnNodeComponentDisconnect { get; set; }

        public abstract GraphNode Node { get; set; }

        public abstract IReadOnlySet<EdgeId> GetEdges(GraphRuntime<GraphNode> graph);
        public abstract bool IsPortCompatible(GameObjectNodes<GraphNode, GraphNodeComponent> data, in PortId input, in PortId output);
        public abstract void OnConnected(GameObjectNodes<GraphNode, GraphNodeComponent> data, in EdgeId edge);
        public abstract void OnDisconnected(GameObjectNodes<GraphNode, GraphNodeComponent> data, in EdgeId edge);

#if UNITY_EDITOR
        public abstract NodeData FindNodeProperties(UnityEditor.SerializedObject nodeObject);
        public abstract IEnumerable<PortData> FindNodePorts(UnityEditor.SerializedObject nodeObject);
#endif
    }
}