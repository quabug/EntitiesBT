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
        where T : struct
        where U : struct, IReadInputValueNode
    {
#if ODIN_INSPECTOR
        [Sirenix.Serialization.OdinSerialize, NonSerialized]
#endif
        public VariableProperty<T> Output;

        protected override unsafe void Build(ref U data, BlobBuilder builder, ITreeNode<INodeDataBuilder>[] tree)
        {
            base.Build(ref data, builder, tree);
            ref var output = ref UnsafeUtilityEx.AsRef<BlobVariable<T>>(data.OutputPtr);
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
        public static unsafe NodeState Tick(int index, INodeBlob blob, IBlackboard bb)
        {
            var inputValue = bb.ReadInputActionValue<TNodeData, TValue>(index, blob);
            if (!inputValue.HasValue) return NodeState.Failure;
            ref var data = ref blob.GetNodeData<TNodeData>(index);
            ref var output = ref UnsafeUtilityEx.AsRef<BlobVariable<TValue>>(data.OutputPtr);
            output.GetDataRef(index, blob, bb) = inputValue.Value;
            return NodeState.Success;
        }
    }
}
