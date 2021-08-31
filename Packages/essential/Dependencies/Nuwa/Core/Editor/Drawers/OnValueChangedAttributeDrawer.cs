using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Nuwa.Editor
{
    [CustomMultiPropertyDrawer(typeof(OnValueChangedAttribute))]
    public class OnValueChangedAttributeDrawer : BaseMultiPropertyDrawer
    {
        private static Dictionary<(object declaringObject, string propertyPath), object> ObservaedProperties;

        static OnValueChangedAttributeDrawer()
        {
            ObservaedProperties = new Dictionary<(object declaringObject, string propertyPath), object>();
        }

        protected override void OnGUISelf(Rect position, SerializedProperty property, GUIContent label)
        {
            var fields = property.GetFieldsByPath().Reverse().Take(2).Select(f => f.field).ToArray();
            if (fields.Length != 2 && fields.All(f => f != null)) return;

            var attribute = (OnValueChangedAttribute) Decorator;
            var currentValue = fields[0];
            if (!string.IsNullOrEmpty(attribute.PropertyName) && currentValue != null)
                currentValue = GetPropertyValue(currentValue);

            var declaringObject = fields[1];
            var id = (declaringObject, property.propertyPath);
            var hasValue = ObservaedProperties.TryGetValue(id, out var previousValue);
            if (!hasValue)
            {
                ObservaedProperties[id] = currentValue;
                return;
            }

            if (!Equals(currentValue, previousValue))
            {
                ObservaedProperties[id] = currentValue;

                var method = declaringObject.GetType().GetMethod(
                    attribute.Callback, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                );

                if (method != null && !method.GetParameters().Any())// Only instantiate methods with 0 parameters
                    method.Invoke(declaringObject, null);
            }

            object GetPropertyValue(object obj)
            {
                if (obj == null) return null;
                var property = obj.GetType().GetProperty(attribute.PropertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                return property.GetValue(obj);
            }
        }
    }
}