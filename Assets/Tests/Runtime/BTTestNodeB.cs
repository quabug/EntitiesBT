#if UNITY_EDITOR

using Blob;
using EntitiesBT.Components;
using EntitiesBT.Core;
using Nuwa.Blob;
using UnityEngine;

namespace EntitiesBT.Test
{
    [BehaviorNode("793F38C5-B0BD-410A-87BD-D122B10E636F")]
    public struct NodeB : INodeData, ICustomResetAction
    {
        public int B;
        public int BB;

        public void Reset<TNodeBlob, TBlackboard>(int index, ref TNodeBlob _, ref TBlackboard __)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            Debug.Log($"[B]: reset {index}");
        }

        public NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard __)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            var data = blob.GetNodeData<NodeB, TNodeBlob>(index);
            var state = data.B == data.BB ? NodeState.Success : NodeState.Failure;
            Debug.Log($"[B]: tick {index} {state}");
            return state;
        }
    }
    
    public class BTTestNodeB : BTNode<NodeB>
    {
        public int B;
        public int BB;

        protected override void Build(
            UnsafeBlobStreamValue<NodeB> value,
            IBlobStream stream,
            ITreeNode<INodeDataBuilder>[] tree
        )
        {
            value.Value.B = B;
            value.Value.BB = B;
        }
    }
}

#endif
