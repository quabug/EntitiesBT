using System;
using System.Reflection;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using UnityEngine;

namespace Nuwa.Blob
{
    public interface IBuilder
    {
        void Build(BlobBuilder builder, IntPtr dataPtr);
    }

    public abstract class Builder<T> : IBuilder where T : unmanaged
    {
        public unsafe void Build(BlobBuilder builder, IntPtr dataPtr)
        {
            Build(builder, ref UnsafeUtility.AsRef<T>(dataPtr.ToPointer()));
        }

        public abstract void Build(BlobBuilder builder, ref T data);
    }

    /// <summary>
    /// Builder of POD.
    /// Show the POD as whole in inspector.
    /// Not support data with its own builder, e.g. `BlobPtr`, `BlobArray` and `BlobString`.
    /// </summary>
    /// <typeparam name="T">type of POD</typeparam>
    [Serializable]
    public class PlainDataBuilder<T> : Builder<T> where T : unmanaged
    {
        public T Value;

        public override void Build(BlobBuilder builder, ref T data)
        {
            data = Value;
        }
    }

    /// <summary>
    /// Builder of structure with `Blob` data inside.
    /// Split each data inside structure into its own builder to show and edit.
    /// Support data with <seealso cref="DefaultBuilderAttribute"/> and <seealso cref="CustomBuilderAttribute"/>, e.g. `BlobPtr`, `BlobArray` and `BlobString`
    /// </summary>
    /// <typeparam name="T">type of blob structure</typeparam>
    [Serializable]
    public class BlobDataBuilder<T> : Builder<T> where T : unmanaged
    {
        [HideInInspector] public string[] FieldNames;
        [SerializeReference, UnboxSingleProperty, UnityDrawProperty] public IBuilder[] Builders;

        public override unsafe void Build(BlobBuilder builder, ref T data)
        {
            var dataPtr = new IntPtr(UnsafeUtility.AddressOf(ref data));
            var fields = typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            for (var i = 0; i < Builders.Length; i++)
            {
                var offset = UnsafeUtility.GetFieldOffset(fields[i]);
                Builders[i].Build(builder, dataPtr + offset);
            }
        }
    }

    [Serializable]
    public class ArrayBuilder<T> : Builder<BlobArray<T>> where T : unmanaged
    {
        [SerializeReference, UnboxSingleProperty, UnityDrawProperty] public IBuilder[] Value;

        public override void Build(BlobBuilder builder, ref BlobArray<T> data)
        {
            var arrayBuilder = builder.Allocate(ref data, Value.Length);
            for (var i = 0; i < Value.Length; i++) ((Builder<T>)Value[i]).Build(builder, ref arrayBuilder[i]);
        }
    }

    [Serializable, DefaultBuilder]
    public class StringBuilder : Builder<BlobString>
    {
        public string Value;

        public override void Build(BlobBuilder builder, ref BlobString data)
        {
            builder.AllocateString(ref data, Value);
        }
    }

    [DefaultBuilder] public class StringArrayBuilder : ArrayBuilder<BlobString> {}
    [DefaultBuilder] public class StringPtrBuilder : PtrBuilder<BlobString> {}

    [Serializable]
    public class PtrBuilder<T> : Builder<BlobPtr<T>> where T : unmanaged
    {
        [SerializeReference, UnboxSingleProperty, UnityDrawProperty] public IBuilder Value;

        public override unsafe void Build(BlobBuilder builder, ref BlobPtr<T> data)
        {
            ref var value = ref builder.Allocate(ref data);
            Value.Build(builder, new IntPtr(UnsafeUtility.AddressOf(ref value)));
        }
    }
}