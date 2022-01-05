using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Nuwa.Editor
{
    public class PropertyFieldWithAncestorName : ImmediatePropertyField
    {
        public PropertyFieldWithAncestorName(SerializedProperty property) : base(property, label: null) {}

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