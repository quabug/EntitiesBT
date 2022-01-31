using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EntitiesBT.Core;
using EntitiesBT.Editor;
using EntitiesBT.Variant;
using GraphExt;
using Nuwa.Blob;
using Nuwa.Editor;
using UnityEngine;

namespace EntitiesBT
{
    [Serializable]
    public sealed class BehaviorTreeNode : BaseGraphNode, GraphExt.ITreeNode<GraphRuntime<IGraphNode>>
    {
        public DynamicBlobDataBuilder Blob;

        public Type BehaviorNodeDataType => Type.GetType(Blob.BlobDataType);
        public BehaviorNodeAttribute BehaviorNodeAttribute => BehaviorNodeDataType.GetCustomAttribute<BehaviorNodeAttribute>();
        public BehaviorNodeType BehaviorNodeType => BehaviorNodeAttribute.Type;

        public string InputPortName => "input-port";
        public string OutputPortName => "output-port";

        private readonly TreeEdge _treeEdge = new TreeEdge();
        private readonly GraphExt.HashSet<EdgeId> _edges = new GraphExt.HashSet<EdgeId>();
        private readonly Action<Transform> _reorderChildrenTransform = NodeTransform.ReorderChildrenTransformAction(node => node.Position.x);

        public override IReadOnlySet<EdgeId> GetEdges(GraphRuntime<IGraphNode> graph)
        {
            _edges.Clear();
            var treeEdge = _treeEdge.Edge(GraphNodeComponent.gameObject);
            if (treeEdge.HasValue) _edges.Add(treeEdge.Value);
            return _edges;
        }

        public override bool IsPortCompatible(GameObjectNodes<IGraphNode, GraphNodeComponent> data, in PortId input, in PortId output)
        {
            // free to connect each other if they are not tree ports
            var isInputTreePort = data.Runtime.IsTreePort(input);
            var isOutputTreePort = data.Runtime.IsTreePort(output);
            if (!isInputTreePort && !isOutputTreePort) return true;

            if (input.NodeId == GraphNodeComponent.Id && output.NodeId == GraphNodeComponent.Id) return false; // same node
            if (input.NodeId == GraphNodeComponent.Id) return true; // only check compatible on output/start node
            // tree port must connect to another tree port
            if (!isInputTreePort || !isOutputTreePort) return false;
            // cannot connect to input/end node which is parent of output/start node to avoid circle dependency
            return !_treeEdge.IsParentInputPort(GraphNodeComponent.gameObject, input);
        }

        public override void OnConnected(GameObjectNodes<IGraphNode, GraphNodeComponent> data, in EdgeId edge)
        {
            if (_edges.Contains(edge)) return;
            _edges.Add(edge);
            // set parent for tree edges
            var parent = data[edge.Output.NodeId].transform;
            var inputPortId = new PortId(GraphNodeComponent.Id, InputPortName);
            if (edge.Input == inputPortId)
            {
                _treeEdge.SetParent(parent: parent, self: GraphNodeComponent.transform);
                _reorderChildrenTransform(parent);
            }
        }

        public override void OnDisconnected(GameObjectNodes<IGraphNode, GraphNodeComponent> data, in EdgeId edge)
        {
            if (!_edges.Contains(edge)) return;
            // reset parent for tree edges
            var inputPortId = new PortId(GraphNodeComponent.Id, InputPortName);
            if (edge.Input == inputPortId)
            {
                var parent = _treeEdge.FindStageRoot(GraphNodeComponent.transform);
                _treeEdge.SetParent(parent: parent, self: GraphNodeComponent.transform);
                _reorderChildrenTransform(parent);
            }
            _edges.Remove(edge);
        }

#if UNITY_EDITOR
        private UnityEditor.SerializedProperty[] _variantProperties;

        public override IEnumerable<PortData> FindNodePorts(UnityEditor.SerializedProperty thisProperty)
        {
            yield return CreateBehaviorTreePortData(InputPortName, PortDirection.Input, 1);

            if (BehaviorNodeType == BehaviorNodeType.Composite)
                yield return CreateBehaviorTreePortData(OutputPortName, PortDirection.Output, int.MaxValue);
            else if (BehaviorNodeType == BehaviorNodeType.Decorate)
                yield return CreateBehaviorTreePortData(OutputPortName, PortDirection.Output, 1);

            _variantProperties ??= GetVariantProperties(thisProperty).ToArray();
            foreach (var variant in _variantProperties)
            {
                var variantType = variant.GetManagedFullType();
                if (variantType != null && typeof(GraphNodeVariant.Any).IsAssignableFrom(variantType))
                {
                    yield return CreateVariantPortData(variant, variantType, PortDirection.Input);
                    yield return CreateVariantPortData(variant, variantType, PortDirection.Output);
                }
            }

            PortData CreateVariantPortData(UnityEditor.SerializedProperty property, Type variantType, PortDirection direction)
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

            PortData CreateBehaviorTreePortData(string portName, PortDirection direction, int capacity)
            {
                return new PortData(
                    portName,
                    PortOrientation.Vertical,
                    direction,
                    capacity,
                    typeof(BehaviorTreeNode),
                    new []{"tree", BehaviorNodeType.ToString().ToLower()}
                );
            }
        }

        public override IEnumerable<INodeProperty> CreateNodeProperties(UnityEditor.SerializedProperty thisProperty)
        {
            yield return CreateVerticalPorts(InputPortName, -10000);
            yield return new GraphExt.Editor.NodeClassesProperty(BehaviorNodeType.ToString().ToLower().Yield());
            yield return new BehaviorBlobDataProperty(thisProperty.FindPropertyRelative(nameof(Blob)));
            if (BehaviorNodeType != BehaviorNodeType.Action) yield return CreateVerticalPorts(OutputPortName, 10000);

            GraphExt.Editor.VerticalPortsProperty CreateVerticalPorts(string portName, int order)
            {
                var verticalPorts = new GraphExt.Editor.VerticalPortsProperty { Name = portName, Order = order};
                var portContainer = new GraphExt.Editor.PortContainerProperty(portName);
                verticalPorts.Ports.Add(portContainer);
                return verticalPorts;
            }
        }

        private IEnumerable<UnityEditor.SerializedProperty> GetVariantProperties(UnityEditor.SerializedProperty thisProperty)
        {
            var serializedNodeBuilder = thisProperty.FindPropertyRelative(nameof(Blob));
            while (serializedNodeBuilder.NextVisible(true))
            {
                var fieldType = serializedNodeBuilder.GetManagedFieldType();
                if (fieldType != null && typeof(IVariant).IsAssignableFrom(fieldType))
                    yield return serializedNodeBuilder.Copy();
            }
        }
#endif
    }
}