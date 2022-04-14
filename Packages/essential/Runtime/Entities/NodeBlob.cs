using System;
using EntitiesBT.Core;
using JetBrains.Annotations;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using Blob;

namespace EntitiesBT.Entities
{
    public struct NodeBlob
    {
        public const int VERSION = 4;

#region Serialized Data
        public BlobArray<int> Types;
        public BlobTreeAny Tree;
        public BlobArray<byte> DefaultGlobalValues;
#endregion
        
#region NonSerialized Data (runtime only)
        public int RuntimeId;
        public BlobArray<NodeState> States; // states of each nodes
        public BlobArray<byte> RuntimeDataBlob; // initialize from `DefaultDataBlob`
        public BlobArray<byte> RuntimeGlobalValues;
#endregion

        public int Count => Types.Length;
        public int RuntimeSize => CalculateRuntimeSize(Count, RuntimeDataBlob.Length, RuntimeGlobalValues.Length);

        public Blob.BlobTreeAny.Node this[int nodeIndex] => Tree[nodeIndex];

        [Pure]
        private static int CalculateDefaultSize(int count, int dataSize, int scopeValuesSize) =>
            UnsafeUtility.SizeOf<NodeBlob>() + dataSize + scopeValuesSize + sizeof(int) * count * 3/*Types/EndIndices/Offsets*/;
        
        [Pure]
        public static int CalculateRuntimeSize(int count, int dataSize, int scopeValuesSize) =>
            dataSize/*RuntimeDataBlob*/ + scopeValuesSize/*RuntimeScopeValues*/ + sizeof(NodeState) * count;

        [Pure]
        public static int CalculateSize(int count, int dataSize, int scopeValuesSize) =>
            CalculateDefaultSize(count, dataSize, scopeValuesSize) + CalculateRuntimeSize(count, dataSize, scopeValuesSize);
    }

    public readonly struct NodeBlobRef : INodeBlob, IEquatable<NodeBlobRef>
    {
        private ref NodeBlob _blob => ref BlobRef.Value;
        public readonly Unity.Entities.BlobAssetReference<NodeBlob> BlobRef;
        
        public NodeBlobRef(Unity.Entities.BlobAssetReference<NodeBlob> blobRef) => BlobRef = blobRef;

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
            (IntPtr) _blob.RuntimeDataBlob.UnsafePtr + _blob[nodeIndex].Offset;

        public unsafe IntPtr GetDefaultScopeValuePtr(int offset) =>
            IntPtr.Add(new IntPtr(_blob.DefaultGlobalValues.UnsafePtr), offset);

        public unsafe IntPtr GetRuntimeScopeValuePtr(int offset) =>
            IntPtr.Add(new IntPtr(_blob.RuntimeGlobalValues.UnsafePtr), offset);

        public NodeState GetState(int nodeIndex) => _blob.States[nodeIndex];
        public void SetState(int nodeIndex, NodeState state) => _blob.States[nodeIndex] = state;

        public bool Equals(NodeBlobRef other)
        {
            return BlobRef.Equals(other.BlobRef);
        }

        public override bool Equals(object obj)
        {
            return obj is NodeBlobRef other && Equals(other);
        }

        public override int GetHashCode()
        {
            return BlobRef.GetHashCode();
        }

        public static bool operator==(NodeBlobRef lhs, NodeBlobRef rhs)
        {
            return lhs.BlobRef == rhs.BlobRef;
        }

        public static bool operator !=(NodeBlobRef lhs, NodeBlobRef rhs)
        {
            return !(lhs == rhs);
        }
    }
}
