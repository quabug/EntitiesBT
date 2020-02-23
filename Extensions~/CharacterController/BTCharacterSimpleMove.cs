using System;
using System.Runtime.InteropServices;
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
        public enum VelocitySource
        {
            Component,
            CustomGlobal,
            CustomLocal
        }
        
        public VelocitySource Source = VelocitySource.Component;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.HideIf("Source", VelocitySource.Component)]
#endif
        public Vector3 CustomVelocity;

        public override BehaviorNodeType NodeType => BehaviorNodeType.Action;
        
        public override int NodeId
        {
            get
            {
                switch (Source)
                {
                case VelocitySource.Component:
                    return typeof(CharacterSimpleMoveWithComponentVelocityNode).GetBehaviorNodeAttribute().Id;
                case VelocitySource.CustomGlobal:
                    return typeof(CharacterSimpleMoveWithCustomGlobalVelocityNode).GetBehaviorNodeAttribute().Id;
                case VelocitySource.CustomLocal:
                    return typeof(CharacterSimpleMoveWithCustomLocalVelocityNode).GetBehaviorNodeAttribute().Id;
                default:
                    throw new ArgumentOutOfRangeException();
                }
            }
        }

        public override unsafe int Size 
        {
            get
            {
                switch (Source)
                {
                case VelocitySource.Component:
                    return sizeof(CharacterSimpleMoveWithComponentVelocityNode);
                case VelocitySource.CustomGlobal:
                    return sizeof(CharacterSimpleMoveWithCustomGlobalVelocityNode);
                case VelocitySource.CustomLocal:
                    return sizeof(CharacterSimpleMoveWithCustomLocalVelocityNode);
                default:
                    throw new ArgumentOutOfRangeException();
                }
            }
        }
        
        public override unsafe void Build(void* dataPtr, ITreeNode<INodeDataBuilder>[] builders)
        {
                
            switch (Source)
            {
            case VelocitySource.Component:
                return;
            case VelocitySource.CustomGlobal:
                UnsafeUtilityEx.AsRef<CharacterSimpleMoveWithCustomGlobalVelocityNode>(dataPtr).Velocity = CustomVelocity;
                break;
            case VelocitySource.CustomLocal:
                UnsafeUtilityEx.AsRef<CharacterSimpleMoveWithCustomLocalVelocityNode>(dataPtr).Velocity = CustomVelocity;
                break;
            default:
                throw new ArgumentOutOfRangeException();
            }
        }
    }

    [BehaviorTreeComponent]
    public struct BTCharacterSimpleMoveVelocity : IComponentData
    {
        public Vector3 Value;
    }

    [StructLayout(LayoutKind.Explicit), BehaviorNode("F372737E-7671-4F64-B994-253EE3191392")]
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
            return NodeState.Success;
        }
    }
    
    [BehaviorNode("21F65017-DCC0-449A-8AE5-E2D296B9E0E5")]
    public struct CharacterSimpleMoveWithCustomGlobalVelocityNode : INodeData
    {
        public Vector3 Velocity;
        
        public static readonly ComponentType[] Types = { ComponentType.ReadWrite<UnityEngine.CharacterController>() };
        
        public static NodeState Tick(int index, INodeBlob blob, IBlackboard bb)
        {
            var controller = bb.GetData<UnityEngine.CharacterController>();
            if (controller == null) return NodeState.Failure;
            controller.SimpleMove(blob.GetNodeData<CharacterSimpleMoveWithCustomGlobalVelocityNode>(index).Velocity);
            return NodeState.Success;
        }
    }
    
    [BehaviorNode("63703866-6C31-4C11-A870-CFDCC96D20D4")]
    public struct CharacterSimpleMoveWithCustomLocalVelocityNode : INodeData
    {
        public Vector3 Velocity;
        
        public static readonly ComponentType[] Types = { ComponentType.ReadWrite<UnityEngine.CharacterController>() };
        
        public static NodeState Tick(int index, INodeBlob blob, IBlackboard bb)
        {
            var controller = bb.GetData<UnityEngine.CharacterController>();
            if (controller == null) return NodeState.Failure;
            var velocity = blob.GetNodeData<CharacterSimpleMoveWithCustomLocalVelocityNode>(index).Velocity;
            velocity = controller.transform.worldToLocalMatrix.MultiplyVector(velocity);
            controller.SimpleMove(velocity);
            return NodeState.Success;
        }
    }
}
