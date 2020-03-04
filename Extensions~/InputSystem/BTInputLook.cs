using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesBT.Core;
using EntitiesBT.DebugView;
using EntitiesBT.Entities;
using EntitiesBT.Variable;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace EntitiesBT.Extensions.InputSystem
{
    public class BTInputLook : BTInputActionBase<InputLookNode>
    {
        [SerializeReference, SerializeReferenceButton]
        public float2Property Output;

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
        
        public static IEnumerable<ComponentType> AccessTypes(int index, INodeBlob blob)
        {
            return blob.GetNodeDefaultData<InputLookNode>(index).Output.ComponentAccessList
                .Append(ComponentType.ReadOnly<InputActionAssetComponent>())
            ;
        }
        
        public static NodeState Tick(int index, INodeBlob blob, IBlackboard bb)
        {
            var inputValue = bb.ReadInputActionValue<InputLookNode, Vector2>(index, blob);
            if (!inputValue.HasValue) return NodeState.Failure;
            ref var data = ref blob.GetNodeData<InputLookNode>(index);
            data.Output.GetDataRef(index, blob, bb) = inputValue.Value;
            return NodeState.Success;
        }
    }
}
