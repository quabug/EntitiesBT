using GraphExt;
using JetBrains.Annotations;
using UnityEngine;

namespace EntitiesBT
{
    public class GraphGameObjectNodes : GameObjectNodes<GraphNode, GraphNodeComponent>
    {
        public GraphGameObjectNodes([NotNull] GameObject root) : base(root) {}

        protected override void OnNodeAdded(in NodeId id, GraphNode node)
        {
            if (!_NodeObjectMap.ContainsKey(id))
            {
                var nodeObject = new GameObject(node.GetType().Name);
                nodeObject.transform.SetParent(Root.transform);
                var nodeComponent = (GraphNodeComponent)nodeObject.AddComponent(node.ComponentType);
                nodeComponent.Id = id;
                nodeComponent.Node = node;
                AddNode(nodeComponent);
                SavePrefab();
            }
        }
    }}
