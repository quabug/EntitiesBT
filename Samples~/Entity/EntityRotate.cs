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
    public class EntityRotate : BTNode<EntityRotateNode, EntityRotateNode.Data>
    {
        public Vector3 Axis;
        public float RadianPerSecond;

        public override unsafe void Build(void* dataPtr)
        {
            var ptr = (EntityRotateNode.Data*) dataPtr;
            ptr->Axis = Axis;
            ptr->RadianPerSecond = RadianPerSecond;
        }
    }
    
    [BehaviorNode("8E25032D-C06F-4AA9-B401-1AD31AF43A2F")]
    public class EntityRotateNode
    {
        public static readonly ComponentType[] Types = {
            ComponentType.ReadWrite<Rotation>()
          , ComponentType.ReadOnly<BehaviorTreeTickDeltaTime>()
        };
        
        public struct Data : INodeData
        {
            public float3 Axis;
            public float RadianPerSecond;
        }

        public static NodeState Tick(int index, INodeBlob blob, IBlackboard bb)
        {
            ref var data = ref blob.GetNodeData<Data>(index);
            ref var rotation = ref bb.GetDataRef<Rotation>();
            var deltaTime = bb.GetData<BehaviorTreeTickDeltaTime>();
            rotation.Value = math.mul(
                math.normalize(rotation.Value)
              , quaternion.AxisAngle(data.Axis, data.RadianPerSecond * deltaTime.Value)
            );
            return NodeState.Running;
        }
    }
    
    public class EntityRotateDebugView : BTDebugView<EntityRotateNode, EntityRotateNode.Data> {}
}
