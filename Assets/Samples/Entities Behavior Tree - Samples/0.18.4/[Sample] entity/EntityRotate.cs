using System;
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
    public class EntityRotate : BTNode<EntityRotateNode>
    {
        public Vector3 Axis;
        public float RadianPerSecond;

        protected override void Build(ref EntityRotateNode data, BlobBuilder _, ITreeNode<INodeDataBuilder>[] __)
        {
            data.Axis = Axis;
            data.RadianPerSecond = RadianPerSecond;
        }
    }
    
    [Serializable]
    [BehaviorNode("8E25032D-C06F-4AA9-B401-1AD31AF43A2F")]
    public struct EntityRotateNode : INodeData
    {
        public float3 Axis;
        public float RadianPerSecond;
        
        [ReadOnly(typeof(BehaviorTreeTickDeltaTime))]
        [ReadWrite(typeof(Rotation))]
        public NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            ref var rotation = ref bb.GetDataRef<Rotation>();
            var deltaTime = bb.GetData<BehaviorTreeTickDeltaTime>();
            rotation.Value = math.mul(
                math.normalize(rotation.Value)
              , quaternion.AxisAngle(Axis, RadianPerSecond * deltaTime.Value)
            );
            return NodeState.Running;
        }

        public void Reset<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard blackboard)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
        }
    }
    
    public class EntityRotateDebugView : BTDebugView<EntityRotateNode> {}
}
