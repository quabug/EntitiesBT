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
        
        public static IEnumerable<ComponentType> AccessTypes(int index, INodeBlob blob)
        {
            yield return ComponentType.ReadWrite<Rotation>();
            yield return ComponentType.ReadOnly<BehaviorTreeTickDeltaTime>();
        }

        public static NodeState Tick(int index, INodeBlob blob, IBlackboard bb)
        {
            ref var data = ref blob.GetNodeData<EntityRotateNode>(index);
            ref var rotation = ref bb.GetDataRef<Rotation>();
            var deltaTime = bb.GetData<BehaviorTreeTickDeltaTime>();
            rotation.Value = math.mul(
                math.normalize(rotation.Value)
              , quaternion.AxisAngle(data.Axis, data.RadianPerSecond * deltaTime.Value)
            );
            return NodeState.Running;
        }
    }
    
    public class EntityRotateDebugView : BTDebugView<EntityRotateNode> {}
}
