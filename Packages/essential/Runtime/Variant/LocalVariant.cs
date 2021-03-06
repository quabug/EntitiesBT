using System;
using EntitiesBT.Core;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Scripting;
using static EntitiesBT.Core.Utilities;

namespace EntitiesBT.Variant
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class LocalVariantAttribute : PropertyAttribute {}

    public static class LocalVariant
    {
        [Serializable]
        public class Any<T> : IVariant where T : unmanaged
        {
            public T Value;

            public IntPtr Allocate(ref BlobBuilder builder, ref BlobVariant blobVariant, INodeDataBuilder self, ITreeNode<INodeDataBuilder>[] tree)
            {
                blobVariant.VariantId = GuidHashCode(GUID);
                return builder.Allocate(ref blobVariant, Value);
            }
        }

        [Serializable] public class Reader<T> : Any<T>, IVariantReader<T> where T : unmanaged {}
        [Serializable] public class ReaderAndWriter<T> : Any<T>, IVariantReaderAndWriter<T> where T : unmanaged {}

        public const string GUID = "BF510555-7E38-49BB-BDC1-E4A85A174EEC";

        [Preserve, RefReaderMethod(GUID)]
        private static ref T Read<T, TNodeBlob, TBlackboard>(ref BlobVariant blobVariant, int index, ref TNodeBlob blob, ref TBlackboard bb)
            where T : unmanaged
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            return ref blobVariant.Value<T>();
        }
    }
}
