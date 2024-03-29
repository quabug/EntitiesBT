using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesBT.Variant;
using GraphExt;
using Nuwa;
using UnityEngine;

#if UNITY_EDITOR
using Nuwa.Editor;
using EntitiesBT.Editor;
using UnityEditor;
using GraphExt.Editor;
using UnityEngine.Assertions;
#endif

namespace EntitiesBT
{
    [DisallowMultipleComponent, ExecuteAlways, AddComponentMenu("")]
    public class VariantNodeComponent : GraphNodeComponent
    {
        [SerializeReference, UnboxSingleProperty, UnityDrawProperty] public VariantNode VariantNode;
        public override GraphNode Node { get => VariantNode; set => VariantNode = (VariantNode)value; }

        [SerializeField, HideInInspector] private List<SerializableEdge> _serializableEdges = new List<SerializableEdge>();
        private readonly GraphExt.HashSet<EdgeId> _edges = new GraphExt.HashSet<EdgeId>();
        private readonly Dictionary<EdgeId, GraphNodeVariant.Any> _variantConnections = new Dictionary<EdgeId, GraphNodeVariant.Any>();

        [SerializeField] private bool _expanded = false;

#if UNITY_EDITOR
        [Inject]
        void Inject(GameObjectNodes<GraphNode, GraphNodeComponent> nodes)
        {
            VariantNode.ConnectedVariants.Clear();
            _variantConnections.Clear();
            foreach (var edge in _edges)
            {
                var graphNodeVariant = FindGraphNodeVariant(nodes, edge);
                _variantConnections.Add(edge, graphNodeVariant);
                VariantNode.ConnectedVariants.Add(graphNodeVariant);
            }
        }
#endif

        public override IReadOnlySet<EdgeId> GetEdges(GraphRuntime<GraphNode> graph)
        {
            _edges.Clear();
            foreach (var edge in _serializableEdges) _edges.Add(edge.ToEdge());
            return _edges;
        }

        public override bool IsPortCompatible(GameObjectNodes<GraphNode, GraphNodeComponent> data, in PortId input, in PortId output)
        {
            if (input.NodeId == Id && output.NodeId == Id) return false; // same node
            Type graphNodeVariantType = null;
#if UNITY_EDITOR
            graphNodeVariantType = FindGraphNodeVariantProperty(data, input, output)?.GetManagedFullType();
#endif
            if (graphNodeVariantType == null) return false;
            var graphNodeVariantValueType = graphNodeVariantType.GetInterface(typeof(IVariant<int>).Name).GenericTypeArguments[0];
            if (VariantNode.ValueType != null && VariantNode.ValueType != graphNodeVariantValueType) return false;
            var variantType = VariantNode.VariantType;
            return typeof(IVariantReaderAndWriter).IsAssignableFrom(variantType) ||
                   HasSameBaseType(typeof(IVariantReader), variantType, graphNodeVariantType) ||
                   HasSameBaseType(typeof(IVariantWriter), variantType, graphNodeVariantType)
            ;

            bool HasSameBaseType(Type baseType, Type first, Type second)
            {
                return baseType.IsAssignableFrom(first) && baseType.IsAssignableFrom(second);
            }
        }

        public override void OnConnected(GameObjectNodes<GraphNode, GraphNodeComponent> data, in EdgeId edge)
        {
#if UNITY_EDITOR
            if (_edges.Contains(edge)) return;
            var graphNodeVariant = FindGraphNodeVariant(data, edge);
            if (graphNodeVariant == null) return;
            Assert.IsTrue(VariantNode.ValueType == null || VariantNode.ValueType == graphNodeVariant.ValueType);

            PortId disconnectingPort;
            if (Id == edge.Input.NodeId)
            {
                VariantPorts.Deconstruct(edge.Output, out var nodeId, out var propertyPath, out _);
                disconnectingPort = VariantPorts.Construct(nodeId, propertyPath, PortDirection.Input);
            }
            else
            {
                VariantPorts.Deconstruct(edge.Input, out var nodeId, out var propertyPath, out _);
                disconnectingPort = VariantPorts.Construct(nodeId, propertyPath, PortDirection.Output);
            }

            foreach (var disconnectingEdge in data.Runtime.Edges.Where(existEdge => existEdge.Contains(disconnectingPort)).ToArray())
            {
                data.Runtime.Disconnect(disconnectingEdge.Input, disconnectingEdge.Output);
            }

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
            name = VariantNode.Name;
            _edges.Add(edge);
            _serializableEdges.Add(edge.ToSerializable());
#endif
        }

