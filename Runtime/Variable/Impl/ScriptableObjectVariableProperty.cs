using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EntitiesBT.Core;
using Unity.Entities;
using UnityEngine;

namespace EntitiesBT.Variable
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class VariableScriptableObjectValueAttribute : PropertyAttribute
    {
        public string ScriptableObjectFieldName;

        public VariableScriptableObjectValueAttribute(string scriptableObjectFieldName)
        {
            ScriptableObjectFieldName = scriptableObjectFieldName;
        }
    }
    
    [Serializable]
    public class ScriptableObjectVariableProperty<T> : VariableProperty<T> where T : struct
    {
        public override int VariablePropertyTypeId => ID;
        
        public ScriptableObject ScriptableObject;
        
#if ODIN_INSPECTOR
        IEnumerable<string> _validValueNames
        {
            get
            {
                if (ScriptableObject == null) return Enumerable.Empty<string>();

                var type = ScriptableObject.GetType();
                var validFields = type
                    .GetFields(FIELD_BINDING_FLAGS)
                    .Where(fi => fi.FieldType == typeof(T))
                    .Select(fi => fi.Name)
                ;
                var validProperties = type
                    .GetProperties(FIELD_BINDING_FLAGS)
                    .Where(pi => pi.CanRead && pi.PropertyType == typeof(T))
                    .Select(pi => pi.Name)
                ;
                
                return validFields.Concat(validProperties);
            }
        }
        
        [Sirenix.OdinInspector.ValueDropdown("_validValueNames")]
#else
        [VariableScriptableObjectValue("ScriptableObject")]
#endif
        public string ScriptableObjectValueName;
        
        protected override void AllocateData(ref BlobBuilder builder, ref BlobVariable<T> blobVariable, INodeDataBuilder self, ITreeNode<INodeDataBuilder>[] tree)
        {
            var type = ScriptableObject.GetType();
            FieldInfo fieldInfo = null;
            PropertyInfo propertyInfo = null;
            if (ScriptableObject != null)
                fieldInfo = type.GetField(ScriptableObjectValueName, FIELD_BINDING_FLAGS);
            if (fieldInfo == null)
                propertyInfo = type.GetProperty(ScriptableObjectValueName, FIELD_BINDING_FLAGS);

            if ((fieldInfo == null || fieldInfo.FieldType != typeof(T))
                && (propertyInfo == null || !propertyInfo.CanRead || propertyInfo.PropertyType != typeof(T)))
            {
                Debug.LogError($"{ScriptableObject.name}.{ScriptableObjectValueName} is not valid");
                throw new ArgumentException();
            }

            var value = fieldInfo?.GetValue(ScriptableObject) ?? propertyInfo?.GetValue(ScriptableObject);
            builder.Allocate(ref blobVariable, (T) value);
        }

        static ScriptableObjectVariableProperty()
        {
            VariableRegisters<T>.Register(ID, GetData, GetDataRef);
        }

        public static readonly int ID = new Guid("B3668D2B-DC57-45F9-B71C-BF578E3EEF88").GetHashCode();
        
        private static T GetData(ref BlobVariable<T> blobVariable, int index, INodeBlob blob, IBlackboard bb)
        {
            return blobVariable.Value<T>();
        }
        
        private static ref T GetDataRef(ref BlobVariable<T> blobVariable, int index, INodeBlob blob, IBlackboard bb)
        {
            return ref blobVariable.Value<T>();
        }
    }
}
