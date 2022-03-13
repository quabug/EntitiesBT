using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Nuwa.Editor
{
    public class PropertyFieldFoldOut : Foldout
    {
        protected override void ExecuteDefaultAction(EventBase evt)
        {
            if (evt is AttachToPanelEvent)
            {
                var propertyField = GetFirstAncestorOfType<PropertyField>();
                if (propertyField != null) text = propertyField.name;
            }

            base.ExecuteDefaultAction(evt);
        }
    }
}