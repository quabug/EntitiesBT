using System;
using EntitiesBT.Core;
using EntitiesBT.Variable;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace EntitiesBT.Extensions.InputSystem
{
    public class BTInputMove : BTInputActionBase<InputMoveNode>
    {
#if ODIN_INSPECTOR
        [Sirenix.Serialization.OdinSerialize, NonSerialized]
#endif
        public VariableProperty<float2> Output;

        protected override void Build(ref InputMoveNode data, BlobBuilder builder, ITreeNode<INodeDataBuilder>[] tree)
        {
            base.Build(ref data, builder, tree);
            Output.Allocate(ref builder, ref data.Output, this, tree);
        }
    }

    [Serializable, BehaviorNode("21CF9C9B-2BD4-4336-BFEF-4671060D1BD9")]
    public struct InputMoveNode : IInputActionNodeData
    {
        public Guid ActionId { get; set; }
        public BlobVariable<float2> Output;
        
        [ReadOnly(typeof(InputActionAssetComponent))]
        public NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            var inputValue = index.ReadInputActionValue<InputMoveNode, Vector2, TNodeBlob, TBlackboard>(ref blob, ref bb);
            if (!inputValue.HasValue) return NodeState.Failure;
            Output.GetDataRef(index, ref blob, ref bb) = inputValue.Value;
            return NodeState.Success;
        }

        public void Reset<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard blackboard)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
        }
    }
}
