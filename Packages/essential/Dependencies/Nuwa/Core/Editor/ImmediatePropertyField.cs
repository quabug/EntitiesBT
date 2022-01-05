using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Nuwa.Editor
{
    public class ImmediatePropertyField : PropertyField
    {
        private const string _EVENT_NAME = "UnityEditor.UIElements.SerializedPropertyBindEvent, UnityEditor.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
        private readonly Type _propertyEvent = Type.GetType(_EVENT_NAME);

        public ImmediatePropertyField(SerializedProperty property, string label) : base(property, label)
        {
            if (property.propertyType != SerializedPropertyType.ManagedReference)
                this.BindProperty(property.serializedObject);

            var resetMethods = typeof(PropertyField)
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(mi => mi.Name == "Reset")
            ;
            foreach (var reset in resetMethods)
            {
                var @params = reset.GetParameters();
                if (@params.Length == 1 && @params[0].ParameterType == typeof(SerializedProperty))
                {
                    reset.Invoke(this, new object[] { property });
                    break;
                }

                if (@params.Length == 1 && @params[0].ParameterType == _propertyEvent)
                {
                    var pooled = _propertyEvent.GetMethod("GetPooled", BindingFlags.Static | BindingFlags.Public);
                    var @event = pooled.Invoke(null, new object[] { property });
                    reset.Invoke(this, new object[] { @event });
                    break;
                }
            }
            name = label;
        }

        protected override void ExecuteDefaultActionAtTarget(EventBase evt)
        {
            if (evt.GetType() == _propertyEvent) evt.StopPropagation();
        }
    }
}