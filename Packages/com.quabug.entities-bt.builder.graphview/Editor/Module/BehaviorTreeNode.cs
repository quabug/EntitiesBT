using System;
using System.Collections.Generic;
using EntitiesBT.Core;
using UnityEditor;
using UnityEngine;

namespace EntitiesBT.Editor
{
    [Serializable]
    public class BehaviorTreeNode : IBehaviorTreeNode
    {
        private readonly BehaviorTreeGraph _graph;

        public GameObject Instance { get; }
        public int Id => Instance.GetInstanceID();

        public string Name => Instance.name;

        public bool IsActive
        {
            get => Instance.activeInHierarchy;
            set => Instance.SetActive(value);
        }

        public Vector2 Position
        {
            get => Instance.transform.localPosition;
            set => _graph.SetPosition(Instance, value, Order);
        }

        public BehaviorNodeType BehaviorType => Instance.GetComponent<INodeDataBuilder>().GetBehaviorNodeType();
        public Type NodeType => Instance.GetComponent<INodeDataBuilder>().GetNodeType();

        public bool IsSelected
        {
            get => _graph.IsSelected(Instance);
            set
            {
                if (value) _graph.Select(Instance);
                else _graph.Unselect(Instance);
            }
        }

        public SerializedObject NodeObject => new SerializedObject((MonoBehaviour)Instance.GetComponent<INodeDataBuilder>());

        public event Action OnSelected;
        public void EmitOnSelected() => OnSelected?.Invoke();

        public IEnumerable<IBehaviorTreeNode> Children => _graph.GetChildrenBehaviorNodes(Id);

        public BehaviorTreeNode(BehaviorTreeGraph graph, GameObject instance)
        {
            _graph = graph;
            Instance = instance;
        }

        public void Dispose()
        {
            _graph.RemoveNode(Instance);
        }

        private float Order(GameObject obj) => obj.transform.localPosition.x;

        public void SetParent(IBehaviorTreeNode parent)
        {
            _graph.SetParent(child: Instance, parent: parent == null ? null : _graph.GetBehaviorNodeById(parent.Id).Instance, Order);
        }
    }
}