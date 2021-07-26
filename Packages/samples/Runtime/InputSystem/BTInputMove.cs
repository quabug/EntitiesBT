using System;
using EntitiesBT.Core;
using EntitiesBT.Variant;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using static EntitiesBT.Extensions.InputSystem.InputExtensions;

namespace EntitiesBT.Extensions.InputSystem
{
    public class BTInputMove : BTInputActionBase<InputMoveNode>
    {
        public SerializedVariantRW<float2> Output;

        protected override unsafe void Build(ref InputMoveNode data, BlobBuilder builder, ITreeNode<INodeDataBuilder>[] tree)
        {
            base.Build(ref data, builder, tree);
            Output.Allocate(ref builder, ref data.Output, this, tree);
        }
    }

    [Serializable, BehaviorNode("21CF9C9B-2BD4-4336-BFEF-4671060D1BD9")]
    public struct InputMoveNode : IInputActionNodeData
    {
        public Guid ActionId { get; set; }
        public BlobVariantReaderAndWriter<float2> Output;
        
        public NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            var inputValue = ReadInputActionValue<InputMoveNode, Vector2, TNodeBlob, TBlackboard>(index, ref blob, ref bb);
            if (!inputValue.HasValue) return NodeState.Failure;
            Output.Write(index, ref blob, ref bb, inputValue.Value);
            return NodeState.Success;
        }
    }
}
