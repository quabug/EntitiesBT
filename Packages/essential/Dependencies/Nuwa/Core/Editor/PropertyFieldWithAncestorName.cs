using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Nuwa.Editor
{
    public class PropertyFieldWithAncestorName : PropertyField
    {
        public PropertyFieldWithAncestorName(SerializedProperty property) : base(property)
        {
            this.BindProperty(property.serializedObject);
        }

        protected override void ExecuteDefaultAction(EventBase evt)
        {
            if (evt is AttachToPanelEvent)
            {
                var propertyField = GetFirstAncestorOfType<PropertyField>();
                if (propertyField != null) label = propertyField.name;
            }

            base.ExecuteDefaultAction(evt);
        }
    }
}