using System;
using EntitiesBT.Core;
using EntitiesBT.Variant;
using Nuwa.Blob;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using static EntitiesBT.Extensions.InputSystem.InputExtensions;

namespace EntitiesBT.Extensions.InputSystem
{
    [Serializable, BehaviorNode("2EBE8CF0-CFF1-436A-AB2F-1E9DABF3A1A3")]
    public struct InputLookNode : IInputActionNodeData
    {
        [field: CustomBuilder(typeof(InputActionGuidBuilder))] public Guid ActionId { get; set; }
        public BlobVariantWO<float2> Output;
        
        public NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            var inputValue = ReadInputActionValue<InputLookNode, Vector2, TNodeBlob, TBlackboard>(index, ref blob, ref bb);
            if (!inputValue.HasValue) return NodeState.Failure;
            Output.Write(index, ref blob, ref bb, inputValue.Value);
            return NodeState.Success;
        }
    }
}
