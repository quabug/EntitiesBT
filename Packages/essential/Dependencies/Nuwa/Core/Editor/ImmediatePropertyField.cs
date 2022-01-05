using System;
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
            this.BindProperty(property.serializedObject);
            var pooled = _propertyEvent.GetMethod("GetPooled", BindingFlags.Static | BindingFlags.Public);
            var @event = pooled.Invoke(null, new object[] { property });
            var reset = typeof(PropertyField).GetMethod("Reset", BindingFlags.Instance | BindingFlags.NonPublic);
            reset.Invoke(this, new[] { @event });
            name = label;
        }

        protected override void ExecuteDefaultActionAtTarget(EventBase evt)
        {
            if (evt.GetType() == _propertyEvent) evt.StopPropagation();
        }
    }
}