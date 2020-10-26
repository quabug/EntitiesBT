using System;
using EntitiesBT.Core;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using UnityEngine.Scripting;
using static EntitiesBT.Core.Utilities;

namespace EntitiesBT.Variant
{
    public static class LocalVariant
    {
        public const string GUID = "BF510555-7E38-49BB-BDC1-E4A85A174EEC";

        [Serializable]
        public class Reader<T> : IVariantReader<T> where T : struct
        {
            public T Value;

            [Preserve, ReaderMethod(GUID)]
            private static ref T Read<TNodeBlob, TBlackboard>(ref BlobVariant blobVariant, int index, ref TNodeBlob blob, ref TBlackboard bb)
                where TNodeBlob : struct, INodeBlob
                where TBlackboard : struct, IBlackboard
            {
                return ref blobVariant.Value<T>();
            }

            public void Allocate(ref BlobBuilder builder, ref BlobVariant blobVariant, INodeDataBuilder self, ITreeNode<INodeDataBuilder>[] tree)
            {
                blobVariant.VariantId = GuidHashCode(GUID);
                builder.Allocate(ref blobVariant, Value);
            }
        }

        [Serializable]
        public class Writer<T> : IVariantWriter<T> where T : struct
        {
            public int ValueOffset;

            public void Allocate(ref BlobBuilder builder, ref BlobVariant blobVariant, INodeDataBuilder self, ITreeNode<INodeDataBuilder>[] tree)
            {
                blobVariant.VariantId = GuidHashCode(GUID);
                builder.Allocate(ref blobVariant, ValueOffset);
            }

            [Preserve, WriterMethod(GUID)]
            private static unsafe void Write<TNodeBlob, TBlackboard>(ref BlobVariant blobVariant, int index, ref TNodeBlob blob, ref TBlackboard bb, T value)
                where TNodeBlob : struct, INodeBlob
                where TBlackboard : struct, IBlackboard
            {
                var valueOffset = blobVariant.Value<int>();
                var valuePtr = blob.GetRuntimeDataPtr(index) + valueOffset;
                UnsafeUtility.AsRef<T>(valuePtr.ToPointer()) = value;
            }
        }
    }
}
