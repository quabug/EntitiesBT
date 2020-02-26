using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using EntitiesBT.Core;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Assertions;

namespace EntitiesBT.Entities
{
    [Serializable]
    public enum VariableValueSource
    {
        CustomValue
      , ComponentValue
      , ScriptableObjectValue
    }
        
    [Serializable]
    public struct Variable<T> where T : struct
    {
        static Variable()
        {
            Assert.AreNotEqual(DataSize, 0);
        }
        
        public static int DataSize => UnsafeUtility.SizeOf<T>();
        
        public VariableValueSource ValueSource;
        public T CustomValue;
        public string ComponentValue;
        public T FallbackValue;
        public ScriptableObject ConfigSource;
        public string ConfigValueName;

        public int BlobSize => ValueSource == VariableValueSource.ComponentValue ? 16 : 4 + UnsafeUtility.SizeOf<T>();
    }
    
    [StructLayout(LayoutKind.Explicit), MayOnlyLiveInBlobStorage, Serializable]
    public unsafe struct BlobVariable
    {
        [FieldOffset(0), SerializeField] private bool _isCustomVariable;
        [FieldOffset(4)] private int _componentDataOffset;
        [FieldOffset(8)] private ulong _componentStableHash;
        
        [FieldOffset(0), SerializeField] private int _dataSize;
        private void* _dataPtr => UnsafeUtility.AddressOf(ref _componentDataOffset);

        public void FromVariableUnsafe<T>(Variable<T> variable) where T : struct
        {
            switch (variable.ValueSource)
            {
            case VariableValueSource.CustomValue:
            {
                SetCustomVariable(variable.CustomValue);
                break;
            }
            case VariableValueSource.ComponentValue:
            {
                var (hash, offset, valueType) = Variable.GetTypeHashAndFieldOffset(variable.ComponentValue);
                if (valueType != typeof(T) || hash == 0)
                {
                    Debug.LogError($"ComponentVariable({variable.ComponentValue}) is not valid, fallback to CustomVariable");
                    // fallback to custom variable
                    SetCustomVariable(variable.FallbackValue);
                    break;
                }
                _isCustomVariable = false;
                _componentStableHash = hash;
                _componentDataOffset = offset;
                break;
            }
            case VariableValueSource.ScriptableObjectValue:
            {
                var value = variable.FallbackValue;
                if (variable.ConfigSource != null)
                {
                    var field = variable.ConfigSource.GetType().GetField(
                        variable.ConfigValueName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    ;
                    if (field != null && field.FieldType == typeof(T))
                        value = (T)field.GetValue(variable.ConfigSource);
                }
                SetCustomVariable(value);
                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
            }
        }

        void SetCustomVariable<T>(T value) where T : struct
        {
            _isCustomVariable = true;
            _dataSize = UnsafeUtility.SizeOf<T>();
            UnsafeUtilityEx.AsRef<T>(_dataPtr) = value;
        }

        public Variable<T> ToVariable<T>() where T : struct
        {
            if (_isCustomVariable)
            {
                return new Variable<T>
                {
                    ValueSource = VariableValueSource.CustomValue
                  , CustomValue = UnsafeUtilityEx.AsRef<T>(_dataPtr)
                };
            }

            var (componentType, fieldInfo) = Variable.GetComponentDataType(_componentStableHash, _componentDataOffset);
            return new Variable<T>
            {
                ValueSource = VariableValueSource.ComponentValue
              , ComponentValue = $"{componentType.Name}.{fieldInfo.Name}"
            };
        }

        public ref T GetData<T>(IBlackboard bb) where T : struct
        {
            if (_isCustomVariable) return ref UnsafeUtilityEx.AsRef<T>(_dataPtr); // TODO: check size
            return ref bb.GetDataRef<T>(_componentStableHash, _componentDataOffset);
        }
        
        public void SetData<T>(IBlackboard bb, T value) where T : struct
        {
            if (_isCustomVariable) UnsafeUtilityEx.AsRef<T>(_dataPtr) = value; // TODO: check size
            else bb.GetDataRef<T>(_componentStableHash, _componentDataOffset) = value;
        }
    }

    public static class Variable
    {
        private static Lazy<IEnumerable<(Type componentType, FieldInfo field, ulong hash, int offset)>> _VALUES =
            new Lazy<IEnumerable<(Type componentType, FieldInfo field, ulong hash, int offset)>>(() =>
            {
                var types =
                    from assembly in AppDomain.CurrentDomain.GetAssemblies()
                    from type in assembly.GetTypes()
                    where type.IsValueType && typeof(IComponentData).IsAssignableFrom(type)
                    select (type, hash: TypeHash.CalculateStableTypeHash(type))
                ;
                
                return
                    from t in types
                    from field in t.type.GetFields(BindingFlags.Instance | BindingFlags.Public)
                    where !field.IsLiteral && !field.IsStatic
                    select (t.type, field, t.hash, offset: Marshal.OffsetOf(t.type, field.Name).ToInt32())
                ;
            });
        
        private static readonly Lazy<Dictionary<string, (ulong hash, int offset, Type valueType)>> _NAME_VALUE_MAP =
            new Lazy<Dictionary<string, (ulong hash, int offset, Type valueType)>>(() =>
            {
                return _VALUES.Value.ToDictionary(t => $"{t.componentType.Name}.{t.field.Name}", t => (t.hash, t.offset, t.field.FieldType));
            });
        
        private static readonly Lazy<Dictionary<(ulong hash, int offset), (Type componentType, FieldInfo componentDataField)>> _VALUE_TYPE_MAP =
            new Lazy<Dictionary<(ulong hash, int offset), (Type componentType, FieldInfo componentDataField)>>(() =>
            {
                return _VALUES.Value.ToDictionary(t => (t.hash, t.offset), t => (t.componentType, t.field));
            });

        public static (ulong hash, int offset, Type valueType) GetTypeHashAndFieldOffset(string componentDataName)
        {
            _NAME_VALUE_MAP.Value.TryGetValue(componentDataName, out var result);
            return result;
        }
        
        public static (Type componentType, FieldInfo componentDataField) GetComponentDataType(ulong hash, int offset)
        {
            _VALUE_TYPE_MAP.Value.TryGetValue((hash, offset), out var result);
            return result;
        }
    }
}
