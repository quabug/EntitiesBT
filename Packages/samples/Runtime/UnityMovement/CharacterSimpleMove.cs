using EntitiesBT.Core;
using EntitiesBT.Variant;
using Unity.Mathematics;
using UnityEngine;

namespace EntitiesBT.Extensions.UnityMovement
{
    [BehaviorNode("21F65017-DCC0-449A-8AE5-E2D296B9E0E5")]
    public struct CharacterSimpleMoveNode : INodeData
    {
        public bool IsLocal;
        public BlobVariantRO<float3> Velocity;

        public NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            var controller = bb.GetObject<CharacterController>();
            if (controller == null) return NodeState.Failure;
            Vector3 velocity = Velocity.Read(index, ref blob, ref bb);
            controller.SimpleMove(IsLocal ? controller.transform.localToWorldMatrix.MultiplyVector(velocity) : velocity);
            return NodeState.Success;
        }
    }
}
