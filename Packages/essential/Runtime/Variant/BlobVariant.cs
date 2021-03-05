using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using EntitiesBT.Core;
using JetBrains.Annotations;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;

namespace EntitiesBT.Variant
{
    [StructLayout(LayoutKind.Sequential)]
    public struct BlobVariant
    {
        public int VariantId;
        public int MetaDataOffsetPtr;

        public static BlobVariant INVALID = new BlobVariant {VariantId = 0, MetaDataOffsetPtr = 0};

        [Pure]
        public ref TValue Value<TValue>() where TValue : struct =>
            ref UnsafeUtility.As<int, BlobPtr<TValue>>(ref MetaDataOffsetPtr).Value;

        [Pure]
        public IEnumerable<ComponentType> GetComponentAccessList()
        {
            var @delegate = DelegateRegistry<AccessorMethodAttribute.Delegate>.TryGetValue(VariantId);
            return @delegate == null ? Enumerable.Empty<ComponentType>() : @delegate(ref this);
        }
    }

    internal static class BlobVariantExtension
    {
        public static T Read<T, TNodeBlob, TBlackboard>(this ref BlobVariant blobVariant, int index, ref TNodeBlob blob, ref TBlackboard bb)
            where T : unmanaged
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            var @delegate = DelegateRegistry<ReaderMethodAttribute.Delegate<T, TNodeBlob, TBlackboard>>
                .TryGetValue(blobVariant .VariantId)
            ;
            Debug.Assert(@delegate != null, nameof(@delegate) + " != null");
            return @delegate.Invoke(ref blobVariant, index, ref blob, ref bb);
        }

        public static ref T ReadRef<T, TNodeBlob, TBlackboard>(this ref BlobVariant blobVariant, int index, ref TNodeBlob blob, ref TBlackboard bb)
            where T : unmanaged
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            var @delegate = DelegateRegistry<RefReaderMethodAttribute.Delegate<T, TNodeBlob, TBlackboard>>
                .TryGetValue(blobVariant .VariantId)
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
                .TryGetValue(blobVariant .VariantId)
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
                .TryGetValue(blobVariant .VariantId)
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
                .TryGetValue(blobVariant .VariantId)
            ;
            if (writer != null) writer(ref blobVariant, index, ref blob, ref bb, value);
            else ReadRef<T, TNodeBlob, TBlackboard>(ref blobVariant, index, ref blob, ref bb) = value;
        }
    }
}
