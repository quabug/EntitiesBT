using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using EntitiesBT.Core;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Assertions;
using Hash128 = UnityEngine.Hash128;

namespace EntitiesBT.Entities
{
    [Serializable]
    public enum VariableValueSource
    {
        Constant
      , ConstantComponent
      , ConstantScriptableObject
      , ConstantNode
        
      , DynamicComponent
      , DynamicScriptableObject
      , DynamicNode
    }

    public struct DynamicComponentData
    {
        public ulong StableHash;
        public int Offset;
    }

    public struct DynamicScriptableObjectData
    {
        public Hash128 Id;
        public int Offset;
    }

    public struct DynamicNodeData
    {
        public int Index;
        public int Offset;
    }
    
    [Serializable]
    public struct Variable<T> where T : struct
    {
        // static Variable()
        // {
        //     Assert.IsFalse(typeof(T).IsZeroSizeStruct());
        // }
        
        public VariableValueSource ValueSource;
        public T ConstantValue;
        public string ComponentValue;
        public ScriptableObject ScriptableObject;
        public string ScriptableObjectValueName;
    }
    
    [StructLayout(LayoutKind.Explicit), MayOnlyLiveInBlobStorage, Serializable]
    public struct BlobVariable<T> where T : struct
    {
        [FieldOffset(0)] public VariableValueSource Source;
        [FieldOffset(4)] public BlobPtr<T> ConstantData;
        [FieldOffset(4)] public BlobPtr<DynamicComponentData> ComponentData;
        [FieldOffset(4)] public BlobPtr<DynamicScriptableObjectData> ScriptableObjectData;
        [FieldOffset(4)] public BlobPtr<DynamicNodeData> NodeData;

        public T GetData(int index, INodeBlob blob, IBlackboard bb)
        {
            switch (Source)
            {
            case VariableValueSource.Constant:
            case VariableValueSource.ConstantComponent:
            case VariableValueSource.ConstantScriptableObject:
            case VariableValueSource.ConstantNode:
                return ConstantData.Value;
            case VariableValueSource.DynamicComponent:
                return bb.GetData<T>(ComponentData.Value.StableHash, ComponentData.Value.Offset);
            case VariableValueSource.DynamicScriptableObject:
                throw new NotImplementedException();
            case VariableValueSource.DynamicNode:
                throw new NotImplementedException();
            default:
                throw new ArgumentOutOfRangeException();
            }
        }
        
        public ref T GetDataRef(int index, INodeBlob blob, IBlackboard bb)
        {
            switch (Source)
            {
            case VariableValueSource.Constant:
            case VariableValueSource.ConstantComponent:
            case VariableValueSource.ConstantScriptableObject:
            case VariableValueSource.ConstantNode:
                return ref ConstantData.Value;
            case VariableValueSource.DynamicComponent:
                return ref bb.GetDataRef<T>(ComponentData.Value.StableHash, ComponentData.Value.Offset);
            case VariableValueSource.DynamicScriptableObject:
                throw new NotImplementedException();
            case VariableValueSource.DynamicNode:
                throw new NotImplementedException();
            default:
                throw new ArgumentOutOfRangeException();
            }
        }
        
        public void SetData(int index, INodeBlob blob, IBlackboard bb, T value)
        {
            GetDataRef(index, blob, bb) = value;
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
            var variable = new Variable<T>();
            variable.ValueSource = blobVariable.Source;
            switch (blobVariable.Source)
            {
            case VariableValueSource.Constant:
            case VariableValueSource.ConstantComponent:
            case VariableValueSource.ConstantScriptableObject:
            case VariableValueSource.ConstantNode:
                variable.ConstantValue = blobVariable.ConstantData.Value;
                break;
            case VariableValueSource.DynamicComponent:
                var component = blobVariable.ComponentData.Value;
                var (componentType, fieldInfo) = GetComponentDataType(component.StableHash, component.Offset);
                variable.ComponentValue = $"{componentType.Name}.{fieldInfo.Name}";
                break;
            case VariableValueSource.DynamicScriptableObject:
                throw new NotImplementedException();
            case VariableValueSource.DynamicNode:
                throw new NotImplementedException();
            default:
                throw new ArgumentOutOfRangeException();
            }
            return variable;
        }
    }
}
