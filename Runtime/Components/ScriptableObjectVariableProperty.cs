using System;
using System.Reflection;
using EntitiesBT.Core;
using EntitiesBT.Variable;
using Unity.Entities;
using UnityEngine;

namespace EntitiesBT.Components
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
        public T FallbackValue;
        public ScriptableObject ScriptableObject;
        [VariableScriptableObjectValue("ScriptableObject")]
        public string ScriptableObjectValueName;

        protected override void AllocateData(ref BlobBuilder builder, ref BlobVariable<T> blobVariable, INodeDataBuilder self, ITreeNode<INodeDataBuilder>[] tree)
        {
            FieldInfo fieldInfo = null;
            if (ScriptableObject != null)
                fieldInfo = ScriptableObject.GetType().GetField(ScriptableObjectValueName, FIELD_BINDING_FLAGS);

            if (fieldInfo == null || fieldInfo.FieldType != typeof(T))
            {
                Debug.LogError($"{ScriptableObject.name}.{ScriptableObjectValueName} is not valid, fallback to ConstantValue");
                builder.Allocate(ref blobVariable, FallbackValue);
                return;
            }

            builder.Allocate(ref blobVariable, (T) fieldInfo.GetValue(ScriptableObject));
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
