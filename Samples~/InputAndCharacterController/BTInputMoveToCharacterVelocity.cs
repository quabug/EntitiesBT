using System.Collections.Generic;
using EntitiesBT.Components;
using EntitiesBT.Core;
using EntitiesBT.Extensions.InputSystem;
using EntitiesBT.Extensions.UnityMovement;
using Unity.Entities;
using UnityEngine;

namespace EntitiesBT.Samples
{
    public class BTInputMoveToCharacterVelocity : BTNode<InputMoveToCharacterVelocityNode>
    {
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.MinValue(0)]
#endif
        public float Speed;

        protected override void Build(ref InputMoveToCharacterVelocityNode data, BlobBuilder _, ITreeNode<INodeDataBuilder>[] __)
        {
            data.Speed = Speed;
        }
    }

    [BehaviorNode("B4559A1E-392B-4B8C-A074-B323AB31EEA7")]
    public struct InputMoveToCharacterVelocityNode : INodeData
    {
        public float Speed;
        
        public static IEnumerable<ComponentType> AccessTypes(int index, INodeBlob blob)
        {
            yield return ComponentType.ReadOnly<BTInputMoveData>();
            yield return ComponentType.ReadWrite<BTCharacterSimpleMoveVelocity>();
        }
        
        public static NodeState Tick(int index, INodeBlob blob, IBlackboard bb)
        {
            var input = bb.GetData<BTInputMoveData>();
            var direction = new Vector3(input.Value.x, 0, input.Value.y).normalized;
            var speed = blob.GetNodeData<InputMoveToCharacterVelocityNode>(index).Speed;
            bb.GetDataRef<BTCharacterSimpleMoveVelocity>().Value = direction * speed;
            return NodeState.Success;
        }
    }
}
