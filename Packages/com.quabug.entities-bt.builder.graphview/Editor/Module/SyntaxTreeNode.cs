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

        public SerializedProperty Name { get; }

        public SyntaxTreeNode(BehaviorTreeGraph graph, GameObject instance)
        {
            _graph = graph;
            Instance = instance;

            var serializedInstance = new SerializedObject(instance);
            Name = serializedInstance.FindProperty("m_Name");
        }

        public void Dispose()
        {
            _graph.RemoveNode(Instance);
        }
    }
}