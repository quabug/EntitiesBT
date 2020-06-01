#if UNITY_EDITOR

using EntitiesBT.Components;
using EntitiesBT.Core;
using Unity.Entities;
using UnityEngine;

namespace EntitiesBT.Test
{
    [BehaviorNode("29E6B4D9-4388-4EA8-8F36-162ACBE5C166")]
    public struct NodeA : INodeData
    {
        public int A;
        
        public void Reset<TNodeBlob, TBlackboard>(int index, ref TNodeBlob _, ref TBlackboard __)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            Debug.Log($"[A]: reset {index}");
        }

        public NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard __)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            var data = blob.GetNodeData<NodeA, TNodeBlob>(index);
            var state = data.A == 0 ? NodeState.Failure : NodeState.Success;
            Debug.Log($"[A]: tick {index} {state}");
            return state;
        }
    }
    
    public class BTTestNodeA : BTNode<NodeA>
    {
        public int A;
        protected override void Build(ref NodeA data, BlobBuilder _, ITreeNode<INodeDataBuilder>[] __) => data.A = A;
    }
}

#endif
