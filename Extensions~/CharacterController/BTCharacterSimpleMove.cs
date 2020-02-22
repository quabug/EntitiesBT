using EntitiesBT.Components;
using EntitiesBT.Core;
using EntitiesBT.Entities;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using UnityEngine;

namespace EntitiesBT.Extensions.CharacterController
{
    public class BTCharacterSimpleMove : BTNode
    {
        public bool UseComponentVelocity = true;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ShowInInspector]
        public bool UseCustomVelocity
        {
            get => !UseComponentVelocity;
            set => UseComponentVelocity = !value;
        }
        
        [Sirenix.OdinInspector.ShowIf("UseCustomVelocity")]
#endif
        public Vector3 CustomVelocity;

        public override BehaviorNodeType NodeType => BehaviorNodeType.Action;
        
        public override int NodeId => UseComponentVelocity
            ? typeof(CharacterSimpleMoveWithComponentVelocityNode).GetBehaviorNodeAttribute().Id
            : typeof(CharacterSimpleMoveWithCustomVelocityNode).GetBehaviorNodeAttribute().Id
        ;

        public override unsafe int Size => UseComponentVelocity ? 0 : sizeof(CharacterSimpleMoveWithCustomVelocityNode);
        
        public override unsafe void Build(void* dataPtr, ITreeNode<INodeDataBuilder>[] builders)
        {
            if (!UseComponentVelocity)
                UnsafeUtilityEx.AsRef<CharacterSimpleMoveWithCustomVelocityNode>(dataPtr).Velocity = CustomVelocity;
        }
    }

    [BehaviorTreeComponent]
    public struct BTCharacterSimpleMoveVelocity : IComponentData
    {
        public Vector3 Value;
    }

    [BehaviorNode("F372737E-7671-4F64-B994-253EE3191392")]
    public struct CharacterSimpleMoveWithComponentVelocityNode : INodeData
    {
        public static readonly ComponentType[] Types =
        {
            ComponentType.ReadWrite<UnityEngine.CharacterController>()
          , ComponentType.ReadOnly<BTCharacterSimpleMoveVelocity>()
        };
        
        public static NodeState Tick(int index, INodeBlob blob, IBlackboard bb)
        {
            var controller = bb.GetData<UnityEngine.CharacterController>();
            if (controller == null) return NodeState.Failure;
            controller.SimpleMove(bb.GetData<BTCharacterSimpleMoveVelocity>().Value);
            return NodeState.Running;
        }
    }
    
    [BehaviorNode("21F65017-DCC0-449A-8AE5-E2D296B9E0E5")]
    public struct CharacterSimpleMoveWithCustomVelocityNode : INodeData
    {
        public Vector3 Velocity;
        
        public static readonly ComponentType[] Types = { ComponentType.ReadWrite<UnityEngine.CharacterController>() };
        
        public static NodeState Tick(int index, INodeBlob blob, IBlackboard bb)
        {
            var controller = bb.GetData<UnityEngine.CharacterController>();
            if (controller == null) return NodeState.Failure;
            controller.SimpleMove(blob.GetNodeData<CharacterSimpleMoveWithCustomVelocityNode>(index).Velocity);
            return NodeState.Running;
        }
    }
}
