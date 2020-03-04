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
        [SerializeReference, SerializeReferenceButton]
        public quaternionProperty RotationProperty;

        protected override void Build(ref SetTransformRotationNode data, BlobBuilder builder, ITreeNode<INodeDataBuilder>[] tree)
        {
            RotationProperty.Allocate(ref builder, ref data.RotationProperty, this, tree);
        }
    }
    
    [BehaviorNode("7FCFF548-4D65-402A-B885-20633923DC22")]
    public struct SetTransformRotationNode : INodeData
    {
        public BlobVariable<quaternion> RotationProperty;
        
        public static IEnumerable<ComponentType> AccessTypes(int index, INodeBlob blob)
        {
            ref var data = ref blob.GetNodeDefaultData<SetTransformRotationNode>(index);
            return data.RotationProperty.ComponentAccessList
                .Append(ComponentType.ReadWrite<Transform>()
            );
        }
        
        public static NodeState Tick(int index, INodeBlob blob, IBlackboard bb)
        {
            var transform = bb.GetData<Transform>();
            if (transform == null) return NodeState.Failure;
            var rotation = blob.GetNodeData<SetTransformRotationNode>(index).RotationProperty.GetData(index, blob, bb);
            transform.rotation = rotation;
            return NodeState.Success;
        }
    }
}
