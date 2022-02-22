using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Nuwa.Editor
{
    public class PropertyFieldWithAncestorName : PropertyField
    {
        public PropertyFieldWithAncestorName(SerializedProperty property) : base(property, label: null)
        {
            this.BindProperty(property.serializedObject);
        }

        protected override void ExecuteDefaultAction(EventBase evt)
        {
            if (evt is AttachToPanelEvent)
            {
                var propertyField = GetFirstAncestorOfType<PropertyField>();
                if (propertyField != null)
                {
                    name = propertyField.name;
                    label = propertyField.name;
                }
            }
            base.ExecuteDefaultAction(evt);
        }
    }
}