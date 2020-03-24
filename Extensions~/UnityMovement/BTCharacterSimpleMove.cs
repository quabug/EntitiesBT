using System;
using System.Collections.Generic;
using EntitiesBT.Components;
using EntitiesBT.Core;
using EntitiesBT.Variable;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace EntitiesBT.Extensions.UnityMovement
{
    public class BTCharacterSimpleMove : BTNode<CharacterSimpleMoveNode>
    {
        public bool IsLocal;
        
#if ODIN_INSPECTOR
        [Sirenix.Serialization.OdinSerialize, NonSerialized]
#endif
        public VariableProperty<float3> VelocityProperty;

        protected override void Build(ref CharacterSimpleMoveNode data, BlobBuilder builder, ITreeNode<INodeDataBuilder>[] tree)
        {
            data.IsLocal = IsLocal;
            VelocityProperty.Allocate(ref builder, ref data.Velocity, this, tree);
        }
    }

    [BehaviorNode("21F65017-DCC0-449A-8AE5-E2D296B9E0E5")]
    public struct CharacterSimpleMoveNode : INodeData
    {
        public bool IsLocal;
        public BlobVariable<float3> Velocity;

        [ReadWrite(typeof(CharacterController))]
        public NodeState Tick(int index, INodeBlob blob, IBlackboard bb)
        {
            var controller = bb.GetData<CharacterController>();
            if (controller == null) return NodeState.Failure;
            Vector3 velocity = Velocity.GetData(index, blob, bb);
            controller.SimpleMove(IsLocal ? controller.transform.localToWorldMatrix.MultiplyVector(velocity) : velocity);
            return NodeState.Success;
        }

        public void Reset(int index, INodeBlob blob, IBlackboard blackboard)
        {
        }
    }
}
