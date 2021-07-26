#if ODIN_INSPECTOR

using EntitiesBT.Core;
using Unity.Entities;
using Sirenix.Serialization;
using System;
using EntitiesBT.Variant;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;

namespace EntitiesBT.Components.Odin
{
    public class OdinPrint : OdinNode<PrintNode>
    {
        [OdinSerializeAttribute, NonSerializedAttribute, HideReferenceObjectPickerAttribute]
        public OdinSerializedVariantReader<float2> TimerSeconds;

        protected override void Build(ref PrintNode data, BlobBuilder builder, ITreeNode<INodeDataBuilder>[] tree)
        {
            TimerSeconds.Allocate(ref builder, ref data.Value, this, tree);
        }
    }

    [Serializable]
    [BehaviorNode("30E1D8D2-356D-422A-AF98-1CB734C3C63D")]
    public struct PrintNode : INodeData
    {
        public BlobVariantReader<float2> Value;

        public NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            Debug.Log(Value.Read(index, ref blob, ref bb));
            return NodeState.Success;
        }

        public void Reset<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard blackboard)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
        }
    }
}

#endif
