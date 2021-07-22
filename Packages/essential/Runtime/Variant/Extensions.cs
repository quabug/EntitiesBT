using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using EntitiesBT.Core;
using JetBrains.Annotations;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Debug = UnityEngine.Debug;

namespace EntitiesBT.Variant
{
    public static class BlobVariantExtension
    {
        [Pure]
        // public static ref TValue Value<TValue>(this ref BlobVariant blobVariant) where TValue : struct =>
            // ref UnsafeUtility.As<int, BlobPtr<TValue>>(ref blobVariant.MetaDataOffsetPtr).Value;
        public static unsafe ref TValue Value<TValue>(this ref BlobVariant blobVariant) where TValue : unmanaged =>
            ref UnsafeUtility.AsRef<TValue>(blobVariant.ValuePtr());

        [Pure]
        public static unsafe void* ValuePtr(this ref BlobVariant blobVariant)
        {
            fixed (int* thisPtr = &blobVariant.MetaDataOffsetPtr)
            {
                return (byte*)thisPtr + blobVariant.MetaDataOffsetPtr;
            }
        }

        [Pure]
        public static unsafe object Value(this ref BlobVariant blobVariant, Type valueType) =>
            Marshal.PtrToStructure(new IntPtr(blobVariant.ValuePtr()), valueType);

        [Pure]
        public static IEnumerable<ComponentType> GetComponentAccessList(this ref BlobVariant blobVariant)
        {
            var @delegate = DelegateRegistry<AccessorMethodAttribute.Delegate>.TryGetValue(blobVariant.VariantId);
            return @delegate == null ? Enumerable.Empty<ComponentType>() : @delegate(ref blobVariant);
        }

        public static T Read<T, TNodeBlob, TBlackboard>(this ref BlobVariant blobVariant, int index, ref TNodeBlob blob, ref TBlackboard bb)
            where T : unmanaged
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            var @delegate = DelegateRegistry<ReaderMethodAttribute.Delegate<T, TNodeBlob, TBlackboard>>
                .TryGetValue(blobVariant.VariantId)
            ;
            Debug.Assert(@delegate != null, nameof(@delegate) + " != null");
            return @delegate.Invoke(ref blobVariant, index, ref blob, ref bb);
        }

        public static IntPtr ReadOnlyPtr<TNodeBlob, TBlackboard>(this ref BlobVariant blobVariant, int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            var @delegate = DelegateRegistry<ReadOnlyPointerMethodAttribute.Delegate<TNodeBlob, TBlackboard>>
                .TryGetValue(blobVariant.VariantId)
            ;
            Debug.Assert(@delegate != null, nameof(@delegate) + " != null");
            return @delegate.Invoke(ref blobVariant, index, ref blob, ref bb);
        }

        public static IntPtr ReadWritePtr<TNodeBlob, TBlackboard>(this ref BlobVariant blobVariant, int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            var @delegate = DelegateRegistry<ReadWritePointerMethodAttribute.Delegate<TNodeBlob, TBlackboard>>
                .TryGetValue(blobVariant.VariantId)
            ;
            Debug.Assert(@delegate != null, nameof(@delegate) + " != null");
            return @delegate.Invoke(ref blobVariant, index, ref blob, ref bb);
        }

        public static IntPtr ReadOnlyPtrWithReadWriteFallback<TNodeBlob, TBlackboard>(this ref BlobVariant blobVariant, int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            var @delegate = DelegateRegistry<ReadOnlyPointerMethodAttribute.Delegate<TNodeBlob, TBlackboard>>
                .TryGetValue(blobVariant.VariantId)
            ;
            if (@delegate != null) return @delegate.Invoke(ref blobVariant, index, ref blob, ref bb);
            return ReadWritePtr(ref blobVariant, index, ref blob, ref bb);
        }

        public static ref T ReadRef<T, TNodeBlob, TBlackboard>(this ref BlobVariant blobVariant, int index, ref TNodeBlob blob, ref TBlackboard bb)
            where T : unmanaged
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            var @delegate = DelegateRegistry<RefReaderMethodAttribute.Delegate<T, TNodeBlob, TBlackboard>>
                .TryGetValue(blobVariant.VariantId)
            ;
            Debug.Assert(@delegate != null, nameof(@delegate) + " != null");
            return ref @delegate(ref blobVariant, index, ref blob, ref bb);
        }

        public static T ReadWithRefFallback<T, TNodeBlob, TBlackboard>(this ref BlobVariant blobVariant, int index, ref TNodeBlob blob, ref TBlackboard bb)
            where T : unmanaged
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            var reader = DelegateRegistry<ReaderMethodAttribute.Delegate<T, TNodeBlob, TBlackboard>>
                .TryGetValue(blobVariant.VariantId)
            ;
            if (reader != null) return reader(ref blobVariant, index, ref blob, ref bb);
            return ReadRef<T, TNodeBlob, TBlackboard>(ref blobVariant, index, ref blob, ref bb);
        }

        public static void Write<T, TNodeBlob, TBlackboard>(this ref BlobVariant blobVariant, int index, ref TNodeBlob blob, ref TBlackboard bb, T value)
            where T : unmanaged
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            var @delegate = DelegateRegistry<WriterMethodAttribute.Delegate<T, TNodeBlob, TBlackboard>>
                .TryGetValue(blobVariant.VariantId)
            ;
            Debug.Assert(@delegate != null, nameof(@delegate) + " != null");
            @delegate(ref blobVariant, index, ref blob, ref bb, value);
        }

        public static void WriteWithRefFallback<T, TNodeBlob, TBlackboard>(this ref BlobVariant blobVariant, int index, ref TNodeBlob blob, ref TBlackboard bb, T value)
            where T : unmanaged
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            var writer = DelegateRegistry<WriterMethodAttribute.Delegate<T, TNodeBlob, TBlackboard>>
                .TryGetValue(blobVariant.VariantId)
            ;
            if (writer != null) writer(ref blobVariant, index, ref blob, ref bb, value);
            else ReadRef<T, TNodeBlob, TBlackboard>(ref blobVariant, index, ref blob, ref bb) = value;
        }
    }
}