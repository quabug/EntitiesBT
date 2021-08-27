using Nuwa.Editor;
using UnityEditor;
using UnityEngine;

namespace Nuwa.Editor
{
    [CustomMultiPropertyDrawer(typeof(AsGameObjectNameAttribute))]
    public class AsGameObjectNameAttributeDrawer : BaseMultiPropertyDrawer
    {
        protected override void OnGUISelf(Rect position, SerializedProperty property, GUIContent label)
        {
            var attribute = (AsGameObjectNameAttribute) Decorator;
            var (renamePattern, renameReplacement) = attribute.NamePatter.ParseReplaceRegex();
            var target = property.serializedObject.targetObject;
            target.name = renamePattern.Replace(property.stringValue, renameReplacement);
            if (string.IsNullOrEmpty(target.name)) target.name = attribute.Default;
            property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}