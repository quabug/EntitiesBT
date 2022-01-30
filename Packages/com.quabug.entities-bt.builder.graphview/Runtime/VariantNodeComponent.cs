using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesBT.Core;
using EntitiesBT.Variant;
using GraphExt;
using Nuwa;
using UnityEngine;
using Nuwa.Editor;

#if UNITY_EDITOR
using EntitiesBT.Editor;
using UnityEditor;
using GraphExt.Editor;
using UnityEditor.Experimental.SceneManagement;
#endif

namespace EntitiesBT
{
    [DisallowMultipleComponent, ExecuteAlways, AddComponentMenu("")]
    public class VariantNodeComponent : MonoBehaviour, INodeComponent<IGraphNode, VariantNodeComponent>
    {
        [SerializeField, Nuwa.ReadOnly, UnityDrawProperty] private string _id;
        public NodeId Id { get => Guid.Parse(_id); set => _id = value.ToString(); }

        [SerializeField] private Vector2 _position;
        public Vector2 Position { get => _position; set => _position = value; }

        [SerializeField, UnboxSingleProperty, UnityDrawProperty] private VariantNode _node;
        public IGraphNode Node { get => _node; set => _node = value as VariantNode; }

        public INodeComponent.NodeComponentConnect OnNodeComponentConnect { get; set; }
        public INodeComponent.NodeComponentDisconnect OnNodeComponentDisconnect { get; set; }

        private readonly GraphExt.HashSet<EdgeId> _edges = new GraphExt.HashSet<EdgeId>();
        private readonly Action<Transform> _reorderChildrenTransform = NodeTransform.ReorderChildrenTransformAction(node => node.Position.x);

        public IReadOnlySet<EdgeId> GetEdges(GraphRuntime<IGraphNode> graph)
        {
            _edges.Clear();
            return _edges;
        }

        public bool IsPortCompatible(GameObjectNodes<IGraphNode, VariantNodeComponent> data, in PortId input, in PortId output)
        {
            if (input.NodeId == Id && output.NodeId == Id) return false; // same node
            if (input.NodeId == Id) return true; // only check compatible on output/start node
            return true;
            // cannot connect to input/end node which is parent of output/start node to avoid circle dependency
        }

        public void OnConnected(GameObjectNodes<IGraphNode, VariantNodeComponent> data, in EdgeId edge)
        {
            if (_edges.Contains(edge)) return;
            _edges.Add(edge);
        }

        public void OnDisconnected(GameObjectNodes<IGraphNode, VariantNodeComponent> data, in EdgeId edge)
        {
            if (!_edges.Contains(edge)) return;
            // reset parent for tree edges
            _edges.Remove(edge);
        }

        private void OnTransformChildrenChanged()
        {
            _reorderChildrenTransform(transform);
        }

#if UNITY_EDITOR
        private SerializedProperty[] _variantProperties;
        private readonly EventTitleProperty _titleProperty = new EventTitleProperty();

        static VariantNodeComponent()
        {
            EditorApplication.hierarchyChanged -= RefreshTitles;
            EditorApplication.hierarchyChanged += RefreshTitles;

            void RefreshTitles()
            {
                var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
                if (prefabStage == null) return;
                foreach (var node in prefabStage.prefabContentsRoot.GetComponentsInChildren<VariantNodeComponent>())
                    node._titleProperty.Title = node.name;
            }
        }

        // public NodeData FindNodeProperties(SerializedObject nodeObject)
        // {
        //     var behaviorNodeType = Node.BehaviorNodeType;
        //     _titleProperty.Title = name;
        //     var properties = new List<INodeProperty>
        //     {
        //         CreateVerticalPorts(Node.InputPortName),
        //         new NodeSerializedPositionProperty { PositionProperty = nodeObject.FindProperty(nameof(_position)) },
        //         new NodeClassesProperty(behaviorNodeType.ToString().ToLower().Yield()),
        //         _titleProperty,
        //         new BehaviorBlobDataProperty(GetSerializedNodeBuilder(nodeObject))
        //     };
        //     if (behaviorNodeType != BehaviorNodeType.Action) properties.Add(CreateVerticalPorts(Node.OutputPortName));
        //     return new NodeData(properties);
        //
        //     VerticalPortsProperty CreateVerticalPorts(string portName)
        //     {
        //         var verticalPorts = new VerticalPortsProperty { Name = portName };
        //         var portContainer = new PortContainerProperty(portName);
        //         verticalPorts.Ports.Add(portContainer);
        //         return verticalPorts;
        //     }
        // }
        //
        // public IEnumerable<PortData> FindNodePorts(SerializedObject nodeObject)
        // {
        //     var behaviorNodeType = Node.BehaviorNodeType;
        //     yield return CreateBehaviorTreePortData(Node.InputPortName, PortDirection.Input, 1);
        //
        //     if (behaviorNodeType == BehaviorNodeType.Composite)
        //         yield return CreateBehaviorTreePortData(Node.OutputPortName, PortDirection.Output, int.MaxValue);
        //     else if (behaviorNodeType == BehaviorNodeType.Decorate)
        //         yield return CreateBehaviorTreePortData(Node.OutputPortName, PortDirection.Output, 1);
        //
        //     _variantProperties ??= GetVariantProperties(nodeObject).ToArray();
        //     foreach (var variant in _variantProperties)
        //     {
        //         var variantType = variant.GetManagedFullType();
        //         if (variantType != null && typeof(GraphNodeVariant.Any).IsAssignableFrom(variantType))
        //         {
        //             yield return CreateVariantPortData(variant, variantType, PortDirection.Input);
        //             yield return CreateVariantPortData(variant, variantType, PortDirection.Output);
        //         }
        //     }
        //
        //     PortData CreateBehaviorTreePortData(string portName, PortDirection direction, int capacity)
        //     {
        //         return new PortData(
        //             portName,
        //             PortOrientation.Vertical,
        //             direction,
        //             capacity,
        //             typeof(VariantNode),
        //             new []{"tree", behaviorNodeType.ToString().ToLower()}
        //         );
        //     }
        //
        //     PortData CreateVariantPortData(SerializedProperty property, Type variantType, PortDirection direction)
        //     {
        //         return new PortData(
        //             VariantPort.CreatePortName(property, direction),
        //             PortOrientation.Horizontal,
        //             direction,
        //             1,
        //             VariantPort.GetPortType(variantType),
        //             new []{"variant"}
        //         );
        //     }
        // }
        //
        // private IEnumerable<SerializedProperty> GetVariantProperties(SerializedObject nodeObject)
        // {
        //     var serializedNodeBuilder = GetSerializedNodeBuilder(nodeObject);
        //     while (serializedNodeBuilder.NextVisible(true))
        //     {
        //         var fieldType = serializedNodeBuilder.GetManagedFieldType();
        //         if (fieldType != null && typeof(IVariant).IsAssignableFrom(fieldType))
        //             yield return serializedNodeBuilder.Copy();
        //     }
        // }
        //
        // private SerializedProperty GetSerializedNodeBuilder(SerializedObject nodeObject)
        // {
        //     return nodeObject.FindProperty(nameof(_node)).FindPropertyRelative(nameof(VariantNode.Blob));
        // }
#endif

    }
}
