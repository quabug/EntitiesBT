using System;
using EntitiesBT.Core;
using EntitiesBT.Components.Odin;
using EntitiesBT.Entities;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace EntitiesBT.Odin.Sample
{
    public class EntityMove : OdinNode<EntityMoveNode>
    {
        public Vector3 Velocity;

        protected override void Build(ref EntityMoveNode data, BlobBuilder _, ITreeNode<INodeDataBuilder>[] __)
        {
            data.Velocity = Velocity;
        }
    }

    [Serializable]
    [BehaviorNode("4DD74C64-8282-402E-A830-3C7F7D0A599A")]
    public struct EntityMoveNode : INodeData
    {
        public float3 Velocity;

        public NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            ref var translation = ref bb.GetDataRef<Translation>();
            var deltaTime = bb.GetData<BehaviorTreeTickDeltaTime>();
            translation.Value += Velocity * deltaTime.Value;
            return NodeState.Running;
        }

        public void Reset<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard blackboard)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
        }
    }
}
