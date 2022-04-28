using JetBrains.Annotations;
using Unity.Collections.LowLevel.Unsafe;
using Blob;

namespace EntitiesBT.Core
{
    public struct NodeBlob
    {
        public const int VERSION = 5;

#region Serialized Data
        public BlobArray<int> Types;
        public BlobTreeAny Nodes;
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

        public BlobTreeAny.Node this[int nodeIndex] => Nodes[nodeIndex];

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
}
