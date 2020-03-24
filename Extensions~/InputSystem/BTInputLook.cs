using System;
using EntitiesBT.Core;
using EntitiesBT.Variable;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace EntitiesBT.Extensions.InputSystem
{
    public class BTInputLook : BTInputActionBase<InputLookNode>
    {
#if ODIN_INSPECTOR
        [Sirenix.Serialization.OdinSerialize, NonSerialized]
#endif
        public VariableProperty<float2> Output;

        protected override void Build(ref InputLookNode data, BlobBuilder builder, ITreeNode<INodeDataBuilder>[] tree)
        {
            base.Build(ref data, builder, tree);
            Output.Allocate(ref builder, ref data.Output, this, tree);
        }
    }

    [Serializable, BehaviorNode("2EBE8CF0-CFF1-436A-AB2F-1E9DABF3A1A3")]
    public struct InputLookNode : IInputActionNodeData
    {
        public Guid ActionId { get; set; }
        public BlobVariable<float2> Output;
        
        [ReadOnly(typeof(InputActionAssetComponent))]
        public NodeState Tick(int index, INodeBlob blob, IBlackboard bb)
        {
            var inputValue = bb.ReadInputActionValue<InputLookNode, Vector2>(index, blob);
            if (!inputValue.HasValue) return NodeState.Failure;
            Output.GetDataRef(index, blob, bb) = inputValue.Value;
            return NodeState.Success;
        }

        public void Reset(int index, INodeBlob blob, IBlackboard blackboard)
        {
        }
    }
}
