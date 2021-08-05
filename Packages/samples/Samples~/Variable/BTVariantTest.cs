using System.Collections.Generic;
using EntitiesBT.Components;
using EntitiesBT.Core;
using EntitiesBT.Variant;
using Unity.Entities;
using UnityEngine;

namespace EntitiesBT.Sample
{
    [BehaviorNode("867BFC14-4293-4D4E-B3F0-280AD4BAA403")]
    public struct VariablesTestNode : INodeData
    {
        [Optional] public BlobVariantRO<long> LongReader;
        public BlobString String;
        public BlobArray<int> IntArray;
        public BlobVariantWO<long> LongWriter;
        public BlobVariantRO<float> SingleReader;
        public BlobVariantRW<float> SingleReaderAndWriter;
        public long Long;

        public NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            if (LongReader.IsValid) Debug.Log(LongReader.Read(index,ref blob, ref bb));
            else Debug.Log($"invalid {nameof(LongReader)}");
            Debug.Log(String.ToString());
            // Debug.Log(string.Join(",", IntArray.ToArray()));
            Debug.Log(SingleReader.Read(index, ref blob, ref bb));
            Debug.Log(SingleReaderAndWriter.Read(index, ref blob, ref bb));
            LongWriter.Write(index, ref blob, ref bb, 100);
            SingleReaderAndWriter.Write(index, ref blob, ref bb, 200);

            if (LongReader.IsValid) Debug.Log(LongReader.Read(index,ref blob, ref bb));
            else Debug.Log($"invalid {nameof(LongReader)}");
            Debug.Log(String.ToString());
            // Debug.Log(string.Join(",", IntArray.ToArray()));
            Debug.Log(SingleReader.Read(index, ref blob, ref bb));
            Debug.Log(SingleReaderAndWriter.Read(index, ref blob, ref bb));

            return NodeState.Success;
        }
    }
}
