#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
 
// possibly try to Migrate from local functions to normal private functions
public static class SerializeReferenceGenericSelectionMenu
{
    /// Purpose.
    /// This is generic selection menu.
    /// Filtering. 
    /// You can add substring filter here to filter by search string.
    /// As well ass type or interface restrictions.
    /// As well as any custom restriction that is based on input type.
    /// And it will be performed on each Appropriate type found by TypeCache.
    public static void ShowContextMenuForManagedReference(this SerializedProperty property, IEnumerable<Func<Type,bool>> filters = null)
    { 
        var context = new GenericMenu();
        FillContextMenu();
        context.ShowAsContext();

        void FillContextMenu() 
        {
            context.AddItem(new GUIContent("Null"), false, MakeNull);
            var realPropertyType = SerializeReferenceTypeNameUtility.GetRealTypeFromTypename(property.managedReferenceFieldTypename);
            if (realPropertyType == null)
            { 
                Debug.LogError("Can not get type from");
                return;
            }
             
            var types = TypeCache.GetTypesDerivedFrom(realPropertyType);
            foreach (var type in types)
            { 
                // Skips unity engine Objects (because they are not serialized by SerializeReference)
                if(type.IsSubclassOf(typeof(UnityEngine.Object)))
                    continue;
                // Skip abstract classes because they should not be instantiated
                if(type.IsAbstract) 
                    continue;   
                if (FilterTypeByFilters(filters, type) == false)  
                    continue; 
                
                AddContextMenu(type, context); 
            }
    
            void MakeNull()
            {
                property.serializedObject.Update();
                property.managedReferenceValue = null;
                property.serializedObject.ApplyModifiedPropertiesWithoutUndo(); // undo is bugged for now
            }

            void AddContextMenu(Type type, GenericMenu genericMenuContext)
            {
                var assemblyName =  type.Assembly.ToString().Split('(', ',')[0];
                var entryName = type.Name + "  ( " + assemblyName + " )";
                genericMenuContext.AddItem(new GUIContent(entryName), false, AssignNewInstanceOfType, type);
            }
            
            void AssignNewInstanceOfType(object typeAsObject)
            {
                var type = (Type) typeAsObject;
                var instance = Activator.CreateInstance(type); 
                property.serializedObject.Update();
                property.managedReferenceValue = instance;
                property.serializedObject.ApplyModifiedPropertiesWithoutUndo(); // undo is bugged for now
            }   
 
            bool FilterTypeByFilters (IEnumerable<Func<Type,bool>> filters_, Type type) =>
                filters_.All(f => f.Invoke(type));  
        } 
    }     
} 

#endif