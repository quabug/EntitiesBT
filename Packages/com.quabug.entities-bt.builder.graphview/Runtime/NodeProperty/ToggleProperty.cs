#if UNITY_EDITOR

using GraphExt;
using GraphExt.Editor;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace EntitiesBT.Editor
{
    public class ToggleProperty : INodeProperty
    {
        public int Order { get; set; } = 0;
        public SerializedProperty BoolProperty { get; set; } = null;

        public class ViewFactory : SingleNodePropertyViewFactory<ToggleProperty>
        {
            protected override VisualElement CreateView(Node node, ToggleProperty property, INodePropertyViewFactory factory)
            {
                var view = new Toggle();
                if (property.BoolProperty != null)
                {
                    Assert.AreEqual(property.BoolProperty.propertyType, SerializedPropertyType.Boolean);
                    view.BindProperty(property.BoolProperty);
                }
                return view;
            }
        }
    }
}

#endif