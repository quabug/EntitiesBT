using EntitiesBT.Core;
using EntitiesBT.Variant;
using Unity.Entities;
using UnityEngine;

[BehaviorNode("F759A85F-0CEB-4173-8964-DCCC5EC10618")]
public struct PrintTestScopeValues : INodeData
{
    public BlobVariantRO<int> IntValue;
    public BlobVariantRO<float> FloatValue;
    public BlobVariantRO<BlobArray<int>> IntArray;

    public NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb) where TNodeBlob : struct, INodeBlob where TBlackboard : struct, IBlackboard
    {
        Debug.Log(IntValue.Read(index, ref blob, ref bb));
        Debug.Log(FloatValue.Read(index, ref blob, ref bb));
        ref var intArray = ref IntArray.Value.ReadRef<BlobArray<int>, TNodeBlob, TBlackboard>(index, ref blob, ref bb);
        Debug.Log($"[{string.Join(",", intArray.ToArray())}]");
        return NodeState.Success;
    }
}

public class IntArrayScopeVariant : ScopeVariant.Reader<BlobArray<int>> {}
