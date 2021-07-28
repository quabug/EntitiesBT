using System;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;

namespace EntitiesBT.Core
{
    public interface ISerializedNodeData
    {
        Type NodeType { get; }
        unsafe void Build(void* dataPtr, BlobBuilder builder, INodeDataBuilder self, ITreeNode<INodeDataBuilder>[] tree);
    }

    public abstract class SerializedNodeData<T> : ISerializedNodeData where T : unmanaged, INodeData
    {
        public Type NodeType => typeof(T);

        unsafe void ISerializedNodeData.Build(void* dataPtr, BlobBuilder builder, INodeDataBuilder self, ITreeNode<INodeDataBuilder>[] tree)
        {
            Build(ref UnsafeUtility.AsRef<T>(dataPtr), builder, self, tree);
        }

        protected abstract void Build(ref T data, BlobBuilder builder, INodeDataBuilder self, ITreeNode<INodeDataBuilder>[] tree);
    }
}