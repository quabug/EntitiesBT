#if UNITY_EDITOR

using EntitiesBT.Components;
using EntitiesBT.Core;
using Unity.Entities;
using UnityEngine.Assertions;

namespace EntitiesBT.Test
{
    [BehaviorNode("59E8EB08-1652-45F3-81DB-775D9D76508D")]
    public struct TestNode : INodeData
    {
        public NodeState State;
        public int Index;
        public int ResetTimes;
        public int TickTimes;
        
        public void Reset<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard blackboard)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            Index = index;
            ResetTimes++;

            ref var defaultData = ref blob.GetNodeDefaultData<TestNode, TNodeBlob>(index);
            defaultData.Index = index;
            defaultData.ResetTimes++;
        }

        public NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard blackboard)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            Assert.AreEqual(Index, index);
            TickTimes++;
            
            ref var defaultData = ref blob.GetNodeDefaultData<TestNode, TNodeBlob>(index);
            defaultData.TickTimes++;
            
            return State;
        }
    }
    
    public class BTTestNodeState : BTNode<TestNode>
    {
        public NodeState State;

        protected override void Build(ref TestNode data, BlobBuilder builder, ITreeNode<INodeDataBuilder>[] tree)
        {
            data.State = State;
            data.ResetTimes = 0;
            data.TickTimes = 0;
        }
    }
}

#endif
