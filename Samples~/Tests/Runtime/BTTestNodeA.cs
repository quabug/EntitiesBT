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

        public void Reset(int index, INodeBlob blob, IBlackboard blackboard)
        {
            Debug.Log($"[A]: reset {index}");
        }

        public NodeState Tick(int index, INodeBlob blob, IBlackboard blackboard)
        {
            var data = blob.GetNodeData<Data>(index);
            var state = data.A == 0 ? NodeState.Failure : NodeState.Success;
            Debug.Log($"[A]: tick {index} {state}");
            return state;
        }
    }
    
    public class BTTestNodeA : BTNode<NodeA, NodeA.Data>
    {
        public int A;
        public override unsafe void Build(void* dataPtr) => ((NodeA.Data*) dataPtr)->A = A;
    }
}
