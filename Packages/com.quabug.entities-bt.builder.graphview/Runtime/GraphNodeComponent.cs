using System;
using System.Collections.Generic;
using GraphExt;
using GraphExt.Editor;
using Nuwa;
using UnityEngine;

namespace EntitiesBT
{
    [AddComponentMenu("")]
    public class GraphNodeComponent : MonoBehaviour, INodeComponent<IGraphNode, GraphNodeComponent>
    {
        [SerializeField, ReadOnly, UnityDrawProperty] private string _id;
        public NodeId Id { get => Guid.Parse(_id); set => _id = value.ToString(); }

        [SerializeField] private Vector2 _position;
        public Vector2 Position { get => _position; set => _position = value; }

        [SerializeField] public string _title;

        public INodeComponent.NodeComponentConnect OnNodeComponentConnect { get; set; }
        public INodeComponent.NodeComponentDisconnect OnNodeComponentDisconnect { get; set; }

        [SerializeReference] private IGraphNode _node;
        public IGraphNode Node { get => _node; set => _node = value; }

        public IReadOnlySet<EdgeId> GetEdges(GraphRuntime<IGraphNode> graph)
        {
            return Node.GetEdges(graph);
        }

        public bool IsPortCompatible(
            GameObjectNodes<IGraphNode, GraphNodeComponent> data,
            in PortId input,
            in PortId output
        )
        {
            return Node.IsPortCompatible(data, input: input, output: output);
        }

        public void OnConnected(GameObjectNodes<IGraphNode, GraphNodeComponent> data, in EdgeId edge)
        {
            Node.OnConnected(data, edge);
        }

        public void OnDisconnected(GameObjectNodes<IGraphNode, GraphNodeComponent> data, in EdgeId edge)
        {
            Node.OnDisconnected(data, edge);
        }

#if UNITY_EDITOR
        static GraphNodeComponent()
        {
            UnityEditor.EditorApplication.hierarchyChanged -= RefreshTitles;
            UnityEditor.EditorApplication.hierarchyChanged += RefreshTitles;

            void RefreshTitles()
            {
                var prefabStage = UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
                if (prefabStage == null) return;
                foreach (var node in prefabStage.prefabContentsRoot.GetComponentsInChildren<GraphNodeComponent>())
                    node._title = node.name;
            }
        }

        public NodeData CreateNodeProperties(UnityEditor.SerializedObject nodeObject)
        {
            nodeObject.Update();
            var properties = new List<INodeProperty>();
            var positionProperty = new NodeSerializedPositionProperty
            {
                PositionProperty = nodeObject.FindProperty(nameof(_position))
            };
            properties.Add(positionProperty);

            var titleProperty = new SerializedTitleProperty
            {
                Property = nodeObject.FindProperty(nameof(_title))
            };
            properties.Add(titleProperty);

            properties.AddRange(_node.CreateNodeProperties( nodeObject.FindProperty(nameof(_node))));

            return new NodeData(properties);
        }

        public IEnumerable<PortData> FindNodePorts(UnityEditor.SerializedObject nodeObject)
        {
            nodeObject.Update();
            return _node.FindNodePorts(nodeObject.FindProperty(nameof(_node)));
        }
#endif
    }
}