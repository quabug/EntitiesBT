using System;
using Blob;
using EntitiesBT.Core;
using Nuwa.Blob;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace EntitiesBT.Sample
{
    [Serializable]
    [BehaviorNode("404DBF2F-A83B-4FF8-B755-F2A6D6836793")]
    public struct IsEntityPositionInBoxNode : INodeData
    {
        public Bounds Bounds;

        public NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            var translation = bb.GetData<Translation>();
            return Bounds.Contains(translation.Value) ? NodeState.Success : NodeState.Failure;
        }
    }

    [DefaultBuilder]
    public class ColliderBoundsBuilder : Nuwa.Blob.Builder<Bounds>
    {
        public Transform Transform;
        public BoxCollider Box;

        protected override void BuildImpl(IBlobStream stream, Nuwa.Blob.UnsafeBlobStreamValue<Bounds> value)
        {
            stream.WriteValue(new Bounds(Box.center + Transform.position, Box.size));
        }
    }
}
