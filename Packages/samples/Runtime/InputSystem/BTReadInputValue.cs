using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesBT.Core;
using EntitiesBT.Variable;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;

namespace EntitiesBT.Extensions.InputSystem
{
    public class BTReadInputValue<T, U> : BTInputActionBase<U>
        where T : unmanaged
        where U : struct, IReadInputValueNode
    {
#if ODIN_INSPECTOR
        [Sirenix.Serialization.OdinSerialize, NonSerialized]
#endif
        public VariableProperty<T> Output;

        protected override unsafe void Build(ref U data, BlobBuilder builder, ITreeNode<INodeDataBuilder>[] tree)
        {
            base.Build(ref data, builder, tree);
            ref var output = ref UnsafeUtility.AsRef<BlobVariable<T>>(data.OutputPtr);
            Output.Allocate(ref builder, ref output, this, tree);
        }
    }

    public interface IReadInputValueNode : IInputActionNodeData
    {
        unsafe void* OutputPtr { get; }
    }
    
    public struct ReadInputValueNode<TNodeData, TValue>
        where TNodeData : struct, IReadInputValueNode
        where TValue : struct
    {
        public static unsafe NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            var inputValue = index.ReadInputActionValue<TNodeData, TValue, TNodeBlob, TBlackboard>(ref blob, ref bb);
            if (!inputValue.HasValue) return NodeState.Failure;
            ref var data = ref blob.GetNodeData<TNodeData, TNodeBlob>(index);
            ref var output = ref UnsafeUtility.AsRef<BlobVariable<TValue>>(data.OutputPtr);
            output.GetDataRef(index, ref blob, ref bb) = inputValue.Value;
            return NodeState.Success;
        }
    }
}
