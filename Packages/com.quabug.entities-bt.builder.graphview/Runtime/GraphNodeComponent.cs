using System;
using System.Collections.Generic;
using System.Linq;
using GraphExt;
using Nuwa;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using GraphExt.Editor;
using UnityEditor.Experimental.SceneManagement;
#endif

namespace EntitiesBT
{
    [AddComponentMenu("")]
    public class GraphNodeComponent : MonoBehaviour, INodeComponent<IGraphNode, GraphNodeComponent>
    {
        [SerializeField, ReadOnly, UnityDrawProperty] private string _id;
        public NodeId Id { get => Guid.Parse(_id); set => _id = value.ToString(); }

        [SerializeField] private Vector2 _position;
        public Vector2 Position { get; set; }

        public INodeComponent.NodeComponentConnect OnNodeComponentConnect { get; set; }
        public INodeComponent.NodeComponentDisconnect OnNodeComponentDisconnect { get; set; }

        [field: SerializeReference]
        public IGraphNode Node { get; set; }

        public IReadOnlySet<EdgeId> GetEdges(GraphRuntime<IGraphNode> graph)
        {
            return Node.GetEdges(graph);
        }

        public bool IsPortCompatible(
            GameObjectNodes<IGraphNode, GraphNodeComponent> data,
            in PortId input,
            in PortId output
        )
        {
            return Node.IsPortCompatible(data, input: input, output: output);
        }

        public void OnConnected(GameObjectNodes<IGraphNode, GraphNodeComponent> data, in EdgeId edge)
        {
            Node.OnConnected(data, edge);
        }

        public void OnDisconnected(GameObjectNodes<IGraphNode, GraphNodeComponent> data, in EdgeId edge)
        {
            Node.OnDisconnected(data, edge);
        }

#if UNITY_EDITOR
        private SerializedProperty[] _variantProperties;
        private readonly EventTitleProperty _titleProperty = new EventTitleProperty();

        static GraphNodeComponent()
        {
            EditorApplication.hierarchyChanged -= RefreshTitles;
            EditorApplication.hierarchyChanged += RefreshTitles;

            void RefreshTitles()
            {
                var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
                if (prefabStage == null) return;
                foreach (var node in prefabStage.prefabContentsRoot.GetComponentsInChildren<GraphNodeComponent>())
                    node._titleProperty.Title = node.name;
            }
        }

        public NodeData FindNodeProperties(SerializedObject nodeObject)
        {
            _titleProperty.Title = name;
            var properties = new List<INodeProperty>
            {
                new NodeSerializedPositionProperty { PositionProperty = nodeObject.FindProperty(nameof(_position)) },
                _titleProperty
            };
            return new NodeData(properties);
        }

        public IEnumerable<PortData> FindNodePorts(SerializedObject nodeObject)
        {
            return Enumerable.Empty<PortData>();
            // _variantProperties ??= GetVariantProperties(nodeObject).ToArray();
            // foreach (var variant in _variantProperties)
            // {
            //     var variantType = variant.GetManagedFullType();
            //     if (variantType != null && typeof(GraphNodeVariant.Any).IsAssignableFrom(variantType))
            //     {
            //         yield return CreateVariantPortData(variant, variantType, PortDirection.Input);
            //         yield return CreateVariantPortData(variant, variantType, PortDirection.Output);
            //     }
            // }
            //
            // PortData CreateVariantPortData(SerializedProperty property, Type variantType, PortDirection direction)
            // {
            //     return new PortData(
            //         VariantPort.CreatePortName(property, direction),
            //         PortOrientation.Horizontal,
            //         direction,
            //         1,
            //         VariantPort.GetPortType(variantType),
            //         new []{"variant"}
            //     );
            // }
        }
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
        //     return nodeObject.FindProperty(nameof(_node)).FindPropertyRelative(nameof(BehaviorTreeNode.Blob));
        // }
#endif
    }
}