using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Nuwa.Editor
{
    public static class MultiPropertyDrawerRegister
    {
        public delegate void DrawFunc(Rect position, UnityEditor.SerializedProperty property, GUIContent label);
        public delegate float GetHeightFunc(SerializedProperty property, GUIContent label);

        private class DrawerEntry : IEquatable<DrawerEntry>
        {
            internal readonly Type Type;
            internal readonly CustomMultiPropertyDrawerAttribute Attribute;

            public DrawerEntry(Type type, CustomMultiPropertyDrawerAttribute attribute)
            {
                Type = type;
                Attribute = attribute;
            }

            public bool Equals(DrawerEntry other)
            {
                return Equals(Type, other.Type) && Equals(Attribute, other.Attribute);
            }

            public override bool Equals(object obj)
            {
                return obj is DrawerEntry other && Equals(other);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((Type != null ? Type.GetHashCode() : 0) * 397) ^ (Attribute != null ? Attribute.GetHashCode() : 0);
                }
            }
        }

        private static readonly IReadOnlyDictionary<Type/* of CustomMultiPropertyDrawerAttribute */, DrawerEntry> _drawerRegister;
        private static readonly IDictionary<(object target, string propertyPath), IReadOnlyList<IMultiPropertyDrawer>> _drawers;

        static MultiPropertyDrawerRegister()
        {
            _drawers = new Dictionary<(object, string), IReadOnlyList<IMultiPropertyDrawer>>();
            _drawerRegister = new ReadOnlyDictionary<Type, DrawerEntry>(
                (
                    from drawerType in TypeCache.GetTypesWithAttribute<CustomMultiPropertyDrawerAttribute>()
                    from attribute in GetCustomPropertyDrawerAttribute(drawerType).Yield()
                    where attribute != null
                    select (drawerType, attribute, attributeType: attribute.Type)
                ).ToDictionary(t => t.attributeType, t => new DrawerEntry(t.drawerType, t.attribute))
            );

            CustomMultiPropertyDrawerAttribute GetCustomPropertyDrawerAttribute(Type type)
            {
                if (type.IsInterface) return null;
                if (type.IsAbstract) return null;
                if (type.IsGenericType) return null;
                if (!typeof(IMultiPropertyDrawer).IsAssignableFrom(type)) return null;
                var attribute = type.GetCustomAttribute<CustomMultiPropertyDrawerAttribute>();
                if (!typeof(MultiPropertyAttribute).IsAssignableFrom(attribute.Type))
                {
                    Debug.LogError($"{attribute.GetType().FullName}.{nameof(attribute.Type)} must be a type of {nameof(MultiPropertyAttribute)}");
                    return null;
                }
                return attribute;
            }
        }

        internal static GetHeightFunc GetHeight([NotNull] FieldInfo fieldInfo)
        {
            return (property, label) => GetOrCreateDrawer(fieldInfo, property).FirstOrDefault()?.GetPropertyHeight(property, label)
                                        ?? EditorGUI.GetPropertyHeight(property, true);
        }

        internal static DrawFunc DrawMultiProperty([NotNull] FieldInfo fieldInfo)
        {
            return (position, property, label) => GetOrCreateDrawer(fieldInfo, property).FirstOrDefault()?.OnGUI(position, property, label);
        }

        internal static VisualElement CreatePropertyGUI(FieldInfo fieldInfo, SerializedProperty property)
        {
            return GetOrCreateDrawer(fieldInfo, property).FirstOrDefault()?.CreatePropertyGUI(property);
        }

        private static IReadOnlyList<IMultiPropertyDrawer> GetOrCreateDrawer(FieldInfo fieldInfo, SerializedProperty property)
        {
            if (_drawers.TryGetValue((property.serializedObject.targetObject, property.propertyPath), out var drawerList)) return drawerList;

            var attributes = fieldInfo
                .GetCustomAttributes<MultiPropertyAttribute>()
                .OrderBy(attribute => attribute.order)
                .ToArray()
            ;
            var drawers = new List<IMultiPropertyDrawer>(attributes.Length);
            for (var i = 0; i < attributes.Length; i++)
            {
                var type = attributes[i].GetType();
                var drawerEntry = FindEntry(type);
                if (drawerEntry.Attribute.Type != type && !drawerEntry.Attribute.UseForChildren)
                {
                    Debug.LogError($"Cannot create drawer of {type.FullName}");
                }
                else
                {
                    var drawer = (IMultiPropertyDrawer)Activator.CreateInstance(drawerEntry.Type);
                    drawer.SortedDrawers = drawers;
                    drawer.Decorator = attributes[i];
                    drawer.AttributeIndex = i;
                    drawer.FieldInfo = fieldInfo;
                    drawers.Add(drawer);
                }
            }
            _drawers[(property.serializedObject.targetObject, property.propertyPath)] = drawers;
            return drawers;

            DrawerEntry FindEntry(Type attributeType)
            {
                if (attributeType == null) return null;
                if (_drawerRegister.TryGetValue(attributeType, out var entry)) return entry;
                return FindEntry(attributeType.BaseType);
            }
        }
    }
}