using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Unity.Entities;

namespace EntitiesBT.Variable
{
    public static class Utility
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

        public static (ulong hash, int offset, Type valueType) GetTypeHashAndFieldOffset(string componentTypeName, string componentValueName)
        {
            _NAME_VALUE_MAP.Value.TryGetValue($"{componentTypeName}.{componentValueName}", out var result);
            return result;
        }
        
        public static (Type componentType, FieldInfo componentDataField) GetComponentDataType(ulong hash, int offset)
        {
            _VALUE_TYPE_MAP.Value.TryGetValue((hash, offset), out var result);
            return result;
        }
        //
        // public static VariableProperty<T> ToVariable<T>(this BlobVariable<T> blobVariable) where T : struct
        // {
        //     var variable = new VariableProperty<T>();
        //     variable.ValueSource = blobVariable.Source;
        //     switch (blobVariable.Source)
        //     {
        //     case ValueSource.Custom:
        //     case ValueSource.ConstantUnityComponent:
        //     case ValueSource.ConstantScriptableObject:
        //     case ValueSource.ConstantNode:
        //         variable.ConstantValue = blobVariable.ConstantData;
        //         break;
        //     case ValueSource.DynamicComponent:
        //         var component = blobVariable.ComponentData;
        //         var (componentType, fieldInfo) = GetComponentDataType(component.StableHash, component.Offset);
        //         variable.ComponentValue = $"{componentType.Name}.{fieldInfo.Name}";
        //         break;
        //     case ValueSource.DynamicScriptableObject:
        //         throw new NotImplementedException();
        //     case ValueSource.DynamicNode:
        //         throw new NotImplementedException();
        //     default:
        //         throw new ArgumentOutOfRangeException();
        //     }
        //     return variable;
        // }
    }
}
