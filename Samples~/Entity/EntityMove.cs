using System;
using EntitiesBT.Core;
using EntitiesBT.Components;
using EntitiesBT.Components.DebugView;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace EntitiesBT.Sample
{
    public class EntityMove : BTNode<EntityMoveNode, EntityMoveNode.Data>
    {
        public Vector3 Velocity;

        public override unsafe void Build(void* dataPtr)
        {
            var ptr = (EntityMoveNode.Data*) dataPtr;
            ptr->Velocity = Velocity;
        }
    }
    
    [BehaviorNode("F5C2EE7E-690A-4B5C-9489-FB362C949192")]
    public class EntityMoveNode
    {
        public static readonly ComponentType[] Types = {
            ComponentType.ReadWrite<Translation>()
          , ComponentType.ReadOnly<BehaviorTreeTickDeltaTime>()
        };
        
        [Serializable]
        public struct Data : INodeData
        {
            public float3 Velocity;
        }

        public static NodeState Tick(int index, INodeBlob blob, IBlackboard bb)
        {
            ref var data = ref blob.GetNodeData<Data>(index);
            ref var translation = ref bb.GetDataRef<Translation>();
            var deltaTime = bb.GetData<BehaviorTreeTickDeltaTime>();
            translation.Value += data.Velocity * deltaTime.Value;
            return NodeState.Running;
        }
    }

    public class EntityMoveDebugView : BTDebugView<EntityMoveNode, EntityMoveNode.Data>
    {
        public float3 EntityPosition;

        public override void InitView(INodeBlob blob, IBlackboard bb, int index)
        {
            Data.Velocity = blob.GetNodeData<EntityMoveNode.Data>(index).Velocity;
        }

        public override void TickView(INodeBlob blob, IBlackboard bb, int index)
        {
            blob.GetNodeData<EntityMoveNode.Data>(index).Velocity = Data.Velocity;
            base.TickView(blob, bb, index);
            EntityPosition = bb.GetData<Translation>().Value;
        }
    }
}
