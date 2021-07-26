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
using System.Reflection;
using System.Text.RegularExpressions;
using Shtif;
using UnityEditor;
using UnityEngine;

namespace EntitiesBT.Attributes.Editor
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
            DrawSelectionButtonForManagedReference();

            EditorGUI.PropertyField(position, property, GUIContent.none, true);

            EditorGUI.EndProperty();

            IEnumerable<Func<Type, bool>> GetAllBuiltInTypeRestrictions()
            {
                var result = new List<Func<Type, bool>>();
                if (!string.IsNullOrEmpty(attribute.TypeRestrictionBySiblingProperty))
                {
                    var baseType = property.GetSiblingPropertyInfo(attribute.TypeRestrictionBySiblingProperty).PropertyType;
                    var derivedTypes = TypeCache.GetTypesDerivedFrom(baseType);
                    result.Add(derivedTypes.Contains);
                }
                return result;
            }

            void DrawSelectionButtonForManagedReference()
            {
                var backgroundColor = new Color(0.1f, 0.55f, 0.9f, 1f);

                var buttonPosition = position;
                buttonPosition.x += EditorGUIUtility.labelWidth + 1 * EditorGUIUtility.standardVerticalSpacing;
                buttonPosition.width = position.width - EditorGUIUtility.labelWidth - 1 * EditorGUIUtility.standardVerticalSpacing;
                buttonPosition.height = EditorGUIUtility.singleLineHeight;

                var storedIndent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;
                var storedColor = GUI.backgroundColor;
                GUI.backgroundColor = backgroundColor;


                var names = GetSplitNamesFromTypename(property.managedReferenceFullTypename);
                var className = string.IsNullOrEmpty(names.ClassName) ? "Null (Assign)" : names.ClassName;
                var assemblyName = names.AssemblyName;
                if (GUI.Button(buttonPosition, new GUIContent(className, className + "  ( "+ assemblyName +" )" )))
                    ShowContextMenuForManagedReference();

                GUI.backgroundColor = storedColor;
                EditorGUI.indentLevel = storedIndent;

                void ShowContextMenuForManagedReference()
                {
                    var context = new GenericMenu();
                    FillContextMenu(context);
                    var popup = GenericMenuPopup.Get(context, "");
                    popup.width = 400;
                    popup.showSearch = true;
                    popup.showTooltip = false;
                    popup.resizeToContent = true;
                    popup.Show(new Vector2(buttonPosition.x, buttonPosition.y));
                    // context.ShowAsContext();
                }
            }

            void FillContextMenu(GenericMenu contextMenu)
            {
                // Adds "Make Null" menu command
                contextMenu.AddItem(new GUIContent("Null"), false, SetManagedReferenceToNull());

                // Collects appropriate types
                var appropriateTypes = GetAppropriateTypesForAssigningToManagedReference();

                // Adds appropriate types to menu
                foreach (var appropriateType in appropriateTypes)
                    AddItemToContextMenu(appropriateType, contextMenu);
            }

            GenericMenu.MenuFunction SetManagedReferenceToNull()
            {
                return () =>
                {
                    property.serializedObject.Update();
                    property.managedReferenceValue = null;
                    property.serializedObject.ApplyModifiedProperties();
                };
            }

            void AddItemToContextMenu(Type type, GenericMenu genericMenuContext)
            {
                var assemblyName =  type.Assembly.ToString().Split('(', ',')[0];
                var entryName = type + "  ( " + assemblyName + " )";
                // HACK: display subclass name after "+"
                entryName = entryName.Substring(entryName.IndexOf('+') + 1);
                genericMenuContext.AddItem(new GUIContent(entryName), false, AssignNewInstanceCommand, new GenericMenuParameterForAssignInstanceCommand(type, property));

                void AssignNewInstanceCommand(object objectGenericMenuParameter)
                {
                    var parameter = (GenericMenuParameterForAssignInstanceCommand) objectGenericMenuParameter;
                    AssignNewInstanceOfTypeToManagedReference(parameter.Property, parameter.Type);
                }

                object AssignNewInstanceOfTypeToManagedReference(SerializedProperty serializedProperty, Type type)
                {
                    var instance = Activator.CreateInstance(type);
                    serializedProperty.serializedObject.Update();
                    serializedProperty.managedReferenceValue = instance;
                    serializedProperty.serializedObject.ApplyModifiedProperties();
                    return instance;
                }
            }

            IEnumerable<Type> GetAppropriateTypesForAssigningToManagedReference()
            {
                var fieldType = GetManagedReferenceFieldType();
                var appropriateTypes = new List<Type>();

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

            // Gets real type of managed reference
            Type GetManagedReferenceFieldType()
            {
                var realPropertyType = GetRealTypeFromTypename(property.managedReferenceFieldTypename);
                if (realPropertyType != null)
                    return realPropertyType;

                Debug.LogError($"Can not get field type of managed reference : {property.managedReferenceFieldTypename}");
                return null;

                // Gets real type of managed reference's field typeName
                Type GetRealTypeFromTypename(string stringType)
                {
                    var names = GetSplitNamesFromTypename(stringType);
                    var realType = Type.GetType($"{names.ClassName}, {names.AssemblyName}");
                    return realType;
                }
            }
        }

        private (string AssemblyName, string ClassName) GetSplitNamesFromTypename(string typename)
        {
            if (string.IsNullOrEmpty(typename))
                return ("","");

            var typeSplitString = typename.Split(char.Parse(" "));
            var typeClassName = typeSplitString[1];
            var typeAssemblyName = typeSplitString[0];
            return (typeAssemblyName,  typeClassName);
        }

        private readonly struct GenericMenuParameterForAssignInstanceCommand
        {
            public GenericMenuParameterForAssignInstanceCommand(Type type, SerializedProperty property)
            {
                Type = type;
                Property = property;
            }

            public readonly SerializedProperty Property;
            public readonly Type Type;
        }
    }
}