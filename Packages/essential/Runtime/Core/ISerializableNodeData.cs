using System;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;

namespace EntitiesBT.Core
{
    public interface ISerializableNodeData
    {
        Type NodeType { get; }

        unsafe void Build(void* dataPtr, BlobBuilder builder, INodeDataBuilder self, ITreeNode<INodeDataBuilder>[] tree);
        unsafe void Load(void* dataPtr);
    }

    [Serializable]
    public abstract class SerializableNodeData<T> : ISerializableNodeData where T : unmanaged, INodeData
    {
        public Type NodeType => typeof(T);

        unsafe void ISerializableNodeData.Build(void* dataPtr, BlobBuilder builder, INodeDataBuilder self, ITreeNode<INodeDataBuilder>[] tree)
        {
            Build(ref UnsafeUtility.AsRef<T>(dataPtr), builder, self, tree);
        }

        public unsafe void Load(void* dataPtr)
        {
            Load(ref UnsafeUtility.AsRef<T>(dataPtr));
        }

        protected virtual void Build(ref T data, BlobBuilder builder, INodeDataBuilder self, ITreeNode<INodeDataBuilder>[] tree) {}
        protected virtual void Load(ref T data) {}
    }
}