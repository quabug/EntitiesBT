using EntitiesBT.Core;
using System;
using EntitiesBT.Variant;
using Unity.Mathematics;
using UnityEngine;

namespace EntitiesBT.Components.Odin
{
    [Serializable]
    [BehaviorNode("30E1D8D2-356D-422A-AF98-1CB734C3C63D")]
    public struct PrintNode : INodeData
    {
        public BlobVariantRO<float2> Value;

        public NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            Debug.Log(Value.Read(index, ref blob, ref bb));
            return NodeState.Success;
        }
    }
}