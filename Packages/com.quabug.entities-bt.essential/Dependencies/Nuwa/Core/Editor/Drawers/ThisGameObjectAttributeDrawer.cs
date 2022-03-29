using Nuwa.Editor;
using UnityEditor;
using UnityEngine;

namespace Nuwa.Editor
{
    [CustomMultiPropertyDrawer(typeof(ThisGameObjectAttribute))]
    public class ThisGameObjectAttributeDrawer : BaseMultiPropertyDrawer
    {
        protected override void OnGUISelf(Rect position, SerializedProperty property, GUIContent label)
        {
            property.objectReferenceValue = ((Component)property.serializedObject.targetObject).gameObject;
            property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}