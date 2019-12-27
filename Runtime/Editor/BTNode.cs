using EntitiesBT.Core;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace EntitiesBT.Editor
{
    [DisallowMultipleComponent]
    public abstract class BTNode : MonoBehaviour
    {
        public abstract int NodeId { get; }
        public abstract int Size { get; }
        public abstract unsafe void Build(void* dataPtr);
        
        public int Index { get; set; }
        private void Reset() => name = GetType().Name;
    }
    
    [DisallowMultipleComponent]
    public abstract class BTNode<T> : BTNode, INodeDataBuilder
        where T : struct, INodeData
    {
        public override int Size => UnsafeUtility.SizeOf<T>();
        public override unsafe void Build(void* dataPtr) {}
    }
    
    public struct ZeroNodeData : INodeData {}
}
