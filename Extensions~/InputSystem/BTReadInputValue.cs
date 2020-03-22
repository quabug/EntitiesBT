using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesBT.Core;
using EntitiesBT.Variable;
using Sirenix.Serialization;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;

namespace EntitiesBT.Extensions.InputSystem
{
    public class BTReadInputValue<T, U> : BTInputActionBase<U>
        where T : struct
        where U : struct, IReadInputValueNode
    {
        [OdinSerialize, NonSerialized]
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
    
    public struct ReadInputValueNode<T> : IReadInputValueNode where T : struct
    {
        public Guid ActionId { get; set; }
        public BlobVariable<T> Output;
        public unsafe void* OutputPtr => UnsafeUtility.AddressOf(ref Output);
        
        public static IEnumerable<ComponentType> AccessTypes(int index, INodeBlob blob)
        {
            return blob.GetNodeDefaultData<ReadInputValueNode<T>>(index).Output.ComponentAccessList
                .Append(ComponentType.ReadOnly<InputActionAssetComponent>())
            ;
        }
        
        public static NodeState Tick(int index, INodeBlob blob, IBlackboard bb)
        {
            var inputValue = bb.ReadInputActionValue<ReadInputValueNode<T>, T>(index, blob);
            if (!inputValue.HasValue) return NodeState.Failure;
            ref var data = ref blob.GetNodeData<ReadInputValueNode<T>>(index);
            data.Output.GetDataRef(index, blob, bb) = inputValue.Value;
            return NodeState.Success;
        }
    }
}
