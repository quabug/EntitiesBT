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

        public SerializedProperty Name { get; }
        public SerializedProperty IsActive { get; }
        public SerializedObject NodeObject { get; }

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

        public BehaviorTreeNode(BehaviorTreeGraph graph, GameObject instance)
        {
            _graph = graph;
            Instance = instance;
            var serializedInstance = new SerializedObject(instance);
            Name = serializedInstance.FindProperty("m_Name");
            IsActive = serializedInstance.FindProperty("m_IsActive");
            NodeObject = new SerializedObject((MonoBehaviour)Instance.GetComponent<INodeDataBuilder>());
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
}