#if UNITY_EDITOR

using GraphExt;
using GraphExt.Editor;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace EntitiesBT.Editor
{
    public class BehaviorBlobDataProperty : INodeProperty
    {
        public int Order => 0;
        public SerializedProperty Property;

        [UsedImplicitly]
        private class ViewFactory : NodePropertyViewFactory<BehaviorBlobDataProperty>
        {
            protected override VisualElement Create(Node node, BehaviorBlobDataProperty property, INodePropertyViewFactory factory)
            {
                var field = new PropertyField(property.Property);
                field.BindProperty(property.Property.serializedObject);
                return field;
            }
        }
    }
}

#endif