using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesBT.Editor;
using EntitiesBT.Variant;
using GraphExt;
using Nuwa;
using Nuwa.Editor;
using OneShot;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using GraphExt.Editor;
using UnityEngine.Assertions;
#endif

namespace EntitiesBT
{
    [DisallowMultipleComponent, ExecuteAlways, AddComponentMenu("")]
    public class VariantNodeComponent : MonoBehaviour, INodeComponent<VariantNode, VariantNodeComponent>, IGraphNodeComponent
    {
        [SerializeField] private bool _hideTitle = false;

        [SerializeField, ReadOnly, UnityDrawProperty] private string _id;
        public NodeId Id { get => Guid.Parse(_id); set => _id = value.ToString(); }

        [SerializeField] private Vector2 _position;
        public Vector2 Position { get => _position; set => _position = value; }

        [SerializeReference, UnboxSingleProperty, UnityDrawProperty] private VariantNode _node;
        public VariantNode Node { get => _node; set => _node = value; }

        public INodeComponent.NodeComponentConnect OnNodeComponentConnect { get; set; }
        public INodeComponent.NodeComponentDisconnect OnNodeComponentDisconnect { get; set; }

        [SerializeField, HideInInspector] private List<SerializableEdge> _serializableEdges = new List<SerializableEdge>();
        private readonly GraphExt.HashSet<EdgeId> _edges = new GraphExt.HashSet<EdgeId>();

        [Inject] private GameObjectNodes<BehaviorTreeNode, BehaviorTreeNodeComponent> _behaviorTreeNodes;

        public IReadOnlySet<EdgeId> GetEdges(GraphRuntime<VariantNode> graph)
        {
            _edges.Clear();
            foreach (var edge in _serializableEdges) _edges.Add(edge.ToEdge());
            return _edges;
        }

        public bool IsPortCompatible(GameObjectNodes<VariantNode, VariantNodeComponent> data, in PortId input, in PortId output)
        {
            if (input.NodeId == Id && output.NodeId == Id) return false; // same node
            Type variantType = null;
#if UNITY_EDITOR
            variantType = FindVariantProperty(input, output, data)?.GetManagedFullType();
#endif
            return variantType != null && Node.VariantType.IsAssignableFrom(variantType);
        }

        public void OnConnected(GameObjectNodes<VariantNode, VariantNodeComponent> data, in EdgeId edge)
        {
            if (_edges.Contains(edge)) return;
#if UNITY_EDITOR
            var graphNodeVariant = FindGraphNodeVariant(edge.Input, edge.Output, data);
            if (graphNodeVariant == null) return;
            Assert.IsTrue(Node.ValueType == null || Node.ValueType == graphNodeVariant.ValueType);
#endif
            var inputNodeId = edge.Input.NodeId;
            var outputNodeId = edge.Output.NodeId;
            foreach (var existEdge in _edges.Where(existEdge => existEdge.Input.NodeId == outputNodeId && existEdge.Output.NodeId == inputNodeId))
            {
                data.Runtime.Disconnect(existEdge.Input, existEdge.Output);
                break;
            }
#if UNITY_EDITOR
            graphNodeVariant.NodeComponent = this;
            Node.ValueType = graphNodeVariant.ValueType;
#endif
            name = Node.Name;
            _edges.Add(edge);
            _serializableEdges.Add(edge.ToSerializable());
        }

        public void OnDisconnected(GameObjectNodes<VariantNode, VariantNodeComponent> data, in EdgeId edge)
        {
            if (!_edges.Contains(edge)) return;
#if UNITY_EDITOR
            var graphNodeVariant = FindGraphNodeVariant(edge.Input, edge.Output, data);
            if (graphNodeVariant == null) return;
            graphNodeVariant.NodeComponent = null;
            if (!_edges.Any()) Node.ValueType = null;
#endif
            name = Node.Name;
            _edges.Remove(edge);
            _serializableEdges.Remove(edge.ToSerializable());
        }

#if UNITY_EDITOR
        private SerializedProperty[] _variantProperties;
        private readonly EventTitleProperty _titleProperty = new EventTitleProperty();

