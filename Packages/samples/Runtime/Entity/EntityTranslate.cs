using System;
using EntitiesBT.Core;
using EntitiesBT.Components;
using EntitiesBT.DebugView;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace EntitiesBT.Sample
{
    public class EntityTranslate : BTNode<EntityTranslateNode>
    {
        public Vector3 Position;

        protected override void Build(ref EntityTranslateNode data, BlobBuilder _, ITreeNode<INodeDataBuilder>[] __)
        {
            data.Position = Position;
        }
    }
    
    [Serializable]
    [BehaviorNode("29A30E27-7A3C-42F4-A0A4-49EFBD890279")]
    public struct EntityTranslateNode : INodeData
    {
        public float3 Position;

        public NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            ref var translation = ref bb.GetDataRef<Translation>();
            translation.Value = Position;
            return NodeState.Success;
        }

        public void Reset<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard blackboard)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
        }
    }

    public class EntityTranslateDebugView : BTDebugView<EntityTranslateNode> {}
}
