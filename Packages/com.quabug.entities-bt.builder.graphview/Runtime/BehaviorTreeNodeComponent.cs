using System;
using System.Collections.Generic;
using EntitiesBT.Core;
using EntitiesBT.Editor;
using GraphExt;
using GraphExt.Editor;
using Nuwa;
using UnityEditor;
using UnityEngine;

namespace EntitiesBT
{
    public class BehaviorTreeNodeComponent : MonoBehaviour, INodeComponent<BehaviorTreeNode, BehaviorTreeNodeComponent>, ITreeNodeComponent
    {
        [SerializeField, Nuwa.ReadOnly, UnityDrawProperty] private string _id;
        public NodeId Id { get => Guid.Parse(_id); set => _id = value.ToString(); }

        [field: SerializeField, Nuwa.ReadOnly, UnityDrawProperty] public Vector2 Position { get; set; }

        [SerializeField, UnboxSingleProperty, UnityDrawProperty] private BehaviorTreeNode _node;
        public BehaviorTreeNode Node { get => _node; set => _node = value; }

        public PortId InputPort => new PortId(Id, Node.InputPortName);
        public PortId OutputPort => new PortId(Id, Node.OutputPortName);

        public INodeComponent.NodeComponentConnect OnNodeComponentConnect { get; set; }
        public INodeComponent.NodeComponentDisconnect OnNodeComponentDisconnect { get; set; }

        private TreeEdge _TreeEdge => GetComponent<TreeEdge>() ?? gameObject.AddComponent<TreeEdge>();

        private readonly GraphExt.HashSet<EdgeId> _edges = new GraphExt.HashSet<EdgeId>();

        private Lazy<SerializedObject> _serializedObject;

        private SerializedProperty _SerializedBuilder =>
            _serializedObject.Value.FindProperty(nameof(_node)).FindPropertyRelative(nameof(BehaviorTreeNode.Blob))
        ;

        public BehaviorTreeNodeComponent()
        {
            _serializedObject = new Lazy<SerializedObject>(() => new SerializedObject(this));
        }

        public IReadOnlySet<EdgeId> GetEdges(GraphRuntime<BehaviorTreeNode> graph)
        {
            _edges.Clear();
            var treeEdge = _TreeEdge.Edge;
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
            return !_TreeEdge.IsParentInputPort(input);
        }

        public void OnConnected(GameObjectNodes<BehaviorTreeNode, BehaviorTreeNodeComponent> data, in EdgeId edge)
        {
            if (_edges.Contains(edge)) return;
            _edges.Add(edge);
            // set parent for tree edges
            _TreeEdge.ConnectParent(edge, data[edge.Output.NodeId].transform);
        }

        public void OnDisconnected(GameObjectNodes<BehaviorTreeNode, BehaviorTreeNodeComponent> data, in EdgeId edge)
        {
            if (!_edges.Contains(edge)) return;
            // reset parent for tree edges
            _TreeEdge.DisconnectParent(edge);
            _edges.Remove(edge);
        }

        public NodeData FindNodeProperties(GameObjectNodes<BehaviorTreeNode, BehaviorTreeNodeComponent> data)
        {
            var behaviorNodeType = Node.BehaviorNodeType;
            var properties = new List<INodeProperty>
            {
                CreateVerticalPorts(Node.InputPortName),
                new NodePositionProperty(Position.x, Position.y),
                new NodeClassesProperty { AdditionalClasses = new[] { behaviorNodeType.ToString().ToLower() } },
                new DynamicTitleProperty(() => name),
                new BehaviorBlobDataProperty { DynamicNodeBuilderProperty = _SerializedBuilder }
            };
            if (behaviorNodeType != BehaviorNodeType.Action) properties.Add(CreateVerticalPorts(Node.OutputPortName));
            return new NodeData(properties);

            VerticalPortsProperty CreateVerticalPorts(string portName)
            {
                var portContainer = new PortContainerProperty(new PortId(Id, portName));
                return new VerticalPortsProperty { Name = portName, Ports = new List<INodeProperty> { portContainer } };
            }
        }

        public IEnumerable<PortData> FindNodePorts(GameObjectNodes<BehaviorTreeNode, BehaviorTreeNodeComponent> data)
        {
            var behaviorNodeType = Node.BehaviorNodeType;
            yield return CreatePortData(Node.InputPortName, PortDirection.Input, 1);
            if (behaviorNodeType == BehaviorNodeType.Composite) yield return CreatePortData(Node.OutputPortName, PortDirection.Output, int.MaxValue);
            else if (behaviorNodeType == BehaviorNodeType.Decorate) yield return CreatePortData(Node.OutputPortName, PortDirection.Output, 1);

            PortData CreatePortData(string portName, PortDirection direction, int capacity)
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
        }
    }
}