using System;
using UnityEditor;
using UnityEngine;

namespace EntitiesBT.Editor
{
    public class SyntaxTreeNode : ISyntaxTreeNode
    {
        private readonly BehaviorTreeGraph _graph;

        public GameObject Instance { get; }
        public int Id => Instance.GetInstanceID();

        public Vector2 Position
        {
            get => Instance.transform.localPosition;
            set => Instance.transform.localPosition = value;
        }

        public string Name => Instance.name;
        public SerializedObject NodeObject => new SerializedObject(_node);
        public Type VariantType => _node.VariantType;

        public bool IsSelected
        {
            get => _graph.IsSelected(Instance);
            set
            {
                if (value) _graph.Select(Instance);
                else _graph.Unselect(Instance);
            }
        }
        public event Action OnSelected;
        internal void EmitOnSelected() => OnSelected?.Invoke();

        private VariantNode _node { get; }

        public SyntaxTreeNode(BehaviorTreeGraph graph, GameObject instance)
        {
            _graph = graph;
            Instance = instance;
            _node = Instance.GetComponent<VariantNode>();
        }

        public void Dispose()
        {
            _graph.RemoveNode(Instance);
        }
    }
}