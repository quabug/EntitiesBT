using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using UnityEngine;

namespace Nuwa.Blob
{
    public interface IDynamicBuilderFactory
    {
        public int Order { get; }
        public Type BuilderType { get; }
        public bool IsValid(Type dataType, FieldInfo fieldInfo);
        public object Create(Type dataType, FieldInfo fieldInfo);
    }

    public abstract class DynamicBuilderFactory<T> : IDynamicBuilderFactory where T : IBuilder
    {
        public virtual int Order => 0;
        public Type BuilderType => typeof(T);
        public abstract bool IsValid(Type dataType, FieldInfo fieldInfo);
        public abstract object Create(Type dataType, FieldInfo fieldInfo);
    }

    [Serializable]
    public class DynamicEnumBuilder<T> : IBuilder where T : unmanaged
    {
        public string EnumTypeName;
        public T Value;

        public unsafe void Build(BlobBuilder builder, IntPtr dataPtr)
        {
            UnsafeUtility.AsRef<T>(dataPtr.ToPointer()) = Value;
        }

        public class Factory<U> : DynamicBuilderFactory<U> where U : DynamicEnumBuilder<T>, new()
        {
            public override bool IsValid(Type dataType, FieldInfo fieldInfo)
            {
                return dataType.IsEnum && Enum.GetUnderlyingType(dataType) == typeof(T);
            }

            public override object Create(Type dataType, FieldInfo fieldInfo)
            {
                return new U { EnumTypeName = dataType.AssemblyQualifiedName };
            }
        }
    }

    public class DynamicByteEnumBuilder : DynamicEnumBuilder<byte>
    {
        public class Factory : Factory<DynamicByteEnumBuilder> {}
    }

    public class DynamicSByteEnumBuilder : DynamicEnumBuilder<sbyte>
    {
        public class Factory : Factory<DynamicSByteEnumBuilder> {}
    }

    public class DynamicShortEnumBuilder : DynamicEnumBuilder<short>
    {
        public class Factory : Factory<DynamicShortEnumBuilder> {}
    }

    public class DynamicUShortEnumBuilder : DynamicEnumBuilder<ushort>
    {
        public class Factory : Factory<DynamicUShortEnumBuilder> {}
    }

    public class DynamicIntEnumBuilder : DynamicEnumBuilder<int>
    {
        public class Factory : Factory<DynamicIntEnumBuilder> {}
    }

    public class DynamicUIntEnumBuilder : DynamicEnumBuilder<uint>
    {
        public class Factory : Factory<DynamicUIntEnumBuilder> {}
    }

    public class DynamicLongEnumBuilder : DynamicEnumBuilder<long>
    {
        public class Factory : Factory<DynamicLongEnumBuilder> {}
    }

    public class DynamicULongEnumBuilder : DynamicEnumBuilder<ulong>
    {
        public class Factory : Factory<DynamicULongEnumBuilder> {}
    }

    [Serializable]
    public class DynamicBlobDataBuilder : IBuilder, IObjectBuilder
    {
        public string BlobDataType;
        public string[] FieldNames;
        [SerializeReference, UnboxSingleProperty, UnityDrawProperty] public IBuilder[] Builders;

        public void Build(BlobBuilder builder, IntPtr dataPtr)
        {
            var fields = Type.GetType(BlobDataType).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            for (var i = 0; i < Builders.Length; i++)
            {
                var offset = UnsafeUtility.GetFieldOffset(fields[i]);
                Builders[i].Build(builder, dataPtr + offset);
            }
        }

        public IReadOnlyList<string> GetFieldNames() => FieldNames;
        public IReadOnlyList<IBuilder> GetBuilders() => Builders;

        public class Factory : DynamicBuilderFactory<DynamicBlobDataBuilder>
        {
            public override int Order => 1000;
            public override bool IsValid(Type dataType, FieldInfo fieldInfo) => !dataType.IsEnum && !dataType.IsPrimitive;
            public override object Create(Type dataType, FieldInfo fieldInfo) => new DynamicBlobDataBuilder { BlobDataType = dataType.AssemblyQualifiedName };
        }
    }

    [Serializable]
    public class DynamicArrayBuilder : IBuilder
    {
        public string ArrayElementType;
        [SerializeReference, UnboxSingleProperty, UnityDrawProperty] public IBuilder[] Value;

        public void Build(BlobBuilder builder, IntPtr dataPtr)
        {
            var elementType = Type.GetType(ArrayElementType);
            var arrayBuilder = builder.AllocateDynamicArray(elementType, dataPtr, Value.Length);
            for (var i = 0; i < Value.Length; i++) Value[i].Build(builder, arrayBuilder[i]);
        }

        public class Factory : DynamicBuilderFactory<DynamicArrayBuilder>
        {
            public override int Order => 100;
            public override bool IsValid(Type dataType, FieldInfo fieldInfo) => dataType.IsGenericType && dataType.GetGenericTypeDefinition() == typeof(BlobArray<>);
            public override object Create(Type dataType, FieldInfo fieldInfo) => new DynamicArrayBuilder { ArrayElementType = dataType.GenericTypeArguments[0].AssemblyQualifiedName };
        }
    }

    [Serializable]
    public class DynamicPtrBuilder : IBuilder
    {
        public string DataType;
        [SerializeReference, UnboxSingleProperty, UnityDrawProperty] public IBuilder Value;

        public void Build(BlobBuilder builder, IntPtr dataPtr)
        {
            var blobPtr = builder.AllocateDynamicPtr(Type.GetType(DataType), dataPtr);
            Value.Build(builder, blobPtr);
        }

        public class Factory : DynamicBuilderFactory<DynamicPtrBuilder>
        {
            public override int Order => 100;
            public override bool IsValid(Type dataType, FieldInfo fieldInfo) => dataType.IsGenericType && dataType.GetGenericTypeDefinition() == typeof(BlobPtr<>);
            public override object Create(Type dataType, FieldInfo fieldInfo) => new DynamicPtrBuilder { DataType = dataType.GenericTypeArguments[0].AssemblyQualifiedName };
        }
    }
}