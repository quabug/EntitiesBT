using System;
using System.Collections.Generic;
using EntitiesBT.Components;
using EntitiesBT.Core;
using GraphExt;
using Nuwa;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using UnityEngine;

#if UNITY_EDITOR
using GraphExt.Editor;
using UnityEditor;
using EntitiesBT.Editor;
#endif

namespace EntitiesBT
{
    [ExecuteAlways]
    public class BehaviorNodeComponent : GraphNodeComponent, ITreeNodeComponent, INodeDataBuilder
    {
#region GraphNode
        public override Vector2 Position
        {
            get => _Position;
            set
            {
                _Position = value;
                var parent = transform.parent.GetComponent<BehaviorNodeComponent>();
                if (parent) parent._reorderChildrenTransform(parent.transform);
#if UNITY_EDITOR
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
#endif
            }
        }

        [SerializeField, UnboxSingleProperty, UnityDrawProperty] private BehaviorNode _node;
        public override GraphNode Node { get => _node; set => _node = (BehaviorNode)value; }

        [SerializeField] private bool _expanded = false;

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
            var isInputTreePort = data.Runtime.IsTreePort(input);
            var isOutputTreePort = data.Runtime.IsTreePort(output);
            if (!isInputTreePort && !isOutputTreePort)
            {
                var isInputBehaviorNode = data[input.NodeId] is BehaviorNodeComponent;
                var isOutputBehaviorNode = data[output.NodeId] is BehaviorNodeComponent;
                return !isInputBehaviorNode || !isOutputBehaviorNode;
            }

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
        private readonly VariantPorts _variantPorts = new VariantPorts();
        private readonly NodeTitleProperty _titleProperty = new NodeTitleProperty();
        private readonly ToggleDisplayProperty _typeTitleController = new ToggleDisplayProperty();


        static BehaviorNodeComponent()
        {
            GraphUtility.RegisterNameChanged<BehaviorNodeComponent>(node => node.UpdateNodeTitle());
        }

        private void UpdateNodeTitle()
        {
            _titleProperty.Title = name;
            _typeTitleController.IsHidden = name == _node.Name;
        }

        public override NodeData FindNodeProperties(SerializedObject nodeObject)
        {
            _typeTitleController.InnerProperty = new LabelProperty(_node.Name, "default-node-name");
            UpdateNodeTitle();
            _titleProperty.ToggleProperty = nodeObject.FindProperty(nameof(_expanded));
            var behaviorNodeType = _node.BehaviorNodeType;
            var properties = new List<INodeProperty>
            {
                CreateVerticalPorts(_node.InputPortName, -100),
                new NodeSerializedPositionProperty { PositionProperty = nodeObject.FindProperty(nameof(_Position)) },
                new NodeClassesProperty("behavior-node", behaviorNodeType.ToString().ToLower()),
                _typeTitleController,
                _titleProperty,
                new NodeSerializedProperty(GetSerializedNodeBuilder(nodeObject)) { HideFoldoutToggle = true, ToggleProperty = _titleProperty.ToggleProperty }
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

            foreach (var variantPort in _variantPorts.FindNodePorts(GetSerializedNodeBuilder(nodeObject))) yield return variantPort;

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
        }

        private SerializedProperty GetSerializedNodeBuilder(SerializedObject nodeObject)
        {
            return nodeObject.FindProperty(nameof(_node)).FindPropertyRelative(nameof(BehaviorNode.Blob));
        }
#endif
#endregion

#region NodeDataBuilder

        public int NodeId => _node.BehaviorNodeAttribute.Id;
        public int NodeIndex { get; set; }
        public INodeDataBuilder Self => this;
        public IEnumerable<INodeDataBuilder> Children => Components.Utilities.Children(this);
        public object GetPreviewValue(string path) => throw new NotSupportedException();
        public void SetPreviewValue(string path, object value) => throw new NotSupportedException();
        public unsafe BlobAssetReference Build(Core.ITreeNode<INodeDataBuilder>[] builders)
        {
            var nodeType = _node.BehaviorNodeDataType;
            if (nodeType.IsZeroSizeStruct()) return BlobAssetReference.Null;
            var blobBuilder = new BlobBuilder(Allocator.Temp, UnsafeUtility.SizeOf(nodeType));
            try
            {
                var dataPtr = blobBuilder.ConstructRootPtrByType(nodeType);
                _node.Blob.Build(blobBuilder, new IntPtr(dataPtr));
                return blobBuilder.CreateReferenceByType(nodeType);
            }
            finally
            {
                blobBuilder.Dispose();
            }
        }

#endregion
    }
}