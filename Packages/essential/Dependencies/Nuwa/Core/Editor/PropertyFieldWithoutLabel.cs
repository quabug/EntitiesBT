using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Nuwa.Editor
{
    public class PropertyFieldWithoutLabel : PropertyField
    {
        public PropertyFieldWithoutLabel(SerializedProperty property) : base(property, label: null)
        {
            this.BindProperty(property.serializedObject);
        }

        protected override void ExecuteDefaultActionAtTarget(EventBase evt)
        {
            base.ExecuteDefaultActionAtTarget(evt);
            var nameLabel = this.Q<Label>();
            // HACK: hide label of PropertyField
            if (nameLabel != null && evt.GetType().Name == "SerializedPropertyBindEvent" /* cannot access `nameof` internal class */)
                nameLabel.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
        }
    }
}