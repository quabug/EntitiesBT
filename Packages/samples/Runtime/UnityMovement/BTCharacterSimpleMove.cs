using EntitiesBT.Attributes;
using EntitiesBT.Components;
using EntitiesBT.Core;
using EntitiesBT.Sample;
using EntitiesBT.Variant;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace EntitiesBT.Extensions.UnityMovement
{
    public class BTCharacterSimpleMove : BTNode<CharacterSimpleMoveNode>
    {
        public bool IsLocal;
        
        [SerializeReference, SerializeReferenceDrawer]
        public float3VariantReader VelocityPropertyReader;

        protected override unsafe void Build(ref CharacterSimpleMoveNode data, BlobBuilder builder, ITreeNode<INodeDataBuilder>[] tree)
        {
            data.IsLocal = IsLocal;
            VelocityPropertyReader.Allocate(ref builder, ref data.Velocity, this, tree);
        }
    }

    [BehaviorNode("21F65017-DCC0-449A-8AE5-E2D296B9E0E5")]
    public struct CharacterSimpleMoveNode : INodeData
    {
        public bool IsLocal;
        public BlobVariantReader<float3> Velocity;

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

        public void Reset<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard blackboard)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
        }
    }
}
