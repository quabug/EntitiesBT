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
    public interface IBehaviorTreeGraph : IEnumerable<BehaviorTreeGraphNode>
    {
        string Name { get; }
        BehaviorTreeGraphNode AddNode(Type nodeType, Vector2 position);
        BehaviorTreeGraphNode MoveNode(int id, Vector2 position);
    }

    public readonly struct BehaviorTreeGraphNode
    {
        public readonly int Id;
        public readonly string Title;
        public readonly Vector2 Position;

        public BehaviorTreeGraphNode(int id, string title, Vector2 position)
        {
            Id = id;
            Title = title;
            Position = position;
        }
    }

    [CreateAssetMenu(fileName = "BehaviorTreeGraph", menuName = "EntitiesBT/BehaviorTreeGraph", order = 0)]
    public class BehaviorTreeGraph : ScriptableObject, IBehaviorTreeGraph
    {
        [Serializable]
        private class Node
        {
            public GameObject Reference;
            public Vector2 Position;
        }

        private const int _rootId = -1;
        private const string _rootName = "Root";

        [Nuwa.ReadOnly, UnityDrawProperty] public GameObject Prefab;
        [Nuwa.ReadOnly, UnityDrawProperty] public Vector2 RootPosition;
        [SerializeField, Nuwa.ReadOnly, UnityDrawProperty] private List<Node> _nodeList = new List<Node>();

        private string PrefabPath => AssetDatabase.GetAssetPath(Prefab);
        public string Name => name;

        private Lazy<IDictionary<GameObject, int>> _objectIdMap;

        public BehaviorTreeGraph()
        {
            ResetObjectIdMap();
        }

        void ResetObjectIdMap()
        {
            _objectIdMap = new Lazy<IDictionary<GameObject, int>>(
                () => _nodeList.Select((node, index) => (node, index)).ToDictionary(t => t.node.Reference, t => t.index)
            );
        }

        public BehaviorTreeGraphNode AddNode(Type nodeType, Vector2 position)
        {
            EnsureNodeList();

            var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            Assert.IsNotNull(prefabStage);
            CreateNodeObject();
            SavePrefab();
            var nodeId = AddObject();
            return new BehaviorTreeGraphNode(nodeId, _nodeList[nodeId].Reference.name, position);

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
                Assert.AreEqual(_nodeList.Count, _objectIdMap.Value.Count);
                var id = _nodeList.Count;
                var addedObject = Prefab.transform.GetChild(Prefab.transform.childCount - 1).gameObject;
                _nodeList.Add(new Node { Reference = addedObject, Position = position });
                _objectIdMap.Value.Add(addedObject, id);
                return id;
            }
        }

        public BehaviorTreeGraphNode MoveNode(int id, Vector2 position)
        {
            SetNodePosition(id, position);
            return GetNodeData(id);
        }

        private void SetNodePosition(int id, Vector2 position)
        {
            if (id == _rootId) RootPosition = position;
            else _nodeList[id].Position = position;
        }

        private BehaviorTreeGraphNode GetNodeData(int id)
        {
            return id == _rootId
                ? GetRootNode()
                : new BehaviorTreeGraphNode(id, _nodeList[id].Reference.name, _nodeList[id].Position)
            ;
        }

        private BehaviorTreeGraphNode GetRootNode()
        {
            return new BehaviorTreeGraphNode(_rootId, _rootName, RootPosition);
        }

        public void Remove()
        {

        }

        public void Connect()
        {

        }

        public IEnumerator<BehaviorTreeGraphNode> GetEnumerator()
        {
            EnsureNodeList();
            var root = GetRootNode();
            return _nodeList.Select((node, index) => GetNodeData(index)).Append(root).GetEnumerator();
        }

        private void EnsureNodeList()
        {
            Assert.AreEqual(_nodeList.Count, _objectIdMap.Value.Count);
            Assert.IsTrue(CheckNodeListMap());

            var nodeKeys = new HashSet<GameObject>(_objectIdMap.Value.Keys);
            foreach (var descendant in Prefab.Descendants())
            {
                if (!nodeKeys.Contains(descendant) && descendant.GetComponent<INodeDataBuilder>() != null)
                {
                    var id = _nodeList.Count;
                    _nodeList.Add(new Node { Reference = descendant, Position = Vector2.zero });
                    _objectIdMap.Value.Add(descendant, id);
                }
                nodeKeys.Remove(descendant);
            }

            foreach (var removedIndex in nodeKeys.Select(key => _objectIdMap.Value[key]).OrderByDescending(i => i))
            {
                var nodeObject = _nodeList[removedIndex].Reference;
                _nodeList.RemoveAt(removedIndex);
                _objectIdMap.Value.Remove(nodeObject);
            }

            bool CheckNodeListMap()
            {
                return _nodeList
                    .Select((node, index) => (node.Reference, index))
                    .All(t =>
                    {
                        var (reference, index) = t;
                        var hasObject = _objectIdMap.Value.TryGetValue(reference, out var objectIndex);
                        return hasObject && objectIndex == index;
                    });
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}