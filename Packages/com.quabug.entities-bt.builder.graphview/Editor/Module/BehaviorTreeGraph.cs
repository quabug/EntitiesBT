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
using UnityEngine.Assertions;

namespace EntitiesBT.Editor
{
    [CreateAssetMenu(fileName = "BehaviorTreeGraph", menuName = "EntitiesBT/BehaviorTreeGraph", order = 0)]
    public class BehaviorTreeGraph : ScriptableObject, IBehaviorTreeGraph, ISerializationCallbackReceiver
    {
        [Serializable]
        private class Node : IBehaviorTreeNode
        {
            private readonly BehaviorTreeGraph _graph;

            public Vector2 Position
            {
                get => _graph._nodePositionList[Id];
                set => _graph._nodePositionList[Id] = value;
            }
            public int Id { get; set; }
            public string Name => Reference.name;
            public GameObject Reference => _graph._nodeReferenceList[Id];

            public Node(BehaviorTreeGraph graph, int id)
            {
                _graph = graph;
                Id = id;
            }
        }

        private readonly List<Node> _nodes = new List<Node>();

        [SerializeField, Nuwa.ReadOnly, UnityDrawProperty] internal GameObject Prefab;
        [SerializeField, Nuwa.ReadOnly, UnityDrawProperty] private int _rootNodeIndex;
        [SerializeField, Nuwa.ReadOnly, UnityDrawProperty] private List<Vector2> _nodePositionList = new List<Vector2>();
        [SerializeField, Nuwa.ReadOnly, UnityDrawProperty] private List<GameObject> _nodeReferenceList = new List<GameObject>();

        private string PrefabPath => AssetDatabase.GetAssetPath(Prefab);
        public string Name => name;

        private IDictionary<GameObject, int> _objectIdMap;

        public IBehaviorTreeNode AddNode(Type nodeType, Vector2 position)
        {
            EnsureNodeList();

            var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            Assert.IsNotNull(prefabStage);
            CreateNodeObject();
            SavePrefab();
            var nodeId = AddObject();
            return new Node(this, nodeId);

            GameObject CreateNodeObject()
            {
                var node = new GameObject();
                node.transform.SetParent(prefabStage.prefabContentsRoot.transform);
                var dynamicNode = node.AddComponent<BTDynamicNode>();
                dynamicNode.NodeData = new NodeAsset { NodeType = nodeType.AssemblyQualifiedName };
                node.name = nodeType.Name;
                return node;
            }

            void SavePrefab()
            {
                // force save the prefab asset to be able to fetch saved GameObject from it
                PrefabUtility.SaveAsPrefabAsset(prefabStage.prefabContentsRoot, PrefabPath);
                // EditorSceneManager.MarkSceneDirty(prefabStage.scene);
            }

            int AddObject()
            {
                Assert.AreEqual(_nodePositionList.Count, _objectIdMap.Count);
                Assert.AreEqual(_nodeReferenceList.Count, _objectIdMap.Count);

                var addedObject = Prefab.transform.GetChild(Prefab.transform.childCount - 1).gameObject;
                return AddNode(addedObject, position);
            }
        }

        public void OnBeforeSerialize() {}

        public void OnAfterDeserialize()
        {
            _objectIdMap = _nodeReferenceList
                .Select((node, index) => (node, index))
                .ToDictionary(t => t.node, t => t.index)
            ;
        }

        public IEnumerator<IBehaviorTreeNode> GetEnumerator()
        {
            EnsureNodeList();
            return _nodeReferenceList.Select((_, index) => new Node(this, index)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private int AddNode(GameObject reference, Vector2 position)
        {
            var id = _nodeReferenceList.Count;
            _nodeReferenceList.Add(reference);
            _nodePositionList.Add(position);
            _objectIdMap.Add(reference, id);
            _nodes.Add(new Node(this, id));
            return id;
        }

        private void RemoveNodesAt(IEnumerable<int> indices)
        {
            foreach (var index in indices.OrderByDescending(i => i))
            {
                var reference = _nodeReferenceList[index];
                _nodeReferenceList.RemoveAt(index);
                _nodePositionList.RemoveAt(index);
                _objectIdMap.Remove(reference);
            }
            ResetNodes();
        }

        private void ResetNodes()
        {
            for (var id = 0; id < _nodes.Count; id++) _nodes[id].Id = id;
        }

        private void EnsureNodeList()
        {
            Assert.IsTrue(ValidateNodeList());

            var nodeKeys = new HashSet<GameObject>(_nodeReferenceList);
            foreach (var descendant in Prefab.Descendants())
            {
                if (!nodeKeys.Contains(descendant) && descendant.GetComponent<INodeDataBuilder>() != null)
                    AddNode(descendant, Vector2.zero);
                nodeKeys.Remove(descendant);
            }

            RemoveNodesAt(nodeKeys.Select(key => _objectIdMap[key]));
        }

        bool ValidateNodeList()
        {
            if (_nodeReferenceList.Count != _nodePositionList.Count) return false;
            if (_nodeReferenceList.Count != _objectIdMap.Count) return false;

            return _nodeReferenceList
                .Select((node, index) => (node, index))
                .All(t =>
                {
                    var (reference, index) = t;
                    var hasObject = _objectIdMap.TryGetValue(reference, out var objectIndex);
                    return hasObject && objectIndex == index;
                });
        }
    }
}