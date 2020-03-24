using System;
using EntitiesBT.Core;
using EntitiesBT.Variable;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace EntitiesBT.Extensions.InputSystem
{
    public class BTReadInputVector2 : BTReadInputValue<Vector2, ReadInputVector2Node> {}

    [BehaviorNode("4B15B481-966C-4F73-96E7-EB26261DF498")]
    public struct ReadInputVector2Node : IReadInputValueNode
    {
        public Guid ActionId { get; set; }
        public BlobVariable<Vector2> Output;
        public unsafe void* OutputPtr => UnsafeUtility.AddressOf(ref Output);

        [ReadOnly(typeof(InputActionAssetComponent))]
        public NodeState Tick(int index, INodeBlob blob, IBlackboard bb) =>
            ReadInputValueNode<ReadInputVector2Node, Vector2>.Tick(index, blob, bb);

        public void Reset(int index, INodeBlob blob, IBlackboard blackboard) {}
    }
}
