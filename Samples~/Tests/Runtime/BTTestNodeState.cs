using EntitiesBT.Core;
using EntitiesBT.Editor;
using NUnit.Framework;

namespace EntitiesBT.Test
{
    public struct TestNodeData : INodeData
    {
        public NodeState State;
    }
    
    public class TestNode : IBehaviorNode
    {
        public NodeState State;
        public int Index = -1;
        public int InitializeTimes = 0;
        public int ResetTimes = 0;
        public int TickTimes = 0;

        public void Initialize(VirtualMachine vm, int index)
        {
            Index = index;
            InitializeTimes++;
        }

        public void Reset(VirtualMachine vm, int index)
        {
            State = vm.GetNodeData<TestNodeData>(index).State;
            Assert.AreEqual(Index, index);
            ResetTimes++;
        }

        public NodeState Tick(VirtualMachine vm, int index)
        {
            Assert.AreEqual(Index, index);
            TickTimes++;
            return State;
        }
    }
    
    public class BTTestNodeState : BTNode
    {
        public NodeState State;
        public override int Type => Factory.GetTypeId<TestNode>();
        public override unsafe int Size => sizeof(TestNodeData);
        public override unsafe void Build(void* dataPtr) =>
            ((TestNodeData*) dataPtr)->State = State;
    }
}
