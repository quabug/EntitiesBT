using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using EntitiesBT.Core;
using JetBrains.Annotations;
using Unity.Assertions;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;

namespace EntitiesBT.Variant
{
    public static class Utilities
    {
        public class ComponentFieldData
        {
            public readonly Type ComponentType;
            public readonly FieldInfo FieldInfo;
            public readonly ulong Hash;
            public readonly int Offset;
            public string Name => FieldInfo == null ? ComponentType.FullName : $"{ComponentType.FullName}.{FieldInfo.Name}";
            public Type Type => FieldInfo == null ? ComponentType : FieldInfo.FieldType;

            public ComponentFieldData(Type componentType, FieldInfo fieldInfo, ulong hash, int offset)
            {
                ComponentType = componentType;
                FieldInfo = fieldInfo;
                Hash = hash;
                Offset = offset;
            }
        }
        
        private static readonly Lazy<ComponentFieldData[]> _COMPONENT_FIELDS = new Lazy<ComponentFieldData[]>(() =>
        {
            var types =
                from type in typeof(Entity).Assembly.GetTypesIncludeReference()
                where type.IsValueType && typeof(IComponentData).IsAssignableFrom(type)
                select (type, hash: TypeHash.CalculateStableTypeHash(type))
            ;

            return Result().ToArray();
            
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
        
        public static void Allocate<TValue>(this BlobBuilder builder, ref BlobVariant blob, TValue value) where TValue : struct
        {
            ref var blobPtr = ref ToBlobPtr<TValue>(ref blob.MetaDataOffsetPtr);
            ref var blobValue = ref builder.Allocate(ref blobPtr);
            blobValue = value;
        }

        public static void Allocate<T>(
            this IVariant property
          , ref BlobBuilder builder
          , ref BlobVariantReader<T> blobVariant
          , [NotNull] INodeDataBuilder self
          , [NotNull] ITreeNode<INodeDataBuilder>[] tree
        ) where T : unmanaged => property.Allocate(ref builder, ref blobVariant.Value, self, tree);

        public static void Allocate<T>(
            this IVariant property
          , ref BlobBuilder builder
          , ref BlobVariantWriter<T> blobVariant
          , [NotNull] INodeDataBuilder self
          , [NotNull] ITreeNode<INodeDataBuilder>[] tree
        ) where T : unmanaged => property.Allocate(ref builder, ref blobVariant.Value, self, tree);

        public static ref BlobPtr<T> ToBlobPtr<T>(ref int offsetPtr) where T : struct
        {
            return ref UnsafeUtility.As<int, BlobPtr<T>>(ref offsetPtr);
        }

        public static unsafe void Allocate<T>(
            this IVariantReader<T> variant
          , ref BlobBuilder builder
          , void* blobVariantPtr
          , [NotNull] INodeDataBuilder self
          , [NotNull] ITreeNode<INodeDataBuilder>[] tree
        ) where T : unmanaged
        {
            variant.Allocate(ref builder, ref UnsafeUtility.AsRef<BlobVariant>(blobVariantPtr), self, tree);
        }

        public static MethodInfo GetVariantMethodInfo(Type type, string name)
        {
            var methodInfo = type.GetMethod(name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            Assert.IsTrue(methodInfo.IsGenericMethod);
            Assert.AreEqual(2, methodInfo.GetGenericArguments().Length);
            return methodInfo;
        }

        // TODO: move to reader extension?
        public static readonly Lazy<IReadOnlyCollection<Type>> VARIANT_READER_TYPES =
            new Lazy<IReadOnlyCollection<Type>>(GetVariantTypes(typeof(IVariantReader<>)));

        // TODO: move to reader extension?
        public static readonly Lazy<IReadOnlyCollection<Type>> VARIANT_WRITER_TYPES =
            new Lazy<IReadOnlyCollection<Type>>(GetVariantTypes(typeof(IVariantWriter<>)));

        private static Func<IReadOnlyCollection<Type>> GetVariantTypes(Type @interface)
        {
            return () =>
            {
                var variableAssembly = @interface.Assembly;
                var variableAssemblyName = variableAssembly.GetName().Name;
                return AppDomain
                    .CurrentDomain
                    .GetAssemblies()
                    .Where(assembly => assembly.GetReferencedAssemblies().Any(name => name.Name == variableAssemblyName))
                    .Append(variableAssembly)
                    .SelectMany(assembly => assembly.GetTypesWithoutException())
                    .Where(type => !type.IsAbstract
                                   && type.IsGenericType
                                   && type.GetGenericArguments().Length == 1
                                   && @interface.IsAssignableFromGeneric(type))
                    .ToArray()
                ;
            };
        }

        public static bool IsReader(Type type) => typeof(IVariantReader<>).IsAssignableFromGeneric(type);
        public static bool IsWriter(Type type) => typeof(IVariantWriter<>).IsAssignableFromGeneric(type);
    }
}
