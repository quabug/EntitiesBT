using System;
using System.Reflection;
using EntitiesBT.Variable;
using Unity.Entities;
using UnityEngine;

namespace EntitiesBT.Components
{
    [Serializable]
    public class ScriptableObjectVariableProperty<T> : VariableProperty<T> where T : struct
    {
        public T FallbackValue;
        public ScriptableObject ScriptableObject;
        public string ScriptableObjectValueName;

        public override void Allocate(ref BlobBuilder builder, ref BlobVariable<T> blobVariable)
        {
            blobVariable.VariableId = CustomVariableProperty<T>.ID;
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
    }
}
