using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesBT.Components;
using EntitiesBT.Core;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Assertions;

namespace EntitiesBT.Editor
{
    public class BehaviorTreeGraph : IBehaviorTreeGraph
    {
        [Serializable]
        private class Node : IBehaviorTreeNode
        {
            private readonly BehaviorTreeGraph _graph;

            public GameObject Instance { get; }
            public int Id => Instance.GetInstanceID();
            public string Name => Instance.name;
            public Vector2 Position
            {
                get => Instance.transform.localPosition;
                set => _graph.SetPosition(this, value);
            }

            public BehaviorNodeType BehaviorType => Instance.GetComponent<INodeDataBuilder>().GetBehaviorNodeType();
            public Type NodeType => Instance.GetComponent<INodeDataBuilder>().GetNodeType();

            public bool IsSelected
            {
                get => _graph.IsSelected(this);
                set
                {
                    if (value) _graph.Select(this);
                    else _graph.Unselect(this);
                }
            }

            public Node(BehaviorTreeGraph graph, GameObject instance)
            {
                _graph = graph;
                Instance = instance;
            }

            public void Dispose()
            {
                _graph.RemoveNode(Id);
            }

            public void SetParent(IBehaviorTreeNode parent)
            {
                _graph.SetParent(child: this, parent: parent);
            }

            public IEnumerable<IBehaviorTreeNode> Children => _graph.GetChildrenNodes(Id);

            public event Action OnSelected;

            public void EmitOnSelected() => OnSelected?.Invoke();
        }

        [NotNull] private readonly GameObject _prefab;
        [NotNull] private readonly PrefabStage _stage;
        private string PrefabPath => AssetDatabase.GetAssetPath(_prefab);
        public string Name => _prefab.name;
        private GameObject RootInstance => _stage.prefabContentsRoot;

        private Lazy<IDictionary<int, Node>> _nodes;

        public BehaviorTreeGraph([NotNull] GameObject prefab, PrefabStage prefabStage)
        {
            _prefab = prefab;
            _stage = prefabStage;
            ResetNodes();

            Selection.selectionChanged += OnSelectionChanged;
        }

        public void Dispose()
        {
            Selection.selectionChanged -= OnSelectionChanged;
        }

        void OnSelectionChanged()
        {
            var selectedInstance = Selection.activeGameObject;
            if (selectedInstance != null && _nodes.Value.TryGetValue(selectedInstance.GetInstanceID(), out var node))
                node.EmitOnSelected();
        }

        private Node GetNodeById(int id) => _nodes.Value[id];
        private Node GetNodeByGameObject(GameObject obj) => GetNodeById(obj.GetInstanceID());

        void ResetNodes()
        {
            _nodes = new Lazy<IDictionary<int, Node>>(() => RootInstance.Descendants()
                .Where(descendant => descendant.GetComponent<INodeDataBuilder>() != null)
                .ToDictionary(descendant => descendant.GetInstanceID(), descendant => new Node(this, descendant))
            );
        }

        public IBehaviorTreeNode AddNode(Type nodeType, Vector2 position)
        {
            var createdNode = CreateNodeObject();
            SavePrefab();
            return createdNode;

            Node CreateNodeObject()
            {
                var nodeObj = new GameObject();
                var dynamicNode = nodeObj.AddComponent<GraphViewNode>();
                var node = new Node(this, nodeObj);
                _nodes.Value.Add(nodeObj.GetInstanceID(), node);
                nodeObj.transform.SetParent(RootInstance.transform);
                nodeObj.transform.position = position;
                dynamicNode.NodeData = new NodeAsset { NodeType = nodeType.AssemblyQualifiedName };
                nodeObj.name = nodeType.Name;
                return node;
            }
        }

        private void SavePrefab()
        {
            EditorSceneManager.MarkSceneDirty(_stage.scene);
        }

        private void SetPosition([NotNull] Node node, Vector2 position)
        {
            node.Instance.transform.localPosition = position;
            var parent = node.Instance.Parent();
            Assert.IsNotNull(parent);
            ReorderChildrenByPosition(parent);
            SavePrefab();
        }

        private void ReorderChildrenByPosition([NotNull] GameObject parentInstance)
        {
            var childrenInstance = parentInstance.Children().ToArray();
            var positionOrderedIndices = childrenInstance
                .Select((child, index) => (child, index))
                .OrderBy(t => t.child.transform.localPosition.x)
                .Select(t => t.index)
                .ToArray()
            ;

            if (!IsOrdered())
            {
                foreach (var index in positionOrderedIndices.Reverse()) childrenInstance[index].transform.SetAsFirstSibling();
                SavePrefab();
            }

            bool IsOrdered()
            {
                return positionOrderedIndices.Zip(positionOrderedIndices.Skip(1), (current, next) => current < next).All(isLess => isLess);
            }
        }

        private void SetParent([NotNull] Node child, [CanBeNull] IBehaviorTreeNode parent)
        {
            var childInstance = child.Instance;
            var parentInstance = parent == null ? RootInstance : GetNodeById(parent.Id).Instance;
            var position = childInstance.transform.localPosition;
            childInstance.transform.SetParent(parentInstance.transform);
            childInstance.transform.localPosition = position;
            ReorderChildrenByPosition(parentInstance);
            SavePrefab();
        }

        public IEnumerable<IBehaviorTreeNode> RootNodes => GetChildrenNodes(RootInstance);

        private IEnumerable<IBehaviorTreeNode> GetChildrenNodes(GameObject instance) =>
            from child in instance.Children()
            where IsNodeGameObject(child)
            select GetNodeByGameObject(child)
        ;

        private IEnumerable<IBehaviorTreeNode> GetChildrenNodes(int nodeId) =>
            GetChildrenNodes(_nodes.Value[nodeId].Instance);

        private bool IsNodeGameObject(GameObject obj) => obj.GetComponent<INodeDataBuilder>() != null;

        private bool IsSelected(Node node) => Selection.activeGameObject == node.Instance;

        private void Select(Node node)
        {
            if (Selection.activeGameObject != node.Instance) Selection.activeGameObject = node.Instance;
        }

        private void Unselect(Node node)
        {
            if (Selection.activeGameObject == node.Instance) Selection.activeGameObject = RootInstance;
        }

        private void RemoveNode(int id)
        {
            var node = _nodes.Value[id];
            _nodes.Value.Remove(id);
            var instance = node.Instance;
            foreach (var child in instance.Children().Reverse()) child.transform.SetParent(RootInstance.transform);
            GameObject.DestroyImmediate(instance);
            SavePrefab();
        }

        GameObject FindCorrespondingInstance(GameObject prefab)
        {
            var indices = prefab.FindHierarchyIndices();
            return RootInstance.FindGameObjectByHierarchyIndices(indices);
        }

        GameObject FindCorrespondingPrefab(GameObject instance)
        {
            var indices = instance.FindHierarchyIndices();
            return _prefab.FindGameObjectByHierarchyIndices(indices);
        }

        private void ReleaseUnmanagedResources()
        {
            // TODO release unmanaged resources here
        }
    }

    public static partial class Extension
    {
        public static IReadOnlyList<int> FindHierarchyIndices(this GameObject target)
        {
            return target.AncestorsAndSelf().Aggregate(new List<int>(), (indices, ancestor) =>
            {
                indices.Insert(0, ancestor.transform.GetSiblingIndex());
                return indices;
            });
        }

        public static GameObject FindGameObjectByHierarchyIndices(this GameObject root, IEnumerable<int> indices)
        {
            return indices.Skip(1).Aggregate(root.transform, (current, index) => current.GetChild(index)).gameObject;
        }
    }
}