using System;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;

namespace EntitiesBT.Core
{
    public interface ISerializableNodeData
    {
        Type NodeType { get; }

        unsafe void Build(void* dataPtr, BlobBuilder builder, INodeDataBuilder self,
            ITreeNode<INodeDataBuilder>[] tree);
    }

    public interface ITestT {}
    public class SerializableNodeData {}

[Serializable]
    public abstract class SerializableNodeData<T> : ISerializableNodeData where T : unmanaged, INodeData
    {
        public Type NodeType => typeof(T);

        unsafe void ISerializableNodeData.Build(void* dataPtr, BlobBuilder builder, INodeDataBuilder self, ITreeNode<INodeDataBuilder>[] tree)
        {
            Build(ref UnsafeUtility.AsRef<T>(dataPtr), builder, self, tree);
        }

        protected virtual void Build(ref T data, BlobBuilder builder, INodeDataBuilder self, ITreeNode<INodeDataBuilder>[] tree) {}
    }
}