using System;
using System.Reflection;
using UnityEditor;
using UnityEngine.UIElements;

namespace Nuwa.Editor
{
    public static class BindingExtensions
    {
        private delegate void TrackPropertyValueCall(VisualElement element, SerializedProperty property, Action<SerializedProperty> callback = null);

        private static readonly Lazy<TrackPropertyValueCall> _trackPropertyValueCall = new Lazy<TrackPropertyValueCall>(() =>
        {
            var bindingExtensions = typeof(UnityEditor.UIElements.BindingExtensions);
            var call = bindingExtensions.GetMethod("TrackPropertyValue", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            return (TrackPropertyValueCall) call.CreateDelegate(typeof(TrackPropertyValueCall));
        });

        public static void TrackPropertyValue(this VisualElement element, SerializedProperty property, Action<SerializedProperty> callback = null)
        {
            _trackPropertyValueCall.Value(element, property, callback);
        }
    }
}