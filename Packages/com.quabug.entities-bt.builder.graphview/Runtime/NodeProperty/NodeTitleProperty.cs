#if UNITY_EDITOR

using System;
using GraphExt;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace EntitiesBT.Editor
{
    public class NodeTitleProperty : INodeProperty
    {
        public int Order { get; set; } = 0;

        private event Action<string> TitleChanged;
        private string _title;
        public string Title
        {
            private get => _title;
            set
            {
                _title = value;
                TitleChanged?.Invoke(_title);
            }
        }

        public SerializedProperty ToggleProperty { get; set; } = null;

        public class View : Foldout
        {
            public View(Node node, NodeTitleProperty property)
            {
                name = "node-title-property";
                text = property._title;
                property.TitleChanged += title =>
                {
                    style.display = title == null ? DisplayStyle.None : DisplayStyle.Flex;
                    text = title;
                };

                var toggle = this.Q<Toggle>();
                if (property.ToggleProperty != null && toggle != null)
                {
                    Assert.AreEqual(property.ToggleProperty.propertyType, SerializedPropertyType.Boolean);
                    toggle.BindProperty(property.ToggleProperty);
                    toggle.RegisterValueChangedCallback(evt => UpdateExpandedNodeState(evt.newValue));
                }

                void UpdateExpandedNodeState(bool expanded)
                {
                    node.RemoveFromClassList(expanded ? "collapsed" : "expanded");
                    node.AddToClassList(expanded ? "expanded" : "collapsed");
                }
            }
        }

        public class ViewFactory : GraphExt.Editor.SingleNodePropertyViewFactory<NodeTitleProperty>
        {
            protected override VisualElement CreateView(Node node, NodeTitleProperty property, GraphExt.Editor.INodePropertyViewFactory factory)
            {
                return new View(node, property);
            }
        }
    }
}

#endif
