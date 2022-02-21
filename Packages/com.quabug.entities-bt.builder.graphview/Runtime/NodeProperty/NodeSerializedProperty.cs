#if UNITY_EDITOR

using System.Reflection;
using GraphExt;
using GraphExt.Editor;
using JetBrains.Annotations;
using Nuwa.Editor;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace EntitiesBT.Editor
{
    public class NodeSerializedProperty : INodeProperty
    {
        public int Order => 0;
        public SerializedProperty Property { get; }
        public bool HideFoldoutToggle { get; set; }= false;
        public SerializedProperty ToggleProperty { get; set; } = null;

        public NodeSerializedProperty(SerializedProperty property)
        {
            Property = property;
        }

        public class Factory : INodePropertyFactory
        {
            public INodeProperty Create(MemberInfo memberInfo, object nodeObj, NodeId nodeId, SerializedProperty fieldProperty = null)
            {
                return new NodeSerializedProperty(fieldProperty);
            }
        }

        [UsedImplicitly]
        private class ViewFactory : SingleNodePropertyViewFactory<NodeSerializedProperty>
        {
            protected override VisualElement CreateView(Node node, NodeSerializedProperty property, INodePropertyViewFactory factory)
            {
                var view = new ImmediatePropertyField(property.Property, label: null);
                var toggle = view.Q<Toggle>();
                toggle?.SetValueWithoutNotify(true);
                if (property.HideFoldoutToggle && toggle != null) toggle.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
                if (property.ToggleProperty != null)
                {
                    Assert.AreEqual(SerializedPropertyType.Boolean, property.ToggleProperty.propertyType);
                    ToggleView(property.ToggleProperty);
                    Nuwa.Editor.BindingExtensions.TrackPropertyValue(view, property.ToggleProperty, ToggleView);
                }
                return view;

                void ToggleView(SerializedProperty toggleProperty)
                {
                    view.style.display = toggleProperty.boolValue ? DisplayStyle.Flex : DisplayStyle.None;
                }
            }
        }
    }
}

#endif