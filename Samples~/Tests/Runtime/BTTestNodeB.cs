#if UNITY_EDITOR

using EntitiesBT.Components;
using EntitiesBT.Core;
using Unity.Entities;
using UnityEngine;

namespace EntitiesBT.Test
{
    [BehaviorNode("793F38C5-B0BD-410A-87BD-D122B10E636F")]
    public struct NodeB : INodeData
    {
        public int B;
        public int BB;

        public void Reset(int index, INodeBlob _, IBlackboard __)
        {
            Debug.Log($"[B]: reset {index}");
        }

        public NodeState Tick(int index, INodeBlob blob, IBlackboard __)
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

        protected override void Build(ref NodeB data, BlobBuilder _, ITreeNode<INodeDataBuilder>[] __)
        {
            data.B = B;
            data.BB = BB;
        }
    }
}

#endif
