using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using static EntitiesBT.Core.Utilities;

namespace EntitiesBT.Variable
{
    public static class Utility
    {
        public class ComponentFieldData
        {
            public readonly Type ComponentType;
            public readonly FieldInfo FieldInfo;
            public readonly ulong Hash;
            public readonly int Offset;
            public string Name => FieldInfo == null ? ComponentType.Name : $"{ComponentType.Name}.{FieldInfo.Name}";
            public Type Type => FieldInfo == null ? ComponentType : FieldInfo.FieldType;

            public ComponentFieldData(Type componentType, FieldInfo fieldInfo, ulong hash, int offset)
            {
                ComponentType = componentType;
                FieldInfo = fieldInfo;
                Hash = hash;
                Offset = offset;
            }
        }
        
        private static readonly Lazy<IEnumerable<ComponentFieldData>> _COMPONENT_FIELDS = new Lazy<IEnumerable<ComponentFieldData>>(() =>
        {
            var types =
                from type in ValidAssemblyTypes
                where type.IsValueType && typeof(IComponentData).IsAssignableFrom(type)
                select (type, hash: TypeHash.CalculateStableTypeHash(type))
            ;

            return Result();
            
            IEnumerable<ComponentFieldData> Result()
            {
                foreach (var (type, hash) in types)
                {
                    yield return new ComponentFieldData(type, null, hash, 0);
                    foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public))
                        if (!field.IsLiteral && !field.IsStatic)
                            yield return new ComponentFieldData(type, field, hash, Marshal.OffsetOf(type, field.Name).ToInt32());
                }
            }

        });
        
        private static readonly Lazy<Dictionary<string, ComponentFieldData>> _NAME_VALUE_MAP =
            new Lazy<Dictionary<string, ComponentFieldData>>(
                () => _COMPONENT_FIELDS.Value.ToDictionary(t => t.Name, t => t)
            );
        
        private static readonly Lazy<ILookup<Type, ComponentFieldData>> _VALUE_TYPE_LOOKUP =
            new Lazy<ILookup<Type, ComponentFieldData>>(
                () => _COMPONENT_FIELDS.Value.ToLookup(t => t.Type, t => t)
            );

        public static ComponentFieldData GetTypeHashAndFieldOffset(string componentValue)
        {
            _NAME_VALUE_MAP.Value.TryGetValue(componentValue, out var result);
            return result;
        }

        public static IEnumerable<ComponentFieldData> GetComponentFields(Type valueType)
        {
            return _VALUE_TYPE_LOOKUP.Value[valueType];
        }
        
        public static void Allocate<TValue, TFallback>(this BlobBuilder builder, ref BlobVariable<TFallback> blob, TValue value)
            where TValue : struct
            where TFallback : struct
        {
            ref var blobPtr = ref UnsafeUtilityEx.As<int, BlobPtr<TValue>>(ref blob.OffsetPtr);
            ref var blobValue = ref builder.Allocate(ref blobPtr);
            blobValue = value;
        }
    }
}
