using EntitiesBT.Components;
using EntitiesBT.Core;
using UnityEngine;

namespace EntitiesBT.Test
{
    [BehaviorNode("29E6B4D9-4388-4EA8-8F36-162ACBE5C166")]
    public struct NodeA : INodeData
    {
        public int A;

        public static void Reset(int index, INodeBlob blob, IBlackboard blackboard)
        {
            Debug.Log($"[A]: reset {index}");
        }

        public static NodeState Tick(int index, INodeBlob blob, IBlackboard blackboard)
        {
            var data = blob.GetNodeData<NodeA>(index);
            var state = data.A == 0 ? NodeState.Failure : NodeState.Success;
            Debug.Log($"[A]: tick {index} {state}");
            return state;
        }
    }
    
    public class BTTestNodeA : BTNode<NodeA>
    {
        public int A;
        public override unsafe void Build(void* dataPtr, ITreeNode<INodeDataBuilder>[] builders) => ((NodeA*) dataPtr)->A = A;
    }
}
