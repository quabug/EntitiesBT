using System;
using EntitiesBT.Components;
using EntitiesBT.Core;
using NUnit.Framework;

namespace EntitiesBT.Test
{
    public static class TestNode
    {
        public static readonly int Id = new Guid("59E8EB08-1652-45F3-81DB-775D9D76508D").GetHashCode();

        static TestNode()
        {
            VirtualMachine.Register(Id, Reset, Tick);
        }
        
        public struct Data : INodeData
        {
            public NodeState DefaultState;
            public NodeState State;
            public int Index;
            public int ResetTimes;
            public int TickTimes;
        }

        static void Reset(int index, INodeBlob blob, IBlackboard blackboard)
        {
            ref var data = ref blob.GetNodeData<Data>(index);
            data.State = data.DefaultState;
            data.Index = index;
            data.ResetTimes++;
        }

        static NodeState Tick(int index, INodeBlob blob, IBlackboard blackboard)
        {
            ref var data = ref blob.GetNodeData<Data>(index);
            Assert.AreEqual(data.Index, index);
            data.TickTimes++;
            return data.State;
        }
    }
    
    public class BTTestNodeState : BTNode<TestNode.Data>
    {
        public NodeState State;

        public override int NodeId => TestNode.Id;

        public override unsafe void Build(void* dataPtr)
        {
            var ptr = (TestNode.Data*) dataPtr;
            ptr->DefaultState = State;
            ptr->ResetTimes = 0;
            ptr->TickTimes = 0;
        }
    }
}
