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

        public SyntaxTreeNode(BehaviorTreeGraph graph, GameObject instance)
        {
            _graph = graph;
            Instance = instance;
        }
    }
}