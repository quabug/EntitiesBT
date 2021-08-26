using System;
using EntitiesBT.Core;
using EntitiesBT.Variant;
using Nuwa.Blob;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace EntitiesBT.Extensions.InputSystem
{
    [BehaviorNode("4B15B481-966C-4F73-96E7-EB26261DF498")]
    public struct ReadInputVector2Node : IReadInputValueNode
    {
        [field: CustomBuilder(typeof(InputActionGuidBuilder))] public Guid ActionId { get; set; }
        public BlobVariantRO<Vector2> Output;
        public unsafe void* OutputPtr => UnsafeUtility.AddressOf(ref Output);

        public NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
            => ReadInputValueNode<ReadInputVector2Node, Vector2>.Tick(index, ref blob, ref bb);
    }
}
