using Nuwa.Editor;
using UnityEditor;
using UnityEngine;

namespace Nuwa.Editor
{
    [CustomMultiPropertyDrawer(typeof(ThisComponentAttribute))]
    public class ThisComponentAttributeDrawer : BaseMultiPropertyDrawer
    {
        protected override void OnGUISelf(Rect position, SerializedProperty property, GUIContent label)
        {
            var fieldInfo = property.GetPropertyField().fieldInfo;
            property.objectReferenceValue = ((Component)property.serializedObject.targetObject).GetComponent(fieldInfo.FieldType);
            property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}