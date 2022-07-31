using System;
using EntitiesBT.Core;
using static EntitiesBT.Core.Utilities;

namespace EntitiesBT.Variant
{
    [VariantClass(GUID)]
    public static class LocalVariant
    {
        public const string GUID = "BF510555-7E38-49BB-BDC1-E4A85A174EEC";

        [Serializable]
        public class Any<T> : IVariant<T> where T : unmanaged
        {
            public T Value;

            public void Allocate(BlobVariantStream stream)
            {
                stream.SetVariantId(GuidHashCode(GUID));
                stream.SetVariantValue(Value);
            }

            public object PreviewValue => Value;
        }

        [Serializable] public class Reader<T> : Any<T>, IVariantReader<T> where T : unmanaged {}
        [Serializable] public class Writer<T> : Any<T>, IVariantWriter<T> where T : unmanaged {}
        [Serializable] public class ReaderAndWriter<T> : Any<T>, IVariantReaderAndWriter<T> where T : unmanaged {}

        [RefReaderMethod]
        private static ref T Read<T, TNodeBlob, TBlackboard>(ref BlobVariant blobVariant, int index, ref TNodeBlob blob, ref TBlackboard bb)
            where T : unmanaged
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            return ref blobVariant.As<T>();
        }

        [ReadWritePointerMethod]
        private static unsafe IntPtr GetPointer<TNodeBlob, TBlackboard>(ref BlobVariant blobVariant, int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            return new IntPtr(blobVariant.AsPointer());
        }
    }
}
