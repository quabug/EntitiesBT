using System;
using System.Reflection;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using UnityEngine;

namespace EntitiesBT.Variable
{
    public static class VariableExtensions
    {
        public static void AllocateVariable<T>(this BlobBuilder builder, ref BlobVariable<T> blobVariable, VariableProperty<T> variable) where T : struct
        {
            blobVariable.Source = variable.ValueSource;
            switch (variable.ValueSource)
            {
            case ValueSource.Constant:
            {
                Allocate(ref blobVariable, variable.ConstantValue);
                break;
            }
            case ValueSource.ConstantScriptableObject:
            {
                var value = variable.ConstantValue;
                if (variable.ScriptableObject != null)
                {
                    var field = variable.ScriptableObject.GetType().GetField(
                        variable.ScriptableObjectValueName, BindingFlags.Public | BindingFlags.NonPublic
                    );
                    if (field != null && field.FieldType == typeof(T))
                        value = (T)field.GetValue(variable.ScriptableObjectValueName);
                    else
                        Debug.LogError($"{variable.ScriptableObject.name}.{variable.ScriptableObjectValueName} is not valid, fallback to ConstantValue");
                }
                Allocate(ref blobVariable, value);
                break;
            }
            case ValueSource.DynamicComponent:
            {
                var (hash, offset, valueType) = Utility.GetTypeHashAndFieldOffset(variable.ComponentValue);
                if (valueType != typeof(T) || hash == 0)
                {
                    Debug.LogError($"ComponentVariable({variable.ComponentValue}) is not valid, fallback to ConstantValue");
                    Allocate(ref blobVariable, variable.ConstantValue);
                    break;
                }
                Allocate(ref blobVariable, new DynamicComponentData{StableHash = hash, Offset = offset});
                break;
            }
            case ValueSource.ConstantComponent:
            case ValueSource.ConstantNode:
            case ValueSource.DynamicScriptableObject:
            case ValueSource.DynamicNode:
                throw new NotImplementedException();
            default:
                throw new ArgumentOutOfRangeException();
            }

            void Allocate<TValue>(ref BlobVariable<T> blob, TValue value) where TValue : struct
            {
                ref var blobPtr = ref UnsafeUtility.As<int, BlobPtr<TValue>>(ref blob.OffsetPtr);
                ref var blobValue = ref builder.Allocate(ref blobPtr);
                blobValue = value;
            }
        }
    }
}
