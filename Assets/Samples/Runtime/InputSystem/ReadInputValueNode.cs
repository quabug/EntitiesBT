using EntitiesBT.Core;
using EntitiesBT.Variant;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using static EntitiesBT.Extensions.InputSystem.InputExtensions;

namespace EntitiesBT.Extensions.InputSystem
{
    public interface IReadInputValueNode : IInputActionNodeData
    {
        unsafe void* OutputPtr { get; }
    }
    
    public struct ReadInputValueNode<TNodeData, TValue>
        where TNodeData : struct, IReadInputValueNode
        where TValue : unmanaged
    {
        [ReadOnly(typeof(InputActionAssetComponent))]
        public static unsafe NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            var inputValue = ReadInputActionValue<TNodeData, TValue, TNodeBlob, TBlackboard>(index, ref blob, ref bb);
            if (!inputValue.HasValue) return NodeState.Failure;
            ref var data = ref blob.GetNodeData<TNodeData, TNodeBlob>(index);
            ref var output = ref UnsafeUtility.AsRef<BlobVariantWO<TValue>>(data.OutputPtr);
            output.Write(index, ref blob, ref bb, inputValue.Value);
            return NodeState.Success;
        }
    }
}
