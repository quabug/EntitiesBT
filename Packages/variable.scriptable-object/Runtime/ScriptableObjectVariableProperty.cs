using System;
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
    public class ScriptableObjectVariablePropertyReader<T> : VariablePropertyReader<T> where T : unmanaged
    {
        public override int VariablePropertyTypeId => ID;
        
        public ScriptableObject ScriptableObject;
        
        [VariableScriptableObjectValue("ScriptableObject")]
        public string ScriptableObjectValueName;
        
        protected override void AllocateData(ref BlobBuilder builder, ref BlobVariableReader<T> blobVariable, INodeDataBuilder self, ITreeNode<INodeDataBuilder>[] tree)
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

        static ScriptableObjectVariablePropertyReader()
        {
            var type = typeof(ScriptableObjectVariablePropertyReader<T>);
            VariableReaderRegisters<T>.Register(ID, type.Getter("GetData"));
        }

        public static readonly int ID = new Guid("B3668D2B-DC57-45F9-B71C-BF578E3EEF88").GetHashCode();
        
        private static T GetData<TNodeBlob, TBlackboard>(ref BlobVariableReader<T> blobVariable, int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            return blobVariable.Value<T>();
        }
        
        private static ref T GetDataRef<TNodeBlob, TBlackboard>(ref BlobVariableReader<T> blobVariable, int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            return ref blobVariable.Value<T>();
        }
    }
}
