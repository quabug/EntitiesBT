using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesBT.Components;
using EntitiesBT.Core;
using Nuwa;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace EntitiesBT.Editor
{
    [CreateAssetMenu(fileName = "BehaviorTreeGraph", menuName = "EntitiesBT/BehaviorTreeGraph", order = 0)]
    public class BehaviorTreeGraph : ScriptableObject, IBehaviorTreeGraph
    {
        [Serializable]
        private class Node : IBehaviorTreeNode
        {
            private readonly BehaviorTreeGraph _graph;

            public GameObject Prefab { get; }

            // TODO: optimize?
            public int Id => _graph._objectIdMap.Value[Prefab];
            public string Name => Prefab.name;
            public Vector2 Position
            {
                get => _graph._nodePositionList[Id];
                set
                {
                    _graph._nodePositionList[Id] = value;
                    EditorUtility.SetDirty(_graph);
                }
            }

            public BehaviorNodeType BehaviorType => Prefab.GetComponent<BTDynamicNode>().BehaviorNodeType;
            public Type NodeType => Type.GetType(Prefab.GetComponent<BTDynamicNode>().NodeData.NodeType);

            public Node(BehaviorTreeGraph graph, GameObject prefab)
            {
                _graph = graph;
                Prefab = prefab;
            }

            public void Dispose()
            {
                _graph.RemoveNodeAt(Id);
            }

            public void OnSelected()
            {
                _graph.Select(Prefab);
            }

            public void OnUnselected()
            {
                _graph.Unselect(Prefab);
            }

            public void SetParent(IBehaviorTreeNode node)
            {
                var childInstance = _graph.FindCorrespondingInstance(Prefab);
                var parentInstance = node == null ? _graph.RootInstance : _graph.FindCorrespondingInstance(_graph._nodePrefabList[node.Id]);
                childInstance.transform.SetParent(parentInstance.transform);
                _graph.SavePrefab();
            }

            public IEnumerable<IBehaviorTreeNode> Children => _graph.GetChildrenNodes(Prefab);
        }

        [SerializeField, Nuwa.ReadOnly, UnityDrawProperty] internal GameObject Prefab;
        [SerializeField, Nuwa.ReadOnly, UnityDrawProperty] private List<Vector2> _nodePositionList = new List<Vector2>();
        [SerializeField, Nuwa.ReadOnly, UnityDrawProperty] private List<GameObject> _nodePrefabList = new List<GameObject>();

        private string PrefabPath => AssetDatabase.GetAssetPath(Prefab);
        public string Name => name;
        private GameObject RootInstance => PrefabStageUtility.GetCurrentPrefabStage().prefabContentsRoot;

        private Lazy<List<Node>> _nodes;
        private Lazy<IDictionary<GameObject, int>> _objectIdMap;

        public BehaviorTreeGraph()
        {
            ResetNodes();
            ResetObjectIdMap();
        }

        void ResetNodes()
        {
            _nodes = new Lazy<List<Node>>(() => _nodePrefabList.Select(prefab => new Node(this, prefab)).ToList());
        }

        void ResetObjectIdMap()
        {
            _objectIdMap = new Lazy<IDictionary<GameObject, int>>(() => ToNodeMap(_nodePrefabList));
        }

        public void RecreateData()
        {
            ResetNodes();
            ResetObjectIdMap();
            EnsureNodeList();
        }

        IDictionary<GameObject, int> ToNodeMap(List<GameObject> nodes)
        {
            return nodes.Select((node, index) => (node, index)).ToDictionary(t => t.node, t => t.index);
        }

        public IBehaviorTreeNode AddNode(Type nodeType, Vector2 position)
        {
            CreateNodeObject();
            ForceSavePrefab();
            return CreateAndAddNode();

            GameObject CreateNodeObject()
            {
                var node = new GameObject();
                node.transform.SetParent(RootInstance.transform);
                var dynamicNode = node.AddComponent<BTDynamicNode>();
                dynamicNode.NodeData = new NodeAsset { NodeType = nodeType.AssemblyQualifiedName };
                node.name = nodeType.Name;
                return node;
            }

            Node CreateAndAddNode()
            {
                var addedObject = Prefab.transform.GetChild(Prefab.transform.childCount - 1).gameObject;
                return AddNode(addedObject, position);
            }
        }

        private void ForceSavePrefab()
        {
            // force save the prefab asset to be able to fetch saved GameObject from it immediately
            PrefabUtility.SaveAsPrefabAsset(RootInstance, PrefabPath);
        }

        private void SavePrefab()
        {
            EditorSceneManager.MarkSceneDirty(PrefabStageUtility.GetCurrentPrefabStage().scene);
        }

        public IEnumerable<IBehaviorTreeNode> RootNodes => GetChildrenNodes(Prefab);

        private IEnumerable<IBehaviorTreeNode> GetChildrenNodes(GameObject root)
        {
            foreach (var child in root.Children())
            {
                if (_objectIdMap.Value.TryGetValue(child, out var id))
                    yield return _nodes.Value[id];
            }
        }

        private void Select(GameObject prefab)
        {
            var instance = FindCorrespondingInstance(prefab);
            Selection.activeObject = instance;
        }

        private void Unselect(GameObject prefab)
        {
            var instance = FindCorrespondingInstance(prefab);
            if (Selection.activeObject == instance) Selection.activeObject = this;
        }

        private Node AddNode(GameObject prefab, Vector2 position)
        {
            _objectIdMap.Value.Add(prefab, _nodePrefabList.Count);
            _nodePrefabList.Add(prefab);
            _nodePositionList.Add(position);
            var node = new Node(this, prefab);
            _nodes.Value.Add(node);

            EditorUtility.SetDirty(this);

            return node;
        }

        private void RemoveNodeAt(int index)
        {
            var prefab = _nodePrefabList[index];
            _objectIdMap.Value.Remove(prefab);
            _nodePrefabList.RemoveAt(index);
            _nodePositionList.RemoveAt(index);
            _nodes.Value.RemoveAt(index);

            ResetNodes();
            ResetObjectIdMap();

            EditorUtility.SetDirty(this);

            if (prefab != null)
            {
                var instance = FindCorrespondingInstance(prefab);
                foreach (var child in instance.Children().Reverse()) child.transform.SetParent(RootInstance.transform);
                DestroyImmediate(instance);
            }
            SavePrefab();
        }

        GameObject FindCorrespondingInstance(GameObject prefab)
        {
            var indices = prefab.FindHierarchyIndices();
            return RootInstance.FindGameObjectByHierarchyIndices(indices.Skip(1) /* skip root */);
        }

        GameObject FindCorrespondingPrefab(GameObject instance)
        {
            var indices = instance.FindHierarchyIndices();
            return Prefab.FindGameObjectByHierarchyIndices(indices.Skip(1) /* skip root */);
        }

        private void RemoveMultipleNodes(IEnumerable<int> indices)
        {
            foreach (var index in indices.OrderByDescending(i => i)) RemoveNodeAt(index);
        }

        private void EnsureNodeList()
        {
            var nodeMap = ToNodeMap(_nodePrefabList);
            foreach (var descendant in Prefab.Descendants())
            {
                if (!nodeMap.ContainsKey(descendant) && descendant.GetComponent<INodeDataBuilder>() != null)
                    AddNode(descendant, Vector2.zero);
                nodeMap.Remove(descendant);
            }
            RemoveMultipleNodes(nodeMap.Values);
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
            return indices.Aggregate(root.transform, (current, index) => current.GetChild(index)).gameObject;
        }
    }
}