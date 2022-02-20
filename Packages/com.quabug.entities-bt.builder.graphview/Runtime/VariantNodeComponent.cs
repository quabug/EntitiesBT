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
    public class VariantNodeComponent : GraphNodeComponent
    {
        [SerializeField] private bool _hideTitle = false;

        [SerializeReference, UnboxSingleProperty, UnityDrawProperty] public VariantNode VariantNode;
        public override GraphNode Node { get => VariantNode; set => VariantNode = (VariantNode)value; }

        [SerializeField, HideInInspector] private List<SerializableEdge> _serializableEdges = new List<SerializableEdge>();
        private readonly GraphExt.HashSet<EdgeId> _edges = new GraphExt.HashSet<EdgeId>();
        private readonly Dictionary<EdgeId, GraphNodeVariant.Any> _variantConnections = new Dictionary<EdgeId, GraphNodeVariant.Any>();

        [Inject]
        void Inject(GameObjectNodes<GraphNode, GraphNodeComponent> nodes)
        {
#if UNITY_EDITOR
            VariantNode.ConnectedVariants.Clear();
            _variantConnections.Clear();
            foreach (var edge in _edges)
            {
                var graphNodeVariant = FindGraphNodeVariant(nodes, edge);
                _variantConnections.Add(edge, graphNodeVariant);
                VariantNode.ConnectedVariants.Add(graphNodeVariant);
            }
#endif
        }

        public override IReadOnlySet<EdgeId> GetEdges(GraphRuntime<GraphNode> graph)
        {
            _edges.Clear();
            foreach (var edge in _serializableEdges) _edges.Add(edge.ToEdge());
            return _edges;
        }

        public override bool IsPortCompatible(GameObjectNodes<GraphNode, GraphNodeComponent> data, in PortId input, in PortId output)
        {
            if (input.NodeId == Id && output.NodeId == Id) return false; // same node
            Type variantType = null;
#if UNITY_EDITOR
            variantType = FindVariantProperty(data, input, output)?.GetManagedFullType();
#endif
            return variantType != null && VariantNode.VariantType.IsAssignableFrom(variantType);
        }

        public override void OnConnected(GameObjectNodes<GraphNode, GraphNodeComponent> data, in EdgeId edge)
        {
            if (_edges.Contains(edge)) return;
#if UNITY_EDITOR
            var graphNodeVariant = FindGraphNodeVariant(data, edge);
            if (graphNodeVariant == null) return;
            Assert.IsTrue(VariantNode.ValueType == null || VariantNode.ValueType == graphNodeVariant.ValueType);
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
            _variantConnections.Add(edge, graphNodeVariant);
            VariantNode.ConnectedVariants.Add(graphNodeVariant);
            if (VariantNode.Variant == null || VariantNode.Variant.FindValueType() != VariantNode.ValueType)
            {
                try
                {
                    var variantType = TypeCache.GetTypesDerivedFrom(VariantNode.BaseType).First();
                    VariantNode.Variant = (IVariant)Activator.CreateInstance(variantType);
                }
                catch
                {
                    // ignored
                }
            }
#endif
            name = VariantNode.Name;
            _edges.Add(edge);
            _serializableEdges.Add(edge.ToSerializable());
        }

        public override void OnDisconnected(GameObjectNodes<GraphNode, GraphNodeComponent> data, in EdgeId edge)
        {
            if (!_edges.Contains(edge)) return;
#if UNITY_EDITOR
            _variantConnections.TryGetValue(edge, out var graphNodeVariant);
            if (graphNodeVariant == null) return;
            graphNodeVariant.NodeComponent = null;
            VariantNode.ConnectedVariants.Remove(graphNodeVariant);
            _variantConnections.Remove(edge);
#endif
            name = VariantNode.Name;
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

        private SerializedProperty FindVariantProperty(GameObjectNodes<GraphNode, GraphNodeComponent> nodes, in PortId input, in PortId output)
        {
            if (input.NodeId == Id && input.Name == VariantNode.INPUT_PORT)
                return output.FindVariantPortProperty(nodes);
            if (output.NodeId == Id && output.Name == VariantNode.OUTPUT_PORT)
                return input.FindVariantPortProperty(nodes);
            return null;
        }

        private GraphNodeVariant.Any FindGraphNodeVariant(GameObjectNodes<GraphNode, GraphNodeComponent> nodes, in PortId input, in PortId output)
        {
            var variantProperty = FindVariantProperty(nodes, input, output);
            if (variantProperty == null) return null;
            return (GraphNodeVariant.Any) variantProperty.GetObject();
        }

        private GraphNodeVariant.Any FindGraphNodeVariant(GameObjectNodes<GraphNode, GraphNodeComponent> nodes, in EdgeId edge)
        {
            return FindGraphNodeVariant(nodes, edge.Input, edge.Output);
        }

        public override NodeData FindNodeProperties(SerializedObject nodeObject)
        {
            _titleProperty.Title = name;
            var properties = new List<INodeProperty>
            {
                new NodeClassesProperty("variant-node", VariantNode.AccessName),
                new NodeSerializedPositionProperty { PositionProperty = nodeObject.FindProperty(nameof(_Position)) },
                new LabelValuePortProperty(_titleProperty, null, new PortContainerProperty(VariantNode.INPUT_PORT), new PortContainerProperty(VariantNode.OUTPUT_PORT)),
                new NodeSerializedProperty(nodeObject.FindProperty(nameof(VariantNode)))
            };
            return new NodeData(properties);
        }

        public override IEnumerable<PortData> FindNodePorts(SerializedObject nodeObject)
        {
            var portClasses = VariantPort.GetPortClasses(VariantNode.VariantType).ToArray();
            yield return new PortData(VariantNode.INPUT_PORT, PortOrientation.Horizontal, PortDirection.Input, int.MaxValue, VariantNode.VariantType, portClasses);
            yield return new PortData(VariantNode.OUTPUT_PORT, PortOrientation.Horizontal, PortDirection.Output, int.MaxValue, VariantNode.VariantType, portClasses);
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
            return nodeObject.FindProperty(nameof(VariantNode)).FindPropertyRelative(nameof(VariantNode<IVariant>.Value));
        }
#endif

    }
}
