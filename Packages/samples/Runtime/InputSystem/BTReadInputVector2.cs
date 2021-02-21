using System;
using EntitiesBT.Core;
using EntitiesBT.Variant;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace EntitiesBT.Extensions.InputSystem
{
    public class BTReadInputVector2 : BTReadInputValue<Vector2, ReadInputVector2Node> {}

    [BehaviorNode("4B15B481-966C-4F73-96E7-EB26261DF498")]
    public struct ReadInputVector2Node : IReadInputValueNode
    {
        public Guid ActionId { get; set; }
        public BlobVariantReader<Vector2> Output;
        public unsafe void* OutputPtr => UnsafeUtility.AddressOf(ref Output);

        public NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
            => ReadInputValueNode<ReadInputVector2Node, Vector2>.Tick(index, ref blob, ref bb);

        public void Reset<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard blackboard)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {}
    }
}
