using System;
using System.Collections.Generic;
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
    [DisallowMultipleComponent, ExecuteAlways, AddComponentMenu("")]
    public class BehaviorTreeNodeComponent : MonoBehaviour, INodeComponent<BehaviorTreeNode, BehaviorTreeNodeComponent>, ITreeNodeComponent
    {
        [SerializeField, Nuwa.ReadOnly, UnityDrawProperty] private string _id;
        public NodeId Id { get => Guid.Parse(_id); set => _id = value.ToString(); }

        [SerializeField, Nuwa.ReadOnly, UnityDrawProperty] private Vector2 _position;

        public Vector2 Position
        {
            get => _position;
            set
            {
                _position = value;
                var parent = transform.parent.GetComponent<BehaviorTreeNodeComponent>();
                if (parent) parent._reorderChildrenTransform(parent.transform);
            }
        }

        [SerializeField, UnboxSingleProperty, UnityDrawProperty] private BehaviorTreeNode _node;
        public BehaviorTreeNode Node { get => _node; set => _node = value; }

        public PortId InputPort => new PortId(Id, Node.InputPortName);
        public PortId OutputPort => new PortId(Id, Node.OutputPortName);

        public INodeComponent.NodeComponentConnect OnNodeComponentConnect { get; set; }
        public INodeComponent.NodeComponentDisconnect OnNodeComponentDisconnect { get; set; }

        private readonly TreeEdge _treeEdge;
        private readonly GraphExt.HashSet<EdgeId> _edges = new GraphExt.HashSet<EdgeId>();
        private readonly Lazy<SerializedObject> _serializedObject;
        private readonly Action<Transform> _reorderChildrenTransform = NodeTransform.ReorderChildrenTransformAction(node => node.Position.x);

        private SerializedProperty _SerializedNodeBuilder =>
            _serializedObject.Value.FindProperty(nameof(_node)).FindPropertyRelative(nameof(BehaviorTreeNode.Blob))
        ;

        public BehaviorTreeNodeComponent()
        {
            _serializedObject = new Lazy<SerializedObject>(() => new SerializedObject(this));
            _treeEdge = new TreeEdge(this);
        }

        public IReadOnlySet<EdgeId> GetEdges(GraphRuntime<BehaviorTreeNode> graph)
        {
            _edges.Clear();
            var treeEdge = _treeEdge.Edge;
            if (treeEdge.HasValue) _edges.Add(treeEdge.Value);
            return _edges;
        }

        public bool IsPortCompatible(GameObjectNodes<BehaviorTreeNode, BehaviorTreeNodeComponent> data, in PortId input, in PortId output)
        {
            // free to connect each other if they are not tree ports
            var isInputTreePort = data.Graph.IsTreePort(input);
            var isOutputTreePort = data.Graph.IsTreePort(output);
            if (!isInputTreePort && !isOutputTreePort) return true;

            if (input.NodeId == Id && output.NodeId == Id) return false; // same node
            if (input.NodeId == Id) return true; // only check compatible on output/start node
            // tree port must connect to another tree port
            if (!isInputTreePort || !isOutputTreePort) return false;
            // cannot connect to input/end node which is parent of output/start node to avoid circle dependency
            return !_treeEdge.IsParentInputPort(input);
        }

        public void OnConnected(GameObjectNodes<BehaviorTreeNode, BehaviorTreeNodeComponent> data, in EdgeId edge)
        {
            if (_edges.Contains(edge)) return;
            _edges.Add(edge);
            // set parent for tree edges
            _treeEdge.ConnectParent(edge, data[edge.Output.NodeId].transform);
        }

        public void OnDisconnected(GameObjectNodes<BehaviorTreeNode, BehaviorTreeNodeComponent> data, in EdgeId edge)
        {
            if (!_edges.Contains(edge)) return;
            // reset parent for tree edges
            _treeEdge.DisconnectParent(edge);
            _edges.Remove(edge);
        }

        public NodeData FindNodeProperties(GameObjectNodes<BehaviorTreeNode, BehaviorTreeNodeComponent> data)
        {
            var behaviorNodeType = Node.BehaviorNodeType;
            var properties = new List<INodeProperty>
            {
                CreateVerticalPorts(Node.InputPortName),
                new NodePositionProperty(Position.x, Position.y),
                new NodeClassesProperty(behaviorNodeType.ToString().ToLower().Yield()),
                new DynamicTitleProperty(() => name),
                new BehaviorBlobDataProperty(_SerializedNodeBuilder, Id)
            };
            if (behaviorNodeType != BehaviorNodeType.Action) properties.Add(CreateVerticalPorts(Node.OutputPortName));
            return new NodeData(properties);

            VerticalPortsProperty CreateVerticalPorts(string portName)
            {
                var verticalPorts = new VerticalPortsProperty { Name = portName };
                var portContainer = new PortContainerProperty(portName);
                verticalPorts.Ports.Add(portContainer);
                return verticalPorts;
            }
        }

        public IEnumerable<PortData> FindNodePorts(GameObjectNodes<BehaviorTreeNode, BehaviorTreeNodeComponent> data)
        {
            var behaviorNodeType = Node.BehaviorNodeType;
            yield return CreateBehaviorTreePortData(Node.InputPortName, PortDirection.Input, 1);

            if (behaviorNodeType == BehaviorNodeType.Composite)
                yield return CreateBehaviorTreePortData(Node.OutputPortName, PortDirection.Output, int.MaxValue);
            else if (behaviorNodeType == BehaviorNodeType.Decorate)
                yield return CreateBehaviorTreePortData(Node.OutputPortName, PortDirection.Output, 1);

            var behaviorNodeProperty = _SerializedNodeBuilder;
            while (behaviorNodeProperty.NextVisible(true))
            {
                var fieldType = behaviorNodeProperty.GetManagedFieldType();
                if (fieldType != null && typeof(IVariant).IsAssignableFrom(fieldType))
                {
                    yield return CreateVariantPortData(behaviorNodeProperty, fieldType, PortDirection.Input);
                    yield return CreateVariantPortData(behaviorNodeProperty, fieldType, PortDirection.Output);
                }
            }

            PortData CreateBehaviorTreePortData(string portName, PortDirection direction, int capacity)
            {
                return new PortData(
                    portName,
                    PortOrientation.Vertical,
                    direction,
                    capacity,
                    typeof(BehaviorTreeNode),
                    new []{"tree", behaviorNodeType.ToString().ToLower()}
                );
            }

            PortData CreateVariantPortData(SerializedProperty property, Type type, PortDirection direction)
            {
                return new PortData(
                    VariantPort.CreatePortName(property, direction),
                    PortOrientation.Horizontal,
                    direction,
                    1,
                    type,
                    new []{"variant"}
                );
            }
        }

        private void OnTransformParentChanged()
        {
            _treeEdge.OnTransformParentChanged();
        }

        private void OnBeforeTransformParentChanged()
        {
            _treeEdge.OnBeforeTransformParentChanged();
        }

        private void OnTransformChildrenChanged()
        {
            _reorderChildrenTransform(transform);
        }
    }
}