using System;
using System.Runtime.InteropServices;
using EntitiesBT.Components;
using EntitiesBT.Core;
using EntitiesBT.Entities;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace EntitiesBT.Extensions.UnityMovement
{
    public class BTSetTransformRotation : BTNode
    {
        enum SourceType
        {
            Component,
            Custom
        }
        
        [SerializeField] private SourceType _source = default;
        
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ShowIf("Source", SourceType.Custom)]
#endif
        public float3 CustomRotation;

        protected override Type NodeType
        {
            get
            {
                switch (_source)
                {
                case SourceType.Component:
                    return typeof(SetTransformRotationWithComponentRotationNode);
                case SourceType.Custom:
                    return typeof(SetTransformRotationWithCustomRotationNode);
                default:
                    throw new ArgumentOutOfRangeException();
                }
            }
        }

        protected override unsafe void Build(void* dataPtr, BlobBuilder _, ITreeNode<INodeDataBuilder>[] __)
        {
            switch (_source)
            {
            case SourceType.Component:
                break;
            case SourceType.Custom:
            {
                ref var data = ref UnsafeUtilityEx.AsRef<SetTransformRotationWithCustomRotationNode>(dataPtr);
                data.Rotation = quaternion.Euler(CustomRotation);
                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
            }
        }
    }

    [BehaviorTreeComponent]
    public struct BTTransformRotationData : IComponentData
    {
        public quaternion Value;
    }
    
    [StructLayout(LayoutKind.Explicit), BehaviorNode("7FCFF548-4D65-402A-B885-20633923DC22")]
    public struct SetTransformRotationWithComponentRotationNode : INodeData
    {
        public static readonly ComponentType[] Types =
        {
            ComponentType.ReadWrite<Transform>()
          , ComponentType.ReadOnly<BTTransformRotationData>()
        };
        
        public static NodeState Tick(int index, INodeBlob blob, IBlackboard bb)
        {
            var transform = bb.GetData<Transform>();
            if (transform == null) return NodeState.Failure;
            var rotation = bb.GetData<BTTransformRotationData>().Value;
            transform.rotation = rotation;
            return NodeState.Success;
        }
    }
    
    [BehaviorNode("A67A8C28-F623-454C-8BCF-12EE8D38BC7F")]
    public struct SetTransformRotationWithCustomRotationNode : INodeData
    {
        public quaternion Rotation;
        
        public static readonly ComponentType[] Types =
        {
            ComponentType.ReadWrite<Transform>()
          , ComponentType.ReadOnly<BTTransformRotationData>()
        };
        
        public static NodeState Tick(int index, INodeBlob blob, IBlackboard bb)
        {
            var transform = bb.GetData<Transform>();
            if (transform == null) return NodeState.Failure;
            var rotation = blob.GetNodeData<SetTransformRotationWithCustomRotationNode>(index).Rotation;
            transform.rotation = rotation;
            return NodeState.Success;
        }
    }
}
