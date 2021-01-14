using System.Collections.Generic;
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

        [Pure]
        public ref TValue Value<TValue>() where TValue : struct =>
            ref UnsafeUtility.As<int, BlobPtr<TValue>>(ref MetaDataOffsetPtr).Value;

        [Pure]
        public IEnumerable<ComponentType> GetComponentAccessList() =>
            VariantRegisters.GetComponentAccess(VariantId)(ref this);
    }

    internal static class BlobVariantExtension
    {
        public static T Read<T, TNodeBlob, TBlackboard>(this ref BlobVariant blobVariant, int index, ref TNodeBlob blob, ref TBlackboard bb)
            where T : unmanaged
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            return VariantRegisters<T>.GetReader<TNodeBlob, TBlackboard>(blobVariant.VariantId)(ref blobVariant, index, ref blob, ref bb);
        }

        public static ref T ReadRef<T, TNodeBlob, TBlackboard>(this ref BlobVariant blobVariant, int index, ref TNodeBlob blob, ref TBlackboard bb)
            where T : unmanaged
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            return ref VariantRegisters<T>.GetRefReader<TNodeBlob, TBlackboard>(blobVariant.VariantId)(ref blobVariant, index, ref blob, ref bb);
        }

        public static T ReadWithRefFallback<T, TNodeBlob, TBlackboard>(this ref BlobVariant blobVariant, int index, ref TNodeBlob blob, ref TBlackboard bb)
            where T : unmanaged
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            return VariantRegisters<T>.TryGetReader<TNodeBlob, TBlackboard>(blobVariant.VariantId, out var reader)
                ? reader(ref blobVariant, index, ref blob, ref bb)
                : VariantRegisters<T>.GetRefReader<TNodeBlob, TBlackboard>(blobVariant.VariantId)(ref blobVariant, index, ref blob, ref bb)
            ;
        }

        public static void Write<T, TNodeBlob, TBlackboard>(this ref BlobVariant blobVariant, int index, ref TNodeBlob blob, ref TBlackboard bb, T value)
            where T : unmanaged
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            VariantRegisters<T>.GetWriter<TNodeBlob, TBlackboard>(blobVariant.VariantId)(ref blobVariant, index, ref blob, ref bb, value);
        }

        public static void WriteWithRefFallback<T, TNodeBlob, TBlackboard>(this ref BlobVariant blobVariant, int index, ref TNodeBlob blob, ref TBlackboard bb, T value)
            where T : unmanaged
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            if (VariantRegisters<T>.TryGetWriter<TNodeBlob, TBlackboard>(blobVariant.VariantId, out var writer))
                writer(ref blobVariant, index, ref blob, ref bb, value);
            else
                VariantRegisters<T>.GetRefReader<TNodeBlob, TBlackboard>(blobVariant.VariantId)(ref blobVariant, index, ref blob, ref bb) = value;
        }
    }
}
