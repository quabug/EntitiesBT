using System;
using EntitiesBT.Core;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using UnityEngine.Scripting;
using static EntitiesBT.Core.Utilities;

namespace EntitiesBT.Variable
{
    public static class LocalVariableProperty
    {
        public const string GUID = "BF510555-7E38-49BB-BDC1-E4A85A174EEC";

        [Serializable]
        public class Reader<T> : IVariablePropertyReader<T> where T : struct
        {
            public T Value;

            [Preserve, ReaderMethod(GUID)]
            private static ref T Read<TNodeBlob, TBlackboard>(ref BlobVariable blobVariable, int index, ref TNodeBlob blob, ref TBlackboard bb)
                where TNodeBlob : struct, INodeBlob
                where TBlackboard : struct, IBlackboard
            {
                return ref blobVariable.Value<T>();
            }

            public void Allocate(ref BlobBuilder builder, ref BlobVariable blobVariable, INodeDataBuilder self, ITreeNode<INodeDataBuilder>[] tree)
            {
                blobVariable.VariableId = GuidHashCode(GUID);
                builder.Allocate(ref blobVariable, Value);
            }
        }

        [Serializable]
        public class Writer<T> : IVariablePropertyWriter<T> where T : struct
        {
            public int ValueOffset;

            public void Allocate(ref BlobBuilder builder, ref BlobVariable blobVariable, INodeDataBuilder self, ITreeNode<INodeDataBuilder>[] tree)
            {
                blobVariable.VariableId = GuidHashCode(GUID);
                builder.Allocate(ref blobVariable, ValueOffset);
            }

            [Preserve, WriterMethod(GUID)]
            private static unsafe void Write<TNodeBlob, TBlackboard>(ref BlobVariable blobVariable, int index, ref TNodeBlob blob, ref TBlackboard bb, T value)
                where TNodeBlob : struct, INodeBlob
                where TBlackboard : struct, IBlackboard
            {
                var valueOffset = blobVariable.Value<int>();
                var valuePtr = blob.GetRuntimeDataPtr(index) + valueOffset;
                UnsafeUtility.AsRef<T>(valuePtr.ToPointer()) = value;
            }
        }
    }
}
