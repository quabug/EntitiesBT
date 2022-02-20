using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesBT.Core;
using EntitiesBT.Editor;
using EntitiesBT.Variant;
using GraphExt;
using GraphExt.Editor;
using Nuwa;
using Nuwa.Editor;
using UnityEditor;
using UnityEngine;

namespace EntitiesBT
{
    public class BehaviorNodeComponent : GraphNodeComponent, ITreeNodeComponent
    {
        [SerializeField, HideInInspector] private bool _expanded = false;

        public override Vector2 Position
        {
            get => _Position;
            set
            {
                _Position = value;
                var parent = transform.parent.GetComponent<BehaviorNodeComponent>();
                if (parent) parent._reorderChildrenTransform(parent.transform);
            }
        }

        [SerializeField, UnboxSingleProperty, UnityDrawProperty] private BehaviorNode _node;
        public override GraphNode Node { get => _node; set => _node = (BehaviorNode)value; }

        public PortId InputPort => new PortId(Id, _node.InputPortName);
        public PortId OutputPort => new PortId(Id, _node.OutputPortName);

        private readonly TreeEdge _treeEdge = new TreeEdge();
        private readonly GraphExt.HashSet<EdgeId> _edges = new GraphExt.HashSet<EdgeId>();
        private readonly Action<Transform> _reorderChildrenTransform = NodeTransform.ReorderChildrenTransformAction(node => node.Position.x);

        public override IReadOnlySet<EdgeId> GetEdges(GraphRuntime<GraphNode> graph)
        {
            _edges.Clear();
            var treeEdge = _treeEdge.Edge(gameObject);
            if (treeEdge.HasValue) _edges.Add(treeEdge.Value);
            return _edges;
        }

        public override bool IsPortCompatible(GameObjectNodes<GraphNode, GraphNodeComponent> data, in PortId input, in PortId output)
        {
            // free to connect each other if they are not tree ports
            var isInputTreePort = data.Runtime.IsTreePort(input);
            var isOutputTreePort = data.Runtime.IsTreePort(output);
            if (!isInputTreePort && !isOutputTreePort) return true;

            if (input.NodeId == Id && output.NodeId == Id) return false; // same node
            if (input.NodeId == Id) return true; // only check compatible on output/start node
            // tree port must connect to another tree port
            if (!isInputTreePort || !isOutputTreePort) return false;
            // cannot connect to input/end node which is parent of output/start node to avoid circle dependency
            return !_treeEdge.IsParentInputPort(gameObject, input);
        }

        public override void OnConnected(GameObjectNodes<GraphNode, GraphNodeComponent> data, in EdgeId edge)
        {
            if (!data.Runtime.IsTreeEdge(edge) || _edges.Contains(edge)) return;
            _edges.Add(edge);
            // set parent for tree edges
            _treeEdge.ConnectParent(this, edge, data[edge.Output.NodeId].transform);
        }

        public override void OnDisconnected(GameObjectNodes<GraphNode, GraphNodeComponent> data, in EdgeId edge)
        {
            if (!_edges.Contains(edge)) return;
            // reset parent for tree edges
            _treeEdge.DisconnectParent(this, edge);
            _edges.Remove(edge);
        }

        private void OnTransformParentChanged()
        {
            _treeEdge.OnTransformParentChanged(this);
        }

        private void OnBeforeTransformParentChanged()
        {
            _treeEdge.OnBeforeTransformParentChanged(this);
        }

        private void OnTransformChildrenChanged()
        {
            _reorderChildrenTransform(transform);
        }

#if UNITY_EDITOR
        private SerializedProperty[] _variantProperties;
        private readonly EventTitleProperty _titleProperty = new EventTitleProperty();

        static BehaviorNodeComponent()
        {
            GraphUtility.RegisterNameChanged<BehaviorNodeComponent>(node => node._titleProperty.Title = node.name);
        }

        public override NodeData FindNodeProperties(SerializedObject nodeObject)
        {
            _titleProperty.Title = name;
            var behaviorNodeType = _node.BehaviorNodeType;
            var properties = new List<INodeProperty>
            {
                CreateVerticalPorts(_node.InputPortName, -100),
                new NodeSerializedPositionProperty { PositionProperty = nodeObject.FindProperty(nameof(_Position)) },
                new NodeClassesProperty("behavior-node", behaviorNodeType.ToString().ToLower()),
                new NodeTitleProperty { TitleProperty = _titleProperty, ToggleProperty = new FoldoutProperty { BoolProperty = nodeObject.FindProperty(nameof(_expanded)) } },
                new NodeSerializedProperty(GetSerializedNodeBuilder(nodeObject)) { HideFoldoutToggle = true, ToggleProperty = nodeObject.FindProperty(nameof(_expanded)) }
            };
            if (behaviorNodeType != BehaviorNodeType.Action) properties.Add(CreateVerticalPorts(_node.OutputPortName, 100));
            return new NodeData(properties);

            VerticalPortsProperty CreateVerticalPorts(string portName, int order)
            {
                var verticalPorts = new VerticalPortsProperty { Name = portName, Order = order };
                var portContainer = new PortContainerProperty(portName);
                verticalPorts.Ports.Add(portContainer);
                return verticalPorts;
            }
        }

        public override IEnumerable<PortData> FindNodePorts(SerializedObject nodeObject)
        {
            var behaviorNodeType = _node.BehaviorNodeType;
            yield return CreateBehaviorTreePortData(_node.InputPortName, PortDirection.Input, 1);

            if (behaviorNodeType == BehaviorNodeType.Composite)
                yield return CreateBehaviorTreePortData(_node.OutputPortName, PortDirection.Output, int.MaxValue);
            else if (behaviorNodeType == BehaviorNodeType.Decorate)
                yield return CreateBehaviorTreePortData(_node.OutputPortName, PortDirection.Output, 1);

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

            PortData CreateBehaviorTreePortData(string portName, PortDirection direction, int capacity)
            {
                return new PortData(
                    portName,
                    PortOrientation.Vertical,
                    direction,
                    capacity,
                    typeof(BehaviorNode),
                    "tree",
                    behaviorNodeType.ToString().ToLower()
                );
            }

            PortData CreateVariantPortData(SerializedProperty property, Type variantType, PortDirection direction)
            {
                return new PortData(
                    VariantPort.CreatePortName(property, direction),
                    PortOrientation.Horizontal,
                    direction,
                    1,
                    VariantPort.GetPortType(variantType),
                    VariantPort.GetPortClasses(variantType).ToArray()
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
            return nodeObject.FindProperty(nameof(_node)).FindPropertyRelative(nameof(BehaviorNode.Blob));
        }
#endif

    }
}