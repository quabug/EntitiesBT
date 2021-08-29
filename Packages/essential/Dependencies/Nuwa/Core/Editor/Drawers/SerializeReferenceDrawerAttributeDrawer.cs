/***
 * MIT License
 *
 * Copyright (c) 2020 TextusGames
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

// https://github.com/TextusGames/UnitySerializedReferenceUI/

using System;
using System.Collections.Generic;
using System.Linq;
using Shtif;
using UnityEditor;
using UnityEngine;

namespace Nuwa.Editor
{
    [CustomMultiPropertyDrawer(typeof(SerializeReferenceDrawerAttribute))]
    public class SerializeReferenceDrawerAttributeDrawer : BaseMultiPropertyDrawer
    {
        protected override void OnGUISelf(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var labelPosition = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(labelPosition, label);

            var attribute = (SerializeReferenceDrawerAttribute) Decorator;
            var filters = GetAllBuiltInTypeRestrictions();
            var (renamePattern, renameReplacement) = attribute.RenamePatter.ParseReplaceRegex();
            var categoryMethodInfo = attribute.CategoryName == null ? null : property.GetSiblingMethodInfo(attribute.CategoryName);
            var categoryFunc = categoryMethodInfo == null
                ? null
                : (Func<Type, string>) categoryMethodInfo.CreateDelegate(typeof(Func<Type, string>), property.serializedObject.targetObject)
            ;

            var isNullable = string.IsNullOrEmpty(attribute.NullableVariable) ? attribute.Nullable : (bool)property.GetSiblingValue(attribute.NullableVariable);

            DrawSelectionButtonForManagedReference();

            EditorGUI.PropertyField(position, property, GUIContent.none, true);

            EditorGUI.EndProperty();

            IEnumerable<Func<Type, bool>> GetAllBuiltInTypeRestrictions()
            {
                var result = new List<Func<Type, bool>>();
                if (!string.IsNullOrEmpty(attribute.TypeRestrictBySiblingProperty))
                {
                    var baseType = property.GetSiblingPropertyInfo(attribute.TypeRestrictBySiblingProperty).PropertyType;
                    var derivedTypes = TypeCache.GetTypesDerivedFrom(baseType);
                    result.Add(derivedTypes.Contains);
                }

                if (!string.IsNullOrEmpty(attribute.TypeRestrictBySiblingTypeName))
                {
                    var baseType = Type.GetType((string)property.GetSiblingValue(attribute.TypeRestrictBySiblingTypeName));
                    var derivedTypes = TypeCache.GetTypesDerivedFrom(baseType);
                    result.Add(derivedTypes.Contains);
                }

                return result;
            }

            void DrawSelectionButtonForManagedReference()
            {
                var buttonPosition = position;
                buttonPosition.x += EditorGUIUtility.labelWidth + 1 * EditorGUIUtility.standardVerticalSpacing;
                buttonPosition.width = position.width - EditorGUIUtility.labelWidth - 1 * EditorGUIUtility.standardVerticalSpacing;
                buttonPosition.height = EditorGUIUtility.singleLineHeight;

                var referenceType = GetTypeFromName(property.managedReferenceFullTypename);

                var storedIndent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;
                var storedColor = GUI.backgroundColor;
                GUI.backgroundColor = !isNullable && referenceType == null ? new Color(1, 0, 0) : new Color(0.1f, 0.55f, 0.9f, 1f);

                var content = referenceType == null ? new GUIContent("Null ( Assign )") : MakeContent(referenceType);
                if (GUI.Button(buttonPosition, content))
                    ShowContextMenuForManagedReference();

                GUI.backgroundColor = storedColor;
                EditorGUI.indentLevel = storedIndent;

                void ShowContextMenuForManagedReference()
                {
                    var context = new GenericMenu();
                    FillContextMenu(context);
                    var popup = GenericMenuPopup.Get(context, "");
                    popup.showSearch = true;
                    popup.showTooltip = false;
                    popup.resizeToContent = true;
                    popup.Show(new Vector2(buttonPosition.x, buttonPosition.y));
                }
            }

            void FillContextMenu(GenericMenu contextMenu)
            {
                // Adds "Make Null" menu command
                if (isNullable) contextMenu.AddItem(new GUIContent("Null"), false, SetManagedReferenceToNull);

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

            void SetManagedReferenceToNull()
            {
                property.serializedObject.Update();
                property.managedReferenceValue = null;
                property.serializedObject.ApplyModifiedProperties();
            }

            void AssignNewInstanceCommand(object typeObject)
            {
                var type = (Type)typeObject;
                if (type == property.GetObject()?.GetType()) return;

                var instance = Activator.CreateInstance(type);
                property.serializedObject.Update();
                property.managedReferenceValue = instance;
                property.serializedObject.ApplyModifiedProperties();
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

            IEnumerable<Type> GetAppropriateTypesForAssigningToManagedReference()
            {
                var fieldType = GetTypeFromName(property.managedReferenceFieldTypename);
                var appropriateTypes = new List<Type>();
                if (fieldType == null) return appropriateTypes;

                // Get and filter all appropriate types
                var derivedTypes = TypeCache.GetTypesDerivedFrom(fieldType);
                foreach (var type in derivedTypes)
                {
                    // Skips unity engine Objects (because they are not serialized by SerializeReference)
                    if (type.IsSubclassOf(typeof(UnityEngine.Object)))
                        continue;
                    // Skip abstract classes because they should not be instantiated
                    if (type.IsAbstract)
                        continue;
                    if (type.IsGenericType)
                        continue;
                    if (!type.IsPublic && !type.IsNestedPublic)
                        continue;
                    // Skip types that has no public empty constructors (activator can not create them)
                    if (type.IsClass && type.GetConstructor(Type.EmptyTypes) == null) // Structs still can be created (strangely)
                        continue;
                    // Filter types by provided filters if there is ones
                    if (filters != null && filters.All(f => f == null || f.Invoke(type)) == false)
                        continue;

                    appropriateTypes.Add(type);
                }

                return appropriateTypes;
            }

            Type GetTypeFromName(string fullName)
            {
                var names = fullName.Split(' ');
                return names.Length != 2 ? null : Type.GetType($"{names[1]}, {names[0]}");
            }
        }
    }
}