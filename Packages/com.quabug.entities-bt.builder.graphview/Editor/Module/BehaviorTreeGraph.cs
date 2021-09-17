using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EntitiesBT.Components;
using EntitiesBT.Core;
using Nuwa;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
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
            public int Id => _graph._nodePrefabList.IndexOf(Prefab);
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
        }

        [SerializeField, Nuwa.ReadOnly, UnityDrawProperty] private int _rootNodeIndex;
        [SerializeField, Nuwa.ReadOnly, UnityDrawProperty] internal GameObject Prefab;
        [SerializeField, Nuwa.ReadOnly, UnityDrawProperty] private List<Vector2> _nodePositionList = new List<Vector2>();
        [SerializeField, Nuwa.ReadOnly, UnityDrawProperty] private List<GameObject> _nodePrefabList = new List<GameObject>();

        private string PrefabPath => AssetDatabase.GetAssetPath(Prefab);
        public string Name => name;
        private GameObject InstanceRoot => PrefabStageUtility.GetCurrentPrefabStage().prefabContentsRoot;

        private readonly Lazy<List<Node>> _nodes;

        public BehaviorTreeGraph()
        {
            _nodes = new Lazy<List<Node>>(() => _nodePrefabList.Select(prefab => new Node(this, prefab)).ToList());
        }

        IDictionary<GameObject, int> ToNodeMap(List<GameObject> nodes)
        {
            return nodes.Select((node, index) => (node, index)).ToDictionary(t => t.node, t => t.index);
        }

        public IBehaviorTreeNode AddNode(Type nodeType, Vector2 position)
        {
            CreateNodeObject();
            SavePrefab();
            return CreateAndAddNode();

            GameObject CreateNodeObject()
            {
                var node = new GameObject();
                node.transform.SetParent(InstanceRoot.transform);
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

        private void SavePrefab()
        {
            // force save the prefab asset to be able to fetch saved GameObject from it immediately
            PrefabUtility.SaveAsPrefabAsset(InstanceRoot, PrefabPath);
            // EditorSceneManager.MarkSceneDirty(prefabStage.scene);
        }

        public IEnumerator<IBehaviorTreeNode> GetEnumerator()
        {
            EnsureNodeList();
            return _nodes.Value.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Select(int id)
        {
            var prefab = _nodePrefabList[id];
            Select(prefab);
        }

        private void Select(GameObject prefab)
        {
            var instance = FindCorrespondingInstance(prefab);
            Selection.activeObject = instance;
        }

        private Node AddNode(GameObject prefab, Vector2 position)
        {
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
            _nodePrefabList.RemoveAt(index);
            _nodePositionList.RemoveAt(index);
            _nodes.Value.RemoveAt(index);

            EditorUtility.SetDirty(this);

            if (prefab != null) DestroyImmediate(FindCorrespondingInstance(prefab));
            SavePrefab();
        }

        GameObject FindCorrespondingInstance(GameObject prefab)
        {
            var indices = prefab.FindHierarchyIndices();
            return InstanceRoot.FindGameObjectByHierarchyIndices(indices.Skip(1) /* skip root */);
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