        public override void OnDisconnected(GameObjectNodes<GraphNode, GraphNodeComponent> data, in EdgeId edge)
        {
#if UNITY_EDITOR
            if (!_edges.Contains(edge)) return;
            _variantConnections.TryGetValue(edge, out var graphNodeVariant);
            if (graphNodeVariant == null) return;
            graphNodeVariant.NodeComponent = null;
            VariantNode.ConnectedVariants.Remove(graphNodeVariant);
            _variantConnections.Remove(edge);

            name = VariantNode.Name;
            _edges.Remove(edge);
            _serializableEdges.Remove(edge.ToSerializable());
#endif
        }

#if UNITY_EDITOR
        private readonly VariantPorts _variantPorts = new VariantPorts();
        private readonly NodeTitleProperty _titleProperty = new NodeTitleProperty();

        static VariantNodeComponent()
        {
            GraphUtility.RegisterNameChanged<VariantNodeComponent>(variant => variant._titleProperty.Title = variant.name);
        }

        private SerializedProperty FindGraphNodeVariantProperty(GameObjectNodes<GraphNode, GraphNodeComponent> nodes, in PortId input, in PortId output)
        {
            if (input.NodeId == Id && input.Name == VariantNode.INPUT_PORT)
                return VariantPorts.FindGraphNodeVariantPortProperty(output, nodes);
            if (output.NodeId == Id && output.Name == VariantNode.OUTPUT_PORT)
                return VariantPorts.FindGraphNodeVariantPortProperty(input, nodes);
            return null;
        }

        private GraphNodeVariant.Any FindGraphNodeVariant(GameObjectNodes<GraphNode, GraphNodeComponent> nodes, in PortId input, in PortId output)
        {
            var variantProperty = FindGraphNodeVariantProperty(nodes, input, output);
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
            _titleProperty.ToggleProperty = nodeObject.FindProperty(nameof(_expanded));
            var properties = new List<INodeProperty>
            {
                new NodeClassesProperty("variant-node", VariantNode.AccessName),
                new NodeSerializedPositionProperty { PositionProperty = nodeObject.FindProperty(nameof(_Position)) },
                new VariantNodeTitleProperty(_titleProperty, VariantNode.INPUT_PORT, VariantNode.OUTPUT_PORT),
                new NodeSerializedProperty(nodeObject.FindProperty(nameof(VariantNode))) { HideFoldoutToggle = true, ToggleProperty = _titleProperty.ToggleProperty }
            };
            return new NodeData(properties);
        }

        public override IEnumerable<PortData> FindNodePorts(SerializedObject nodeObject)
        {
            var portClasses = VariantPorts.GetPortClasses(VariantNode.VariantType).ToArray();
            yield return new PortData(VariantNode.INPUT_PORT, PortOrientation.Horizontal, PortDirection.Input, int.MaxValue, VariantNode.VariantType, portClasses);
            yield return new PortData(VariantNode.OUTPUT_PORT, PortOrientation.Horizontal, PortDirection.Output, int.MaxValue, VariantNode.VariantType, portClasses);
            foreach (var variantPort in _variantPorts.FindNodePorts(GetSerializedNodeBuilder(nodeObject))) yield return variantPort;
        }

        private SerializedProperty GetSerializedNodeBuilder(SerializedObject nodeObject)
        {
            return nodeObject.FindProperty(nameof(VariantNode)).FindPropertyRelative(nameof(VariantNode<IVariant>.Value));
        }
#endif

    }
}
