using Nuwa.Editor;
using UnityEditor;
using UnityEngine;

namespace Nuwa.Editor
{
    [CustomMultiPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyAttributeDrawer : BaseMultiPropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var storeEnabled = GUI.enabled;
            GUI.enabled = false;
            base.OnGUI(position, property, label);
            GUI.enabled = storeEnabled;
        }
    }
}