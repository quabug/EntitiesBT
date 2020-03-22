using System;
using System.Collections.Generic;
using EntitiesBT.Core;
using Unity.Entities;
using UnityEngine;

namespace EntitiesBT.Extensions.InputSystem
{
    public class BTReadInputVector2 : BTReadInputValue<Vector2, ReadInputVector2Node> {}

    [BehaviorNode("4B15B481-966C-4F73-96E7-EB26261DF498")]
    public struct ReadInputVector2Node : IReadInputValueNode
    {
        public ReadInputValueNode<Vector2> Data;

        public Guid ActionId
        {
            get => Data.ActionId;
            set => Data.ActionId = value;
        }

        public unsafe void* OutputPtr => Data.OutputPtr;

        public static IEnumerable<ComponentType> AccessTypes(int index, INodeBlob blob) =>
            ReadInputValueNode<Vector2>.AccessTypes(index, blob);

        public static NodeState Tick(int index, INodeBlob blob, IBlackboard bb) =>
            ReadInputValueNode<Vector2>.Tick(index, blob, bb);
    }
}
