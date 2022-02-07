using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesBT.Editor;
using EntitiesBT.Variant;
using GraphExt;
using Nuwa;
using Nuwa.Editor;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using GraphExt.Editor;
#endif

namespace EntitiesBT
{
    [DisallowMultipleComponent, ExecuteAlways, AddComponentMenu("")]
    public class VariantNodeComponent : MonoBehaviour, INodeComponent<VariantNode, VariantNodeComponent>
    {
        [SerializeField] private bool _hideTitle = false;

        [SerializeField, Nuwa.ReadOnly, UnityDrawProperty] private string _id;
        public NodeId Id { get => Guid.Parse(_id); set => _id = value.ToString(); }

        [SerializeField] private Vector2 _position;
        public Vector2 Position { get => _position; set => _position = value; }

        [SerializeReference, UnboxSingleProperty, UnityDrawProperty] private VariantNode _node;
        public VariantNode Node { get => _node; set => _node = value; }

        public INodeComponent.NodeComponentConnect OnNodeComponentConnect { get; set; }
        public INodeComponent.NodeComponentDisconnect OnNodeComponentDisconnect { get; set; }

        private readonly GraphExt.HashSet<EdgeId> _edges = new GraphExt.HashSet<EdgeId>();
        private readonly Action<Transform> _reorderChildrenTransform = NodeTransform.ReorderChildrenTransformAction(node => node.Position.x);

        public IReadOnlySet<EdgeId> GetEdges(GraphRuntime<VariantNode> graph)
        {
            _edges.Clear();
            return _edges;
        }

        public bool IsPortCompatible(GameObjectNodes<VariantNode, VariantNodeComponent> data, in PortId input, in PortId output)
        {
            if (input.NodeId == Id && output.NodeId == Id) return false; // same node
            if (input.NodeId == Id) return true; // only check compatible on output/start node
            return true;
            // cannot connect to input/end node which is parent of output/start node to avoid circle dependency
        }

        public void OnConnected(GameObjectNodes<VariantNode, VariantNodeComponent> data, in EdgeId edge)
        {
            if (_edges.Contains(edge)) return;
            _edges.Add(edge);
        }

        public void OnDisconnected(GameObjectNodes<VariantNode, VariantNodeComponent> data, in EdgeId edge)
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
            GraphUtility.RegisterNameChanged<VariantNodeComponent>(variant =>
                variant._titleProperty.Title = variant._hideTitle ? null : variant.name
            );
        }

        public NodeData FindNodeProperties(SerializedObject nodeObject)
        {
            _titleProperty.Title = name;
            var properties = new List<INodeProperty>
            {
                new NodeSerializedPositionProperty { PositionProperty = nodeObject.FindProperty(nameof(_position)) },
                _titleProperty,
            };
            return new NodeData(properties);
        }

        public IEnumerable<PortData> FindNodePorts(SerializedObject nodeObject)
        {
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
