using System;
using Blob;
using Nuwa.Blob;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;

namespace EntitiesBT.Core
{
    public readonly struct ManagedNodeBlobRef : INodeBlob, IEquatable<ManagedNodeBlobRef>, IDisposable
    {
        private ref NodeBlob _blob => ref BlobRef.Value;
        public readonly ManagedBlobAssetReference<NodeBlob> BlobRef;
        
        public ManagedNodeBlobRef(ManagedBlobAssetReference<NodeBlob> blobRef) => BlobRef = blobRef;

        public int RuntimeId
        {
            get => _blob.RuntimeId;
            set => _blob.RuntimeId = value;
        }

        public int Count => _blob.Count;
        public int GetTypeId(int nodeIndex) => _blob.Types[nodeIndex];
        public int GetEndIndex(int nodeIndex) => _blob[nodeIndex].EndIndex;
        public int GetNodeDataSize(int nodeIndex) => _blob[nodeIndex].Size;
        public int GetNodeDataSize(int nodeIndex, int count)
        {
            var currentOffset = _blob[nodeIndex].Offset;
            var nextOffset = _blob[math.min(nodeIndex + count, Count)].Offset;
            return nextOffset - currentOffset;
        }

        public unsafe void ResetStates(int index, int count) =>
            UnsafeUtility.MemClear(_blob.States.UnsafePtr + index, sizeof(NodeState) * count);

        public unsafe IntPtr GetDefaultDataPtr(int nodeIndex) => (IntPtr) _blob[nodeIndex].UnsafePtr;
        
        public unsafe IntPtr GetRuntimeDataPtr(int nodeIndex) =>
            IntPtr.Add(new IntPtr(_blob.RuntimeDataBlob.UnsafePtr), _blob[nodeIndex].Offset);

        public unsafe IntPtr GetDefaultScopeValuePtr(int offset) =>
            IntPtr.Add(new IntPtr(_blob.DefaultGlobalValues.UnsafePtr), offset);

        public unsafe IntPtr GetRuntimeScopeValuePtr(int offset) =>
            IntPtr.Add(new IntPtr(_blob.RuntimeGlobalValues.UnsafePtr), offset);

        public NodeState GetState(int nodeIndex) => _blob.States[nodeIndex];
        public void SetState(int nodeIndex, NodeState state) => _blob.States[nodeIndex] = state;

        public bool Equals(ManagedNodeBlobRef other)
        {
            return BlobRef.Equals(other.BlobRef);
        }

        public override bool Equals(object obj)
        {
            return obj is ManagedNodeBlobRef other && Equals(other);
        }

        public override int GetHashCode()
        {
            return BlobRef.GetHashCode();
        }

        public static bool operator==(ManagedNodeBlobRef lhs, ManagedNodeBlobRef rhs)
        {
            return lhs.BlobRef == rhs.BlobRef;
        }

        public static bool operator !=(ManagedNodeBlobRef lhs, ManagedNodeBlobRef rhs)
        {
            return !(lhs == rhs);
        }

        public void Dispose()
        {
            BlobRef?.Dispose();
        }
    }
}
