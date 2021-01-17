using System;
using EntitiesBT.Core;
using EntitiesBT.Variant;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace EntitiesBT.Extensions.InputSystem
{
    public class BTInputLook : BTInputActionBase<InputLookNode>
    {
        public IVariantWriter<float2> Output;

        protected override unsafe void Build(ref InputLookNode data, BlobBuilder builder, ITreeNode<INodeDataBuilder>[] tree)
        {
            base.Build(ref data, builder, tree);
            Output.Allocate(ref builder, ref data.Output, this, tree);
        }
    }

    [Serializable, BehaviorNode("2EBE8CF0-CFF1-436A-AB2F-1E9DABF3A1A3")]
    public struct InputLookNode : IInputActionNodeData
    {
        public Guid ActionId { get; set; }
        public BlobVariantWriter<float2> Output;
        
        [ReadOnly(typeof(InputActionAssetComponent))]
        public NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            var inputValue = index.ReadInputActionValue<InputLookNode, Vector2, TNodeBlob, TBlackboard>(ref blob, ref bb);
            if (!inputValue.HasValue) return NodeState.Failure;
            Output.Write(index, ref blob, ref bb, inputValue.Value);
            return NodeState.Success;
        }

        public void Reset<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard blackboard)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
        }
    }
}
