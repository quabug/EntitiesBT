using UnityEditor;
using UnityEngine;

namespace EntitiesBT.Attributes.Editor
{
    [CustomPropertyDrawer(typeof(MultiPropertyAttribute), true)]
    public class MultiPropertyUnityDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // [optimize] TODO: only call last `GetHeight` of same `fieldInfo` in one frame
            return MultiPropertyDrawerRegister.GetHeight(fieldInfo)(property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // [optimize] TODO: only call last `DrawMultiProperty` of same `fieldInfo` in one frame
            MultiPropertyDrawerRegister.DrawMultiProperty(fieldInfo)(position, property, label);
        }
    }
}