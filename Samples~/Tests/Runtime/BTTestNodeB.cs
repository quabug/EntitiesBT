using EntitiesBT.Core;
using EntitiesBT.Editor;
using UnityEngine;

namespace EntitiesBT.Test
{
    public class NodeB : IBehaviorNode
    {
        public struct Data : INodeData
        {
            public int B;
            public int BB;
        }

        public void Reset(VirtualMachine vm, int index)
        {
            Debug.Log($"[B]: reset {index}");
        }

        public NodeState Tick(VirtualMachine vm, int index)
        {
            var data = vm.GetNodeData<Data>(index);
            var state = data.B == data.BB ? NodeState.Success : NodeState.Failure;
            Debug.Log($"[B]: tick {index} {state}");
            return state;
        }
    }
    
    public class BTTestNodeB : BTNode
    {
        public int B;
        public int BB;
        
        public override int Type => Factory.GetTypeId<NodeB>();
        public override unsafe void Build(void* dataPtr)
        {
            var ptr = (NodeB.Data*) dataPtr;
            ptr->B = B;
            ptr->BB = BB;
        }

        public override unsafe int Size => sizeof(NodeB.Data);
    }
}
