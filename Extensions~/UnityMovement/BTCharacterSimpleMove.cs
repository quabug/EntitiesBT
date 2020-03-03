using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using EntitiesBT.Components;
using EntitiesBT.Core;
using EntitiesBT.Entities;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using UnityEngine;

namespace EntitiesBT.Extensions.UnityMovement
{
    public class BTCharacterSimpleMove : BTNode
    {
        enum VelocitySource
        {
            Component,
            CustomGlobal,
            CustomLocal
        }
        
        [SerializeField] private VelocitySource Source = VelocitySource.Component;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.HideIf("Source", VelocitySource.Component)]
#endif
        public Vector3 CustomVelocity;

        protected override Type NodeType
        {
            get
            {
                switch (Source)
                {
                case VelocitySource.Component:
                    return typeof(CharacterSimpleMoveWithComponentVelocityNode);
                case VelocitySource.CustomGlobal:
                    return typeof(CharacterSimpleMoveWithCustomGlobalVelocityNode);
                case VelocitySource.CustomLocal:
                    return typeof(CharacterSimpleMoveWithCustomLocalVelocityNode);
                default:
                    throw new ArgumentOutOfRangeException();
                }
            }
        }

        protected override unsafe void Build(void* dataPtr, BlobBuilder _, ITreeNode<INodeDataBuilder>[] __)
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
        public static IEnumerable<ComponentType> AccessTypes(int index, INodeBlob blobs)
        {
            yield return ComponentType.ReadWrite<CharacterController>();
            yield return ComponentType.ReadOnly<BTCharacterSimpleMoveVelocity>();
        }
        
        public static NodeState Tick(int index, INodeBlob blob, IBlackboard bb)
        {
            var controller = bb.GetData<CharacterController>();
            if (controller == null) return NodeState.Failure;
            controller.SimpleMove(bb.GetData<BTCharacterSimpleMoveVelocity>().Value);
            return NodeState.Success;
        }
    }
    
    [BehaviorNode("21F65017-DCC0-449A-8AE5-E2D296B9E0E5")]
    public struct CharacterSimpleMoveWithCustomGlobalVelocityNode : INodeData
    {
        public Vector3 Velocity;

        public static IEnumerable<ComponentType> AccessTypes(int index, INodeBlob blob)
        {
            yield return ComponentType.ReadWrite<CharacterController>();
        }
        
        public static NodeState Tick(int index, INodeBlob blob, IBlackboard bb)
        {
            var controller = bb.GetData<CharacterController>();
            if (controller == null) return NodeState.Failure;
            controller.SimpleMove(blob.GetNodeData<CharacterSimpleMoveWithCustomGlobalVelocityNode>(index).Velocity);
            return NodeState.Success;
        }
    }
    
    [BehaviorNode("63703866-6C31-4C11-A870-CFDCC96D20D4")]
    public struct CharacterSimpleMoveWithCustomLocalVelocityNode : INodeData
    {
        public Vector3 Velocity;

        public static IEnumerable<ComponentType> AccessTypes(int index, INodeBlob blob)
        {
            yield return ComponentType.ReadWrite<CharacterController>();
        }
        
        public static NodeState Tick(int index, INodeBlob blob, IBlackboard bb)
        {
            var controller = bb.GetData<CharacterController>();
            if (controller == null) return NodeState.Failure;
            var velocity = blob.GetNodeData<CharacterSimpleMoveWithCustomLocalVelocityNode>(index).Velocity;
            velocity = controller.transform.localToWorldMatrix.MultiplyVector(velocity);
            controller.SimpleMove(velocity);
            return NodeState.Success;
        }
    }
}
