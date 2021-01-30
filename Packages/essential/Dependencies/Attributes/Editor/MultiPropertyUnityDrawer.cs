using System.Linq;
using UnityEditor;
using UnityEngine;

namespace EntitiesBT.Attributes.Editor
{
    [CustomPropertyDrawer(typeof(MultiPropertyAttribute), true)]
    public class MultiPropertyUnityDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var drawers = MultiPropertyDrawerRegister.GetOrCreateDrawers(fieldInfo);
            drawers.FirstOrDefault()?.OnGUI(position, property, label);
        }
    }
}