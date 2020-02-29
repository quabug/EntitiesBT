using System;
using System.Reflection;
using EntitiesBT.Variable;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using UnityEngine;

namespace EntitiesBT.Components
{
    public static class VariableExtensions
    {
        delegate void AllocateDelegate<T>(BlobBuilder builder, ref BlobVariable<T> blobVariable, VariableProperty<T> variable) where T : struct;
        
        public static void AllocateVariable<T>(this BlobBuilder builder, ref BlobVariable<T> blobVariable, VariableProperty<T> variable) where T : struct
        {
            blobVariable.Source = variable.ValueSource;
            switch (variable.ValueSource)
            {
            case ValueSource.CustomConstant:
                builder.AllocateConstant(ref blobVariable, variable);
                break;
            case ValueSource.ScriptableObjectConstant:
                builder.AllocateScriptableObject(ref blobVariable, variable);
                break;
            case ValueSource.ComponentDynamic:
                builder.AllocateComponentDynamic(ref blobVariable, variable);
                break;
            case ValueSource.ComponentConstant:
                break;
            case ValueSource.NodeConstant:
                break;
            case ValueSource.NodeDynamic:
                break;
            default:
                throw new ArgumentOutOfRangeException();
            }
        }

        private static void AllocateComponentDynamic<T>(this BlobBuilder builder, ref BlobVariable<T> blobVariable, VariableProperty<T> variable) where T : struct
        {
            var (hash, offset, valueType) = Utility.GetTypeHashAndFieldOffset(variable.ComponentTypeName, variable.ComponentValueName);
            if (valueType != typeof(T) || hash == 0)
            {
                Debug.LogError($"ComponentVariable({variable.ComponentTypeName}.{variable.ComponentValueName}) is not valid, fallback to ConstantValue");
                builder.Allocate(ref blobVariable, variable.ConstantValue);
                return;
            }
            builder.Allocate(ref blobVariable, new DynamicComponentData{StableHash = hash, Offset = offset});
        }
        
        private static void AllocateComponentConstant<T>(this BlobBuilder builder, ref BlobVariable<T> blobVariable, VariableProperty<T> variable) where T : struct
        {
            var (hash, offset, valueType) = Utility.GetTypeHashAndFieldOffset(variable.ComponentTypeName, variable.ComponentValueName);
            if (valueType != typeof(T) || hash == 0)
            {
                Debug.LogError($"ComponentVariable({variable.ComponentTypeName}.{variable.ComponentValueName}) is not valid, fallback to ConstantValue");
                builder.Allocate(ref blobVariable, variable.ConstantValue);
                return;
            }
            builder.Allocate(ref blobVariable, new DynamicComponentData{StableHash = hash, Offset = offset});
        }

        private static void AllocateConstant<T>(this BlobBuilder builder, ref BlobVariable<T> blobVariable, VariableProperty<T> variable) where T : struct
        {
            builder.Allocate(ref blobVariable, variable.ConstantValue);
        }

        private static void AllocateScriptableObject<T>(this BlobBuilder builder, ref BlobVariable<T> blobVariable, VariableProperty<T> variable) where T : struct
        {
            FieldInfo fieldInfo = null;
            if (variable.ScriptableObject != null)
            {
                fieldInfo = variable.ScriptableObject.GetType()
                    .GetField(variable.ScriptableObjectValueName, VariableProperty.FIELD_BINDING_FLAGS)
                ;
            }

            if (fieldInfo == null || fieldInfo.FieldType != typeof(T))
            {
                Debug.LogError($"{variable.ScriptableObject.name}.{variable.ScriptableObjectValueName} is not valid, fallback to ConstantValue");
                builder.AllocateConstant(ref blobVariable, variable);
                return;
            }

            builder.Allocate(ref blobVariable, (T) fieldInfo.GetValue(variable.ScriptableObject));
        }

        private static void Allocate<TValue, TFallback>(this BlobBuilder builder, ref BlobVariable<TFallback> blob, TValue value)
            where TValue : struct
            where TFallback : struct
        {
            ref var blobPtr = ref UnsafeUtility.As<int, BlobPtr<TValue>>(ref blob.OffsetPtr);
            ref var blobValue = ref builder.Allocate(ref blobPtr);
            blobValue = value;
        }
    }
}
