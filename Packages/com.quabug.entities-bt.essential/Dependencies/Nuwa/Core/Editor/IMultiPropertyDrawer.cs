using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Nuwa.Editor
{
    public interface IMultiPropertyDrawer
    {
        MultiPropertyAttribute Decorator { set; }
        IReadOnlyList<IMultiPropertyDrawer> SortedDrawers { set; }
        int AttributeIndex { set; }
        FieldInfo FieldInfo { set; }

        void OnGUI(Rect position, SerializedProperty property, GUIContent label);
        float GetPropertyHeight(SerializedProperty property, GUIContent label);
        VisualElement CreatePropertyGUI(SerializedProperty property);
    }
}