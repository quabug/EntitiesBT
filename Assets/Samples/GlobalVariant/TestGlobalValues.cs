using EntitiesBT.Core;
using EntitiesBT.Variant;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;

public struct TestValues : IGlobalValuesBlob
{
    public int IntValue;
    public int IntValue2;
    public float FloatValue;
    public BlobArray<int> IntArray;
    public int Size => UnsafeUtility.SizeOf<TestValues>() + sizeof(int) * IntArray.Length;
}

public class TestGlobalValues : GlobalValues<TestValues> {}
