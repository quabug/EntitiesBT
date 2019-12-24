using EntitiesBT.Core;
using EntitiesBT.Editor;
using UnityEngine;

namespace EntitiesBT.Test
{
    public struct NodeDataA : INodeData
    {
        public int A;
    }

    public class NodeA : IBehaviorNode
    {
        public void Initialize(VirtualMachine vm, int index)
        {
            Debug.Log($"[A]: initialize {index}");
        }

        public void Reset(VirtualMachine vm, int index)
        {
            Debug.Log($"[A]: reset {index}");
        }

        public NodeState Tick(VirtualMachine vm, int index)
        {
            var data = vm.GetNodeData<NodeDataA>(index);
            var state = data.A == 0 ? NodeState.Failure : NodeState.Success;
            Debug.Log($"[A]: tick {index} {state}");
            return state;
        }
    }
    
    public class BTTestNodeA : BTNode
    {
        public int A;
        public override int Type => Factory.GetTypeId<NodeA>();
        public override unsafe void Build(void* dataPtr) => ((NodeDataA*) dataPtr)->A = A;
        public override unsafe int Size => sizeof(NodeDataA);
    }
}
