using EntitiesBT.Core;
using EntitiesBT.Variant;
using Unity.Mathematics;
using UnityEngine;

namespace EntitiesBT.Extensions.UnityMovement
{
    [BehaviorNode("7FCFF548-4D65-402A-B885-20633923DC22")]
    public struct SetTransformRotationNode : INodeData
    {
        public BlobVariantRO<quaternion> RotationProperty;
        
        public NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            var transform = bb.GetObject<Transform>();
            if (transform == null) return NodeState.Failure;
            var rotation = RotationProperty.Read(index, ref blob, ref bb);
            transform.rotation = rotation;
            return NodeState.Success;
        }
    }
}
