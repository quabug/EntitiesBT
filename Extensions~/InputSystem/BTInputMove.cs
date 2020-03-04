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
    public class BTInputMove : BTInputActionBase<InputMoveNode>
    {
        [SerializeReference, SerializeReferenceButton]
        public float2Property Output;

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
        
        public static IEnumerable<ComponentType> AccessTypes(int index, INodeBlob blob)
        {
            ref var data = ref blob.GetNodeDefaultData<InputMoveNode>(index);
            return data.Output.ComponentAccessList.Append(ComponentType.ReadOnly<InputActionAssetComponent>());
        }
        
        public static NodeState Tick(int index, INodeBlob blob, IBlackboard bb)
        {
            var inputValue = bb.ReadInputActionValue<InputMoveNode, Vector2>(index, blob);
            if (!inputValue.HasValue) return NodeState.Failure;
            ref var data = ref blob.GetNodeData<InputMoveNode>(index);
            data.Output.GetDataRef(index, blob, bb) = inputValue.Value;
            return NodeState.Success;
        }
    }
}
