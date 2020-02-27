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
      //, NodeValue
    }
    
    [Serializable]
    public struct Variable<T> where T : struct
    {
        static Variable()
        {
            Assert.IsFalse(typeof(T).IsZeroSizeStruct());
        }
        
        public VariableValueSource ValueSource;
        public T CustomValue;
        public string ComponentValue;
        public ScriptableObject ConfigSource;
        public string ConfigValueName;
    }
    
    [StructLayout(LayoutKind.Explicit), MayOnlyLiveInBlobStorage, Serializable]
    public struct BlobVariable<T> where T : struct
    {
        [FieldOffset(0), SerializeField] public bool IsCustomVariable;
        [FieldOffset(4)] public int ComponentDataOffset;
        [FieldOffset(8)] public ulong ComponentStableHash;
        [FieldOffset(0), SerializeField] public BlobPtr<T> CustomData;

        public ref T GetData(IBlackboard bb)
        {
            if (IsCustomVariable) return ref CustomData.Value;
            else return ref bb.GetDataRef<T>(ComponentStableHash, ComponentDataOffset);
        }
        
        public void SetData(IBlackboard bb, T value)
        {
            if (IsCustomVariable) CustomData.Value = value;
            else bb.GetDataRef<T>(ComponentStableHash, ComponentDataOffset) = value;
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

        public static Variable<T> ToVariable<T>(this BlobVariable<T> blobVariable) where T : struct
        {
            if (blobVariable.IsCustomVariable)
            {
                return new Variable<T>
                {
                    ValueSource = VariableValueSource.CustomValue
                  , CustomValue = blobVariable.CustomData.Value
                };
            }

            var (componentType, fieldInfo) = 
                GetComponentDataType(blobVariable.ComponentStableHash, blobVariable.ComponentDataOffset);
            return new Variable<T>
            {
                ValueSource = VariableValueSource.ComponentValue
              , ComponentValue = $"{componentType.Name}.{fieldInfo.Name}"
            };
        }
    }
}
