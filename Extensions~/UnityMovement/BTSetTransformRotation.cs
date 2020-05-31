using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesBT.Components;
using EntitiesBT.Core;
using EntitiesBT.Variable;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace EntitiesBT.Extensions.UnityMovement
{
    public class BTSetTransformRotation : BTNode<SetTransformRotationNode>
    {
#if ODIN_INSPECTOR
        [Sirenix.Serialization.OdinSerialize, NonSerialized]
#endif
        public VariableProperty<quaternion> RotationProperty;

        protected override void Build(ref SetTransformRotationNode data, BlobBuilder builder, ITreeNode<INodeDataBuilder>[] tree)
        {
            RotationProperty.Allocate(ref builder, ref data.RotationProperty, this, tree);
        }
    }
    
    [BehaviorNode("7FCFF548-4D65-402A-B885-20633923DC22")]
    public struct SetTransformRotationNode : INodeData
    {
        [ReadOnly] public BlobVariable<quaternion> RotationProperty;
        
        [ReadWrite(typeof(Transform))]
        public NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            var transform = bb.GetData<Transform>();
            if (transform == null) return NodeState.Failure;
            var rotation = RotationProperty.GetData(index, blob, bb);
            transform.rotation = rotation;
            return NodeState.Success;
        }

        public void Reset<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard blackboard)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
        }
    }
}
