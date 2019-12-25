using EntitiesBT.Core;
using EntitiesBT.Editor;
using UnityEngine;

namespace EntitiesBT.Test
{
    public class NodeA : IBehaviorNode
    {
        public struct Data : INodeData
        {
            public int A;
        }

        public void Reset(VirtualMachine vm, int index)
        {
            Debug.Log($"[A]: reset {index}");
        }

        public NodeState Tick(VirtualMachine vm, int index)
        {
            var data = vm.GetNodeData<Data>(index);
            var state = data.A == 0 ? NodeState.Failure : NodeState.Success;
            Debug.Log($"[A]: tick {index} {state}");
            return state;
        }
    }
    
    public class BTTestNodeA : BTNode
    {
        public int A;
        public override unsafe void Build(void* dataPtr) => ((NodeA.Data*) dataPtr)->A = A;
        public override IBehaviorNode BehaviorNode => new NodeA();
        public override unsafe int Size => sizeof(NodeA.Data);
    }
}
