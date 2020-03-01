// using EntitiesBT.Variable;
// using UnityEditor;
// using UnityEngine;
//
// namespace EntitiesBT.Editor
// {
//     [VariablePropertyDrawer(typeof(CustomVariableProperty<>))]
//     public class CustomVariablePropertyDrawer : PropertyDrawer
//     {
//         public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
//         {
//             return EditorGUI.GetPropertyHeight(property);
//         }
//
//         public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//         {
//             var value = EditorGUI.IntField(position, new GUIContent("test"), 0);
//             // EditorGUI.PropertyField(position, property, label, true);
//         }
//         
//     }
// }
