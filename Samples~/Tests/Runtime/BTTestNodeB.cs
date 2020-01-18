using System.Collections.Generic;
using EntitiesBT.Components;
using EntitiesBT.Core;
using UnityEngine;

namespace EntitiesBT.Test
{
    [BehaviorNode("793F38C5-B0BD-410A-87BD-D122B10E636F")]
    public struct NodeB : INodeData
    {
        public int B;
        public int BB;

        public static void Reset(int index, INodeBlob blob, IBlackboard blackboard)
        {
            Debug.Log($"[B]: reset {index}");
        }

        public static NodeState Tick(int index, INodeBlob blob, IBlackboard blackboard)
        {
            var data = blob.GetNodeData<NodeB>(index);
            var state = data.B == data.BB ? NodeState.Success : NodeState.Failure;
            Debug.Log($"[B]: tick {index} {state}");
            return state;
        }
    }
    
    public class BTTestNodeB : BTNode<NodeB>
    {
        public int B;
        public int BB;

        public override unsafe void Build(void* dataPtr, ITreeNode<INodeDataBuilder>[] builders)
        {
            var ptr = (NodeB*) dataPtr;
            ptr->B = B;
            ptr->BB = BB;
        }
    }
}