        static VariantNodeComponent()
        {
            GraphUtility.RegisterNameChanged<VariantNodeComponent>(variant =>
                variant._titleProperty.Title = variant._hideTitle ? null : variant.name
            );
        }

        private SerializedProperty FindVariantProperty(in PortId input, in PortId output, GameObjectNodes<VariantNode, VariantNodeComponent> nodes)
        {
            if (input.NodeId == Id && input.Name == VariantNode.INPUT_PORT)
                return output.FindVariantPortProperty(nodes, _behaviorTreeNodes);
            if (output.NodeId == Id && output.Name == VariantNode.OUTPUT_PORT)
                return input.FindVariantPortProperty(nodes, _behaviorTreeNodes);
            return null;
        }

        private GraphNodeVariant.Any FindGraphNodeVariant(in PortId input, in PortId output, GameObjectNodes<VariantNode, VariantNodeComponent> nodes)
        {
            var variantProperty = FindVariantProperty(input, output, nodes);
            if (variantProperty == null) return null;
            return (GraphNodeVariant.Any) variantProperty.GetObject();
        }

        public NodeData FindNodeProperties(SerializedObject nodeObject)
        {
            _titleProperty.Title = name;
            var properties = new List<INodeProperty>
            {
                new NodeSerializedPositionProperty { PositionProperty = nodeObject.FindProperty(nameof(_position)) },
                new LabelValuePortProperty(_titleProperty, null, new PortContainerProperty(VariantNode.INPUT_PORT), new PortContainerProperty(VariantNode.OUTPUT_PORT)),
                new NodeSerializedProperty(nodeObject.FindProperty(nameof(_node)))
            };
            return new NodeData(properties);
        }

        public IEnumerable<PortData> FindNodePorts(SerializedObject nodeObject)
        {
            yield return new PortData(VariantNode.INPUT_PORT, PortOrientation.Horizontal, PortDirection.Input, int.MaxValue, Node.VariantType);
            yield return new PortData(VariantNode.OUTPUT_PORT, PortOrientation.Horizontal, PortDirection.Output, int.MaxValue, Node.VariantType);
            _variantProperties ??= GetVariantProperties(nodeObject).ToArray();
            foreach (var variant in _variantProperties)
            {
                var variantType = variant.GetManagedFullType();
                if (variantType != null && typeof(GraphNodeVariant.Any).IsAssignableFrom(variantType))
                {
                    yield return CreateVariantPortData(variant, variantType, PortDirection.Input);
                    yield return CreateVariantPortData(variant, variantType, PortDirection.Output);
                }
            }

            PortData CreateVariantPortData(SerializedProperty property, Type variantType, PortDirection direction)
            {
                return new PortData(
                    VariantPort.CreatePortName(property, direction),
                    PortOrientation.Horizontal,
                    direction,
                    1,
                    VariantPort.GetPortType(variantType),
                    new []{"variant"}
                );
            }
        }

        private IEnumerable<SerializedProperty> GetVariantProperties(SerializedObject nodeObject)
        {
            var serializedNodeBuilder = GetSerializedNodeBuilder(nodeObject);
            while (serializedNodeBuilder.NextVisible(true))
            {
                var fieldType = serializedNodeBuilder.GetManagedFieldType();
                if (fieldType != null && typeof(IVariant).IsAssignableFrom(fieldType))
                    yield return serializedNodeBuilder.Copy();
            }
        }

        private SerializedProperty GetSerializedNodeBuilder(SerializedObject nodeObject)
        {
            return nodeObject.FindProperty(nameof(_node)).FindPropertyRelative(nameof(VariantNode<IVariant>.Value));
        }
#endif

    }
}
