using UnityEditor;
using UnityEngine;

namespace EntitiesBT.Attributes.Editor
{
    [CustomPropertyDrawer(typeof(MultiPropertyAttribute), true)]
    public class MultiPropertyUnityDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return MultiPropertyDrawerRegister.GetHeight(fieldInfo)(property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            MultiPropertyDrawerRegister.DrawMultiProperty(fieldInfo)(position, property, label);
        }
    }
}