#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using EntitiesBT.Components;
using EntitiesBT.Core;
using Unity.Entities;
using UnityEngine.Assertions;

namespace EntitiesBT.Test
{
    [BehaviorNode("59E8EB08-1652-45F3-81DB-775D9D76508D")]
    public struct TestNode : INodeData
    {
        public NodeState DefaultState;
        public NodeState State;
        public int Index;
        public int ResetTimes;
        public int TickTimes;
        
        public static IEnumerable<ComponentType> AccessTypes(int index, INodeBlob blob) => Enumerable.Empty<ComponentType>();

        public static void Reset(int index, INodeBlob blob, IBlackboard blackboard)
        {
            ref var data = ref blob.GetNodeDefaultData<TestNode>(index);
            data.State = data.DefaultState;
            data.Index = index;
            data.ResetTimes++;
        }

        public static NodeState Tick(int index, INodeBlob blob, IBlackboard blackboard)
        {
            ref var data = ref blob.GetNodeDefaultData<TestNode>(index);
            Assert.AreEqual(data.Index, index);
            data.TickTimes++;
            return data.State;
        }
    }
    
    public class BTTestNodeState : BTNode<TestNode>
    {
        public NodeState State;

        protected override void Build(ref TestNode data, BlobBuilder _, ITreeNode<INodeDataBuilder>[] __)
        {
            data.DefaultState = State;
            data.ResetTimes = 0;
            data.TickTimes = 0;
        }
    }
}

#endif
