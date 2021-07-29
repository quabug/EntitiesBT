using System.Linq;
using EntitiesBT.Attributes;
using EntitiesBT.Attributes.Editor;
using UnityEditor;
using UnityEngine;

namespace EntitiesBT.Editor
{
    [CustomMultiPropertyDrawer(typeof(SerializedTypeAttribute))]
    public class SerializedTypeDrawer : BaseMultiPropertyDrawer
    {
        private string[] _options;

        protected override void OnGUISelf(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.String)
            {
                if (_options == null)
                {
                    var baseType = ((SerializedTypeAttribute) Decorator).BaseType;
                    _options = TypeCache.GetTypesDerivedFrom(baseType).Select(type => type.AssemblyQualifiedName).ToArray();
                }
                property.PopupFunc()(position, label.text, _options);
            }
            else
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }
    }
}
