using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesBT.Core;
using EntitiesBT.Components;
using EntitiesBT.DebugView;
using EntitiesBT.Entities;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace EntitiesBT.Sample
{
    public class EntityMove : BTNode<EntityMoveNode>
    {
        public Vector3 Velocity;

        protected override void Build(ref EntityMoveNode data, BlobBuilder _, ITreeNode<INodeDataBuilder>[] __)
        {
            data.Velocity = Velocity;
        }
    }
    
    [Serializable]
    [BehaviorNode("F5C2EE7E-690A-4B5C-9489-FB362C949192")]
    public struct EntityMoveNode : INodeData
    {
        public float3 Velocity;

        [ReadOnly(typeof(BehaviorTreeTickDeltaTime))]
        [ReadWrite(typeof(Translation))]
        public NodeState Tick(int index, INodeBlob blob, IBlackboard bb)
        {
            ref var translation = ref bb.GetDataRef<Translation>();
            var deltaTime = bb.GetData<BehaviorTreeTickDeltaTime>();
            translation.Value += Velocity * deltaTime.Value;
            return NodeState.Running;
        }

        public void Reset(int index, INodeBlob blob, IBlackboard blackboard)
        {
        }
    }

    public class EntityMoveDebugView : BTDebugView<EntityMoveNode> {}
}
