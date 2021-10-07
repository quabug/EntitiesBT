using UnityEngine;

namespace EntitiesBT.Editor
{
    public class VariantNode : IVariantNode
    {
        private readonly BehaviorTreeGraph _graph;

        public GameObject Instance { get; }
        public int Id => Instance.GetInstanceID();

        public VariantNode(BehaviorTreeGraph graph, GameObject instance)
        {
            _graph = graph;
            Instance = instance;
        }
    }
}