using System;
using System.Collections.Generic;
using EntitiesBT.Core;
using Unity.Entities;
using UnityEngine.InputSystem;

namespace EntitiesBT.Extensions.InputSystem
{
    public class BTCheckInputActionPhase : BTInputActionBase<CheckInputActionPhaseNode>
    {
        public InputActionPhase Phase;
        
        protected override void Build(ref CheckInputActionPhaseNode data, BlobBuilder _, ITreeNode<INodeDataBuilder>[] __)
        {
            base.Build(ref data, _, __);
            data.Phase = Phase;
        }
    }

    [BehaviorNode("60562945-A352-4537-95F6-62774823A492")]
    public struct CheckInputActionPhaseNode : IInputActionNodeData
    {
        public Guid ActionId { get; set; }
        public InputActionPhase Phase;

        public static IEnumerable<ComponentType> AccessTypes(int index, INodeBlob blob)
        {
            yield return ComponentType.ReadOnly<InputActionAssetComponent>();
        }
        
        public static NodeState Tick(int index, INodeBlob blob, IBlackboard bb)
        {
            var input = bb.GetData<InputActionAssetComponent>().Value;
            var data = blob.GetNodeData<CheckInputActionPhaseNode>(index);
            var action = input.FindAction(data.ActionId);
            return action != null && action.phase == data.Phase ? NodeState.Success : NodeState.Failure;
        }
    }
}
