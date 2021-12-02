using System;
using System.Collections.Generic;
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

        public IEnumerable<GraphNodeVariant.Any> GraphNodeVariants => new SerializedObject(_node).FindGraphNodeVariantProperties().ToGraphNodeVariant();

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

        public void Connect(GraphNodeVariant.Any variant, int variantPortIndex, int syntaxNodePortIndex)
        {
            var changed = variant.Node != _node || variant.VariantPortIndex != variantPortIndex || variant.SyntaxNodePortIndex != syntaxNodePortIndex;
            variant.Node = _node;
            variant.VariantPortIndex = variantPortIndex;
            variant.SyntaxNodePortIndex = syntaxNodePortIndex;
            _node.OnConnected(variant);
            if (changed) _graph.SavePrefab();
        }

        public void Disconnect(GraphNodeVariant.Any variant)
        {
            var changed = variant.Node != null;
            variant.Node = null;
            variant.VariantPortIndex = -1;
            variant.SyntaxNodePortIndex = -1;
            _node.OnDisconnected(variant);
            if (changed) _graph.SavePrefab();
        }
    }
}