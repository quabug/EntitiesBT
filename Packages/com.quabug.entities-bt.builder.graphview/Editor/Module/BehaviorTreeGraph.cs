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
        [NotNull] private readonly GameObject _prefab;
        [NotNull] private readonly PrefabStage _stage;
        private string PrefabPath => AssetDatabase.GetAssetPath(_prefab);
        public string Name => _prefab.name;
        private GameObject RootInstance => _stage.prefabContentsRoot;

        public BehaviorTreeGraph([NotNull] GameObject prefab, PrefabStage prefabStage)
        {
            _prefab = prefab;
            _stage = prefabStage;
            ResetBehaviorNodes();
            ResetSyntaxNodes();

            Selection.selectionChanged += OnSelectionChanged;
            EditorApplication.update += OnUpdate;
        }

        public void Dispose()
        {
            EditorApplication.update -= OnUpdate;
            Selection.selectionChanged -= OnSelectionChanged;
        }

        void OnUpdate()
        {

        }

        void OnSelectionChanged()
        {
            var selectedInstance = Selection.activeGameObject;
            if (selectedInstance != null && _behaviorNodes.Value.TryGetValue(selectedInstance.GetInstanceID(), out var node))
                node.EmitOnSelected();
        }

        private void SavePrefab()
        {
            EditorSceneManager.MarkSceneDirty(_stage.scene);
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

        internal void SetPosition([NotNull] GameObject node, Vector2 position, Func<GameObject, float> order)
        {
            node.transform.localPosition = position;
            var parent = node.Parent();
            Assert.IsNotNull(parent);
            ReorderChildrenByPosition(parent, order);
            SavePrefab();
        }

        internal void SetParent([NotNull] GameObject child, [CanBeNull] GameObject parent, Func<GameObject, float> order)
        {
            parent ??= RootInstance;
            var position = child.transform.localPosition;
            child.transform.SetParent(parent.transform);
            child.transform.localPosition = position;
            ReorderChildrenByPosition(parent, order);
            SavePrefab();
        }

        private void ReorderChildrenByPosition([NotNull] GameObject parentInstance, Func<GameObject, float> order)
        {
            var childrenInstance = parentInstance.Children().ToArray();
            var positionOrderedIndices = childrenInstance
                .Select((child, index) => (child, index))
                .OrderBy(t => order(t.child))
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

        internal bool IsSelected(GameObject node) => Selection.activeGameObject == node;

        internal void Select(GameObject node)
        {
            if (Selection.activeGameObject != node) Selection.activeGameObject = node;
        }

        internal void Unselect(GameObject node)
        {
            if (Selection.activeGameObject == node) Selection.activeGameObject = RootInstance;
        }

        internal void RemoveNode(GameObject node)
        {
            _behaviorNodes.Value.Remove(node.GetInstanceID());
            _syntaxNodes.Value.Remove(node.GetInstanceID());
            foreach (var child in node.Children().Reverse()) child.transform.SetParent(RootInstance.transform);
            GameObject.DestroyImmediate(node);
            SavePrefab();
        }

#region BehaviorTree

        private Lazy<IDictionary<int, BehaviorTreeNode>> _behaviorNodes;

        internal BehaviorTreeNode GetBehaviorNodeById(int id) => _behaviorNodes.Value[id];
        private BehaviorTreeNode GetBehaviorNodeByGameObject(GameObject obj) => GetBehaviorNodeById(obj.GetInstanceID());

        void ResetBehaviorNodes()
        {
            _behaviorNodes = new Lazy<IDictionary<int, BehaviorTreeNode>>(() => RootInstance.Descendants()
                .Where(descendant => descendant.GetComponent<INodeDataBuilder>() != null)
                .ToDictionary(descendant => descendant.GetInstanceID(), descendant => new BehaviorTreeNode(this, descendant))
            );
        }

        public IBehaviorTreeNode AddBehaviorNode(Type nodeType, Vector2 position)
        {
            var createdNode = CreateNodeObject();
            SavePrefab();
            return createdNode;

            BehaviorTreeNode CreateNodeObject()
            {
                var nodeObj = new GameObject();
                var dynamicNode = nodeObj.AddComponent<GraphViewNode>();
                var node = new BehaviorTreeNode(this, nodeObj);
                _behaviorNodes.Value.Add(nodeObj.GetInstanceID(), node);
                nodeObj.transform.SetParent(RootInstance.transform);
                nodeObj.transform.position = position;
                dynamicNode.NodeData = new NodeAsset { NodeType = nodeType.AssemblyQualifiedName };
                nodeObj.name = nodeType.Name;
                return node;
            }
        }

        public IEnumerable<IBehaviorTreeNode> BehaviorTreeRootNodes => GetChildrenBehaviorNodes(RootInstance);

        internal IEnumerable<IBehaviorTreeNode> GetChildrenBehaviorNodes(GameObject instance) =>
            from child in instance.Children()
            where IsBehaviorNodeGameObject(child)
            select GetBehaviorNodeByGameObject(child)
        ;

        internal IEnumerable<IBehaviorTreeNode> GetChildrenBehaviorNodes(int nodeId) =>
            GetChildrenBehaviorNodes(_behaviorNodes.Value[nodeId].Instance);

        private bool IsBehaviorNodeGameObject(GameObject obj) => obj.GetComponent<INodeDataBuilder>() != null;

#endregion

#region SyntaxTree

        private Lazy<IDictionary<int, SyntaxTreeNode>> _syntaxNodes;

        void ResetSyntaxNodes()
        {
            _syntaxNodes = new Lazy<IDictionary<int, SyntaxTreeNode>>(() => RootInstance.Descendants()
                .Where(descendant => descendant.GetComponent<GraphVariantNode>() != null)
                .ToDictionary(descendant => descendant.GetInstanceID(), descendant => new SyntaxTreeNode(this, descendant))
            );
        }

        public ISyntaxTreeNode AddSyntaxNode(Type variantBaseType, Vector2 position)
        {
            var createdNode = CreateNodeObject();
            SavePrefab();
            return createdNode;

            SyntaxTreeNode CreateNodeObject()
            {
                var variantObj = new GameObject();
                var variantNode = variantObj.AddComponent<GraphVariantNode>();
                variantNode.VariantClass = variantBaseType.AssemblyQualifiedName;
                var node = new SyntaxTreeNode(this, variantObj);
                variantObj.transform.SetParent(RootInstance.transform);
                variantObj.transform.position = position;
                return node;
            }
        }

        public IEnumerable<ISyntaxTreeNode> SyntaxTreeRootNodes => GetChildrenSyntaxNodes(RootInstance);

        internal IEnumerable<ISyntaxTreeNode> GetChildrenSyntaxNodes(GameObject instance) =>
            from child in instance.Children()
            where IsSyntaxNodeGameObject(child)
            select GetSyntaxNodeByGameObject(child)
        ;

        private bool IsSyntaxNodeGameObject(GameObject obj) => obj.GetComponent<GraphVariantNode>() != null;

        internal SyntaxTreeNode GetSyntaxNodeById(int id) => _syntaxNodes.Value[id];
        private SyntaxTreeNode GetSyntaxNodeByGameObject(GameObject obj) => GetSyntaxNodeById(obj.GetInstanceID());

#endregion
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