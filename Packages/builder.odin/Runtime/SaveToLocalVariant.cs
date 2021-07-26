using System;
using EntitiesBT.Core;
using EntitiesBT.Variant;
using Unity.Entities;
using UnityEngine.Scripting;
using static EntitiesBT.Core.Utilities;

namespace EntitiesBT.Components.Odin
{
    [VariantClass(GUID)]
    public class SaveToLocalVariant
    {
        [Serializable]
        public class Any<T> : IVariant where T : unmanaged
        {
            public IVariantReader<T> Reader;
            public T Value;

            public IntPtr Allocate(ref BlobBuilder builder, ref BlobVariant blobVariant, INodeDataBuilder self, ITreeNode<INodeDataBuilder>[] tree)
            {
                blobVariant.VariantId = GuidHashCode(GUID);
                return builder.Allocate(ref blobVariant, Value);
            }
        }

        [Serializable] public class Reader<T> : Any<T>, IVariantReader<T> where T : unmanaged {}
        [Serializable] public class ReaderAndWriter<T> : Any<T>, IVariantReaderAndWriter<T> where T : unmanaged {}

        public const string GUID = "F315684E-2AA1-4780-B31B-2791A93359F1";

        [RefReaderMethod]
        private static ref T Read<T, TNodeBlob, TBlackboard>(ref BlobVariant blobVariant, int index, ref TNodeBlob blob, ref TBlackboard bb)
            where T : unmanaged
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            return ref blobVariant.Value<T>();
        }
    }
}