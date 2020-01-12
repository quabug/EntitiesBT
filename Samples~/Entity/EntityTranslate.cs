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
    public class EntityTranslate : BTNode<EntityTranslateNode, EntityTranslateNode.Data>
    {
        public Vector3 Position;

        protected override void Build(ref EntityTranslateNode.Data data)
        {
            data.Position = Position;
        }
    }
    
    [BehaviorNode("29A30E27-7A3C-42F4-A0A4-49EFBD890279")]
    public class EntityTranslateNode
    {
        public static readonly ComponentType[] Types = { ComponentType.ReadWrite<Translation>() };
        
        [Serializable]
        public struct Data : INodeData
        {
            public float3 Position;
        }

        public static NodeState Tick(int index, INodeBlob blob, IBlackboard bb)
        {
            ref var data = ref blob.GetNodeData<Data>(index);
            ref var translation = ref bb.GetDataRef<Translation>();
            translation.Value = data.Position;
            return NodeState.Success;
        }
    }

    public class EntityTranslateDebugView : BTDebugView<EntityTranslateNode, EntityTranslateNode.Data> {}
}
