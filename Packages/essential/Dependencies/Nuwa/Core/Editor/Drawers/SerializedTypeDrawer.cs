using System;
using System.Collections.Generic;
using System.Linq;
using Nuwa;
using Nuwa.Editor;
using Shtif;
using UnityEditor;
using UnityEngine;

namespace EntitiesBT.Editor
{
    [CustomMultiPropertyDrawer(typeof(SerializedTypeAttribute))]
    public class SerializedTypeDrawer : BaseMultiPropertyDrawer
    {
        private string[] _options;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        protected override void OnGUISelf(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.PropertyField(position, property, label, true);
                return;
            }

            property.serializedObject.Update();

            position.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.LabelField(position, label);

            position.x += EditorGUIUtility.labelWidth + 1 * EditorGUIUtility.standardVerticalSpacing;
            position.width = position.width - EditorGUIUtility.labelWidth - 1 * EditorGUIUtility.standardVerticalSpacing;

            var attribute = (SerializedTypeAttribute) Decorator;
            var (renamePattern, renameReplacement) = attribute.RenamePatter.ParseReplaceRegex();
            var self = property.GetDeclaringObject();
            var categoryMethodInfo = attribute.CategoryName == null ? null : property.GetSiblingMethodInfo(attribute.CategoryName);
            var categoryFunc = categoryMethodInfo == null
                ? null
                : (Func<Type, string>) categoryMethodInfo.CreateDelegate(typeof(Func<Type, string>), self)
            ;
            var whereMethodInfo = attribute.Where == null ? null : property.GetSiblingMethodInfo(attribute.Where);
            var whereFunc = whereMethodInfo == null
                ? type => true
                : (Func<Type, bool>) whereMethodInfo.CreateDelegate(typeof(Func<Type, bool>), self)
            ;

            ShowButton();

            void ShowButton()
            {
                // var buttonPosition = position;
                // buttonPosition.x += EditorGUIUtility.labelWidth + 1 * EditorGUIUtility.standardVerticalSpacing;
                // buttonPosition.width = position.width - EditorGUIUtility.labelWidth - 1 * EditorGUIUtility.standardVerticalSpacing;
                // buttonPosition.height = EditorGUIUtility.singleLineHeight;

                var type = Type.GetType(property.stringValue);

                var storedColor = GUI.backgroundColor;
                GUI.backgroundColor = attribute.Nullable == false && type == null ? new Color(1, 0, 0) : new Color(0.1f, 0.55f, 0.9f, 1f);

                var content = type == null ? new GUIContent("Null ( Assign )") : MakeContent(type);
                if (GUI.Button(position, content)) ShowContextMenuForManagedReference();

                GUI.backgroundColor = storedColor;
                // EditorGUI.indentLevel = storedIndent;

                // EditorGUI.DropdownButton(position, label, FocusType.Passive);
                // if (_options == null)
                // {
                //     var baseType = ((SerializedTypeAttribute) Decorator).BaseType;
                //     _options = TypeCache.GetTypesDerivedFrom(baseType).Select(type => type.AssemblyQualifiedName).ToArray();
                // }
                // property.PopupFunc()(position, label.text, _options);
            }

            GUIContent MakeContent(Type type)
            {
                var entryName = type.FullName;
                if (renamePattern != null) entryName = renamePattern.Replace(entryName, renameReplacement);
                if (attribute.DisplayAssemblyName) entryName += "  ( " + type.Assembly.GetName().Name + " )";
                if (categoryFunc != null)
                {
                    var category = categoryFunc(type);
                    if (!string.IsNullOrEmpty(category)) entryName = category + "/" + entryName;
                }
                return new GUIContent(entryName, $"{type.FullName} ( {type.Assembly.GetName().Name} )");
            }

            void ShowContextMenuForManagedReference()
            {
                var context = new GenericMenu();
                FillContextMenu(context);
                var popup = GenericMenuPopup.Get(context, "");
                popup.showSearch = true;
                popup.showTooltip = false;
                popup.resizeToContent = true;
                popup.Show(new Vector2(position.x, position.y));
            }

            void FillContextMenu(GenericMenu contextMenu)
            {
                // Adds "Make Null" menu command
                if (attribute.Nullable) contextMenu.AddItem(new GUIContent("Null"), false, SetNull);

                // Collects appropriate types
                var appropriateTypes = GetAppropriateTypesForAssigningToManagedReference();

                // Adds appropriate types to menu
                var typeContentMap =
                    from type in appropriateTypes
                    select (type, content: MakeContent(type))
                ;

                if (attribute.AlphabeticalOrder) typeContentMap = typeContentMap.OrderBy(t => t.content.text);
                foreach (var (type, content) in typeContentMap)
                    contextMenu.AddItem(content, false, AssignNewInstanceCommand, type);
            }

            void SetNull()
            {
                property.serializedObject.Update();
                property.stringValue = "";
                property.serializedObject.ApplyModifiedProperties();
            }

            void AssignNewInstanceCommand(object typeObject)
            {
                var type = (Type)typeObject;
                if (type == property.GetObject()?.GetType()) return;

                property.serializedObject.Update();
                property.stringValue = type.AssemblyQualifiedName;
                property.serializedObject.ApplyModifiedProperties();
            }

            IEnumerable<Type> GetAppropriateTypesForAssigningToManagedReference()
            {
                var baseType = attribute.BaseType ?? typeof(object);
                return TypeCache.GetTypesDerivedFrom(baseType).Where(whereFunc);
            }
        }
    }
}
