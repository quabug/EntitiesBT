using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using EntitiesBT.Core;
using Unity.Entities;

namespace EntitiesBT.Variant
{

    public struct BlobVariantRW
    {
        public BlobVariant Reader;
        public BlobVariant Writer;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct BlobVariantRW<T> : IRuntimeComponentAccessor where T : unmanaged
    {
        public BlobVariantRO<T> Reader;
        public BlobVariantWO<T> Writer;

        public IEnumerable<ComponentType> AccessTypes => Reader.AccessTypes.Concat(Writer.AccessTypes);

        public T Read<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
            => Reader.Read(index, ref blob, ref bb);

        public void Write<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb, T value)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
            => Writer.Write(index, ref blob, ref bb, value);

        public bool IsValid => Reader.IsValid && Writer.IsValid;
    }
}