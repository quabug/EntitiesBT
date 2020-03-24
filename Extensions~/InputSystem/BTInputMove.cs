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
        public NodeState Tick(int index, INodeBlob blob, IBlackboard bb)
        {
            var inputValue = bb.ReadInputActionValue<InputMoveNode, Vector2>(index, blob);
            if (!inputValue.HasValue) return NodeState.Failure;
            Output.GetDataRef(index, blob, bb) = inputValue.Value;
            return NodeState.Success;
        }

        public void Reset(int index, INodeBlob blob, IBlackboard blackboard)
        {
        }
    }
}
