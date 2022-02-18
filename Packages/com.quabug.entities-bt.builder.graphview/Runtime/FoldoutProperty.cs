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
    public class FoldoutProperty : INodeProperty
    {
        public int Order { get; set; } = 0;
        public SerializedProperty BoolProperty { get; set; } = null;

        public class ViewFactory : SingleNodePropertyViewFactory<FoldoutProperty>
        {
            protected override VisualElement CreateView(Node node, FoldoutProperty property, INodePropertyViewFactory factory)
            {
                var view = new Foldout();
                if (property.BoolProperty != null)
                {
                    Assert.AreEqual(property.BoolProperty.propertyType, SerializedPropertyType.Boolean);
                    view.Q<Toggle>().BindProperty(property.BoolProperty);
                }
                return view;
            }
        }
    }
}

#endif
