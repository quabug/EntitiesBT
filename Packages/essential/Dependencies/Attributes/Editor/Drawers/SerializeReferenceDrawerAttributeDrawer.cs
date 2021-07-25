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
using UnityEditor;
using UnityEngine;

namespace EntitiesBT.Attributes.Editor
{
    [CustomMultiPropertyDrawer(typeof(SerializeReferenceDrawerAttribute))]
    public class SerializeReferenceDrawerAttributeDrawer : BaseMultiPropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var labelPosition = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(labelPosition, label);

            var typeRestrictions = property.IsReferencedArrayElement() ?
                null : GetAllBuiltInTypeRestrictions(property.GetPropertyField().fieldInfo)
            ;

            DrawSelectionButtonForManagedReference(property, position, typeRestrictions);

            EditorGUI.PropertyField(position, property, GUIContent.none, true);

            EditorGUI.EndProperty();

            IEnumerable<Func<Type, bool>> GetAllBuiltInTypeRestrictions(FieldInfo fieldInfo)
            {
                var result = new List<Func<Type, bool>>();

                var attributeObjects = fieldInfo.GetCustomAttributes(false);
                foreach (var attributeObject in attributeObjects)
                {
                    switch (attributeObject)
                    {
                        case SerializeReferenceDrawerPropertyBaseTypeAttribute propertyBaseType:
                        {
                            var baseType = property.GetSiblingPropertyInfo(propertyBaseType.PropertyName).PropertyType;
                            var derivedTypes = TypeCache.GetTypesDerivedFrom(baseType);
                            result.Add(derivedTypes.Contains);
                            break;
                        }
                    }
                }
                return result;
            }
        }

        private void DrawSelectionButtonForManagedReference(SerializedProperty property, Rect position, IEnumerable<Func<Type, bool>> filters = null)
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
                ShowContextMenuForManagedReference(property, filters);

            GUI.backgroundColor = storedColor;
            EditorGUI.indentLevel = storedIndent;
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

        private void ShowContextMenuForManagedReference(SerializedProperty property, IEnumerable<Func<Type,bool>> filters = null)
        {
            var context = new GenericMenu();
            FillContextMenu(filters, context, property);
            context.ShowAsContext();
        }

        private void FillContextMenu(IEnumerable<Func<Type, bool>> enumerableFilters, GenericMenu contextMenu, SerializedProperty property)
        {
            var filters = new List<Func<Type, bool>>();
            if (enumerableFilters != null) filters.AddRange(enumerableFilters);

            // Adds "Make Null" menu command
            contextMenu.AddItem(new GUIContent("Null"), false, SetManagedReferenceToNull(property));

            // Collects appropriate types
            var appropriateTypes = GetAppropriateTypesForAssigningToManagedReference(property, filters);

            // Adds appropriate types to menu
            foreach (var appropriateType in appropriateTypes)
                AddItemToContextMenu(appropriateType, contextMenu, property);
        }

        private GenericMenu.MenuFunction SetManagedReferenceToNull(SerializedProperty serializedProperty)
        {
            return () =>
            {
                serializedProperty.serializedObject.Update();
                serializedProperty.managedReferenceValue = null;
                serializedProperty.serializedObject.ApplyModifiedProperties();
            };
        }

        private void AddItemToContextMenu(Type type, GenericMenu genericMenuContext, SerializedProperty property)
        {
            var assemblyName =  type.Assembly.ToString().Split('(', ',')[0];
            var entryName = type + "  ( " + assemblyName + " )";
            genericMenuContext.AddItem(new GUIContent(entryName), false, AssignNewInstanceCommand, new GenericMenuParameterForAssignInstanceCommand(type, property));
        }

        private void AssignNewInstanceCommand(object objectGenericMenuParameter )
        {
            var parameter = (GenericMenuParameterForAssignInstanceCommand) objectGenericMenuParameter;
            var type = parameter.Type;
            var property = parameter.Property;
            AssignNewInstanceOfTypeToManagedReference(property, type);
        }

        private object AssignNewInstanceOfTypeToManagedReference(SerializedProperty serializedProperty, Type type)
        {
            var instance = Activator.CreateInstance(type);

            serializedProperty.serializedObject.Update();
            serializedProperty.managedReferenceValue = instance;
            serializedProperty.serializedObject.ApplyModifiedProperties();

            return instance;
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

        private IEnumerable<Type> GetAppropriateTypesForAssigningToManagedReference(SerializedProperty property, List<Func<Type, bool>> filters = null)
        {
            var fieldType = GetManagedReferenceFieldType(property);
            return GetAppropriateTypesForAssigningToManagedReference(fieldType, filters);
        }

        /// Gets real type of managed reference
        private Type GetManagedReferenceFieldType(SerializedProperty property)
        {
            var realPropertyType = GetRealTypeFromTypename(property.managedReferenceFieldTypename);
            if (realPropertyType != null)
                return realPropertyType;

            Debug.LogError($"Can not get field type of managed reference : {property.managedReferenceFieldTypename}");
            return null;
        }

        /// Gets real type of managed reference's field typeName
        private Type GetRealTypeFromTypename(string stringType)
        {
            var names = GetSplitNamesFromTypename(stringType);
            var realType = Type.GetType($"{names.ClassName}, {names.AssemblyName}");
            return realType;
        }

        private IEnumerable<Type> GetAppropriateTypesForAssigningToManagedReference(Type fieldType, List<Func<Type, bool>> filters = null)
        {
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
    }
}