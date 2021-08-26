using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Nuwa.Editor
{
    public abstract class BaseMultiPropertyDrawer : IMultiPropertyDrawer
    {
        public MultiPropertyAttribute Decorator { protected get; set; }
        public IReadOnlyList<IMultiPropertyDrawer> SortedDrawers { protected get; set; }
        public int AttributeIndex { get; set; }
        public FieldInfo FieldInfo { protected get; set; }

        protected IMultiPropertyDrawer NextDrawer => AttributeIndex + 1 < SortedDrawers.Count
            ? SortedDrawers[AttributeIndex + 1]
            : null
        ;

        public virtual void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            OnGUISelf(position, property, label);
            NextDrawer?.OnGUI(position, property, label);
        }

        public virtual float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, true);
        }

        protected virtual void OnGUISelf(Rect position, SerializedProperty property, GUIContent label) {}
    }
}