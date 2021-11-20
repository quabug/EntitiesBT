using System;
using EntitiesBT.Variant;
using Nuwa;
using Unity.Entities;

namespace EntitiesBT
{
    [VariantClass(GUID)]
    public class GraphNodeVariant
    {
        public const string GUID = "E085D58A-34CC-4548-9F09-974C60AC396C";

        [Serializable]
        public class Any : IVariant
        {
            [ReadOnly, UnityDrawProperty] public VariantNode Node;

            public IntPtr Allocate(ref BlobBuilder builder, ref BlobVariant blobVariant)
            {
                // TODO: check validation
                // Assert.IsTrue(Node.IsValid());
                return Node.Allocate(ref builder, ref blobVariant);
            }
        }

        [Serializable] public class Reader<T> : Any, IVariantReader<T> where T : unmanaged {}
        [Serializable] public class Writer<T> : Any, IVariantWriter<T> where T : unmanaged {}
        [Serializable] public class ReaderAndWriter<T> : Any, IVariantReaderAndWriter<T> where T : unmanaged {}
    }
}