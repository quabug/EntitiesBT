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
        
        public static IEnumerable<ComponentType> AccessTypes(int index, INodeBlob blob)
        {
            yield return ComponentType.ReadWrite<Translation>();
            yield return ComponentType.ReadOnly<BehaviorTreeTickDeltaTime>();
        }

        public static NodeState Tick(int index, INodeBlob blob, IBlackboard bb)
        {
            ref var data = ref blob.GetNodeData<EntityMoveNode>(index);
            ref var translation = ref bb.GetDataRef<Translation>();
            var deltaTime = bb.GetData<BehaviorTreeTickDeltaTime>();
            translation.Value += data.Velocity * deltaTime.Value;
            return NodeState.Running;
        }
    }

    public class EntityMoveDebugView : BTDebugView<EntityMoveNode> {}
}
