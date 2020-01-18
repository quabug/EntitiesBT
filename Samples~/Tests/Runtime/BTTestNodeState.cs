using EntitiesBT.Components;
using EntitiesBT.Core;
using NUnit.Framework;

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

        public override unsafe void Build(void* dataPtr, ITreeNode<INodeDataBuilder>[] builders)
        {
            var ptr = (TestNode*) dataPtr;
            ptr->DefaultState = State;
            ptr->ResetTimes = 0;
            ptr->TickTimes = 0;
        }
    }
}
