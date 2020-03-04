using System.Collections.Generic;
using EntitiesBT.Components;
using EntitiesBT.Core;
using EntitiesBT.Variable;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace EntitiesBT.Extensions.UnityMovement
{
    public class BTCharacterSimpleMove : BTNode<CharacterSimpleMoveNode>
    {
        public bool IsLocal;
        
        [SerializeReference, SerializeReferenceButton]
        public float3Property VelocityProperty;

        protected override void Build(ref CharacterSimpleMoveNode data, BlobBuilder builder, ITreeNode<INodeDataBuilder>[] tree)
        {
            data.IsLocal = IsLocal;
            VelocityProperty.Allocate(ref builder, ref data.Velocity, this, tree);
        }
    }

    [BehaviorNode("21F65017-DCC0-449A-8AE5-E2D296B9E0E5")]
    public struct CharacterSimpleMoveNode : INodeData
    {
        public bool IsLocal;
        public BlobVariable<float3> Velocity;

        public static IEnumerable<ComponentType> AccessTypes(int index, INodeBlob blob)
        {
            yield return ComponentType.ReadWrite<CharacterController>();
        }
        
        public static NodeState Tick(int index, INodeBlob blob, IBlackboard bb)
        {
            var controller = bb.GetData<CharacterController>();
            if (controller == null) return NodeState.Failure;
            ref var data = ref blob.GetNodeData<CharacterSimpleMoveNode>(index);
            Vector3 velocity = data.Velocity.GetData(index, blob, bb);
            controller.SimpleMove(data.IsLocal ? controller.transform.localToWorldMatrix.MultiplyVector(velocity) : velocity);
            return NodeState.Success;
        }
    }
}
