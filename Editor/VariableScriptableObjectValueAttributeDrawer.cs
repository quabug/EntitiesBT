using EntitiesBT.Components;
using UnityEditor;
using UnityEngine;

namespace EntitiesBT.Editor
{
    [CustomPropertyDrawer(typeof(VariableScriptableObjectValueAttribute))]
    public class VariableScriptableObjectValueAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
        }
    }
}
