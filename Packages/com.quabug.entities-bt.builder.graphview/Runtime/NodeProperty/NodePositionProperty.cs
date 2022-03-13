#if UNITY_EDITOR

using System;
using GraphExt;
using GraphExt.Editor;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace EntitiesBT.Editor
{
    public class NodePositionProperty : INodeProperty
    {
        private readonly Vector2 _initialPosition;
        private readonly Action<Vector2> _onPositionChanged;
        public int Order { get; set; } = 0;

        public NodePositionProperty(Vector2 initialPosition, Action<Vector2> onPositionChanged)
        {
            _initialPosition = initialPosition;
            _onPositionChanged = onPositionChanged;
        }

        private class View : VisualElement, IDisposable
        {
            private readonly Node _node;
            private readonly NodePositionProperty _property;

            public View(Node node,  NodePositionProperty property)
            {
                _node = node;
                _property = property;
                node.SetPosition(new Rect(property._initialPosition, Vector2.zero));
                style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
                node.RegisterCallback<ElementMovedEvent>(OnNodeMoved);
            }

            public void Dispose()
            {
                _node.UnregisterCallback<ElementMovedEvent>(OnNodeMoved);
            }

            private void OnNodeMoved(ElementMovedEvent evt)
            {
                evt.StopImmediatePropagation();
                _property._onPositionChanged?.Invoke(evt.Position);
            }
        }

        [UsedImplicitly]
        private class ViewFactory : SingleNodePropertyViewFactory<NodePositionProperty>
        {
            protected override VisualElement CreateView(Node node, NodePositionProperty property, INodePropertyViewFactory _)
            {
                return new View(node, property);
            }
        }
    }
}

#endif