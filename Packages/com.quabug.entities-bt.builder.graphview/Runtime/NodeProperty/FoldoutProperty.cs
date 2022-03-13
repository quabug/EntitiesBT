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
                view.style.width = new StyleLength(12);
                var toggle = view.Q<Toggle>();
                if (toggle != null)
                {
                    toggle.style.marginTop = new StyleLength(0f);
                    toggle.style.marginBottom = new StyleLength(0f);
                    toggle.style.marginLeft = new StyleLength(0f);
                    toggle.style.marginRight = new StyleLength(0f);
                }

                if (property.BoolProperty != null && toggle != null)
                {
                    Assert.AreEqual(property.BoolProperty.propertyType, SerializedPropertyType.Boolean);
                    toggle.BindProperty(property.BoolProperty);
                }
                return view;
            }
        }
    }
}

#endif
