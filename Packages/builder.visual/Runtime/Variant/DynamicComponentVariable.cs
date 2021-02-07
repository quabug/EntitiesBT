using System;
using EntitiesBT.Core;
using EntitiesBT.Variant;
using Unity.Entities;
using static EntitiesBT.Core.Utilities;

namespace EntitiesBT.Builder.Visual
{
    public static class DynamicComponentVariant
    {
        [Serializable]
        public class Reader<T> : IVariantReader<T> where T : unmanaged
        {
            private readonly ulong _stableHash;
            private readonly int _offset;

            public Reader(ulong stableHash, int offset)
            {
                _stableHash = stableHash;
                _offset = offset;
            }

            public IntPtr Allocate(ref BlobBuilder builder, ref BlobVariant blobVariant, INodeDataBuilder self, ITreeNode<INodeDataBuilder>[] tree)
            {
                blobVariant.VariantId = GuidHashCode(ComponentVariant.GUID);
                return builder.Allocate(ref blobVariant, new ComponentVariant.DynamicComponentData{StableHash = _stableHash, Offset = _offset});
            }
        }
    }
}
