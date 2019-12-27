using EntitiesBT.Core;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace EntitiesBT.Editor
{
    [DisallowMultipleComponent]
    public abstract class BTNode : MonoBehaviour
    {
        public abstract int GetTypeId(Registries<IBehaviorNode> registries);
        public abstract int Size { get; }
        public abstract unsafe void Build(void* dataPtr);
        
        public int Index { get; set; }
        private void Reset() => name = GetType().Name;
    }
    
    [DisallowMultipleComponent]
    public abstract class BTNode<T, U> : BTNode, INodeDataBuilder
        where T : IBehaviorNode
        where U : struct, INodeData
    {
        public override int GetTypeId(Registries<IBehaviorNode> registries) => registries.GetIndex<T>();
        public override int Size => UnsafeUtility.SizeOf<U>();
        public override unsafe void Build(void* dataPtr) {}
    }
    
    public struct ZeroNodeData : INodeData {}
    
    public class UnknownBehaviorNode : IBehaviorNode {
        public void Reset(int index, INodeBlob blob, IBlackboard bb)
        {
            throw new System.NotImplementedException();
        }

        public NodeState Tick(int index, INodeBlob blob, IBlackboard bb)
        {
            throw new System.NotImplementedException();
        }
    }
}
