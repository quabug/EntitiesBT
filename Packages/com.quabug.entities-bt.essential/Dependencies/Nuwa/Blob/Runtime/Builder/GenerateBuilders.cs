





namespace Nuwa.Blob
{

    [Nuwa.Blob.DefaultBuilder] public class BooleanBuilder : Nuwa.Blob.PlainDataBuilder<System.Boolean> {}
    [Nuwa.Blob.DefaultBuilder] public class ByteBuilder : Nuwa.Blob.PlainDataBuilder<System.Byte> {}
    [Nuwa.Blob.DefaultBuilder] public class CharBuilder : Nuwa.Blob.PlainDataBuilder<System.Char> {}
    [Nuwa.Blob.DefaultBuilder] public class DoubleBuilder : Nuwa.Blob.PlainDataBuilder<System.Double> {}
    [Nuwa.Blob.DefaultBuilder] public class Int16Builder : Nuwa.Blob.PlainDataBuilder<System.Int16> {}
    [Nuwa.Blob.DefaultBuilder] public class Int32Builder : Nuwa.Blob.PlainDataBuilder<System.Int32> {}
    [Nuwa.Blob.DefaultBuilder] public class Int64Builder : Nuwa.Blob.PlainDataBuilder<System.Int64> {}
    [Nuwa.Blob.DefaultBuilder] public class IntPtrBuilder : Nuwa.Blob.PlainDataBuilder<System.IntPtr> {}
    [Nuwa.Blob.DefaultBuilder] public class SByteBuilder : Nuwa.Blob.PlainDataBuilder<System.SByte> {}
    [Nuwa.Blob.DefaultBuilder] public class SingleBuilder : Nuwa.Blob.PlainDataBuilder<System.Single> {}
    [Nuwa.Blob.DefaultBuilder] public class UInt16Builder : Nuwa.Blob.PlainDataBuilder<System.UInt16> {}
    [Nuwa.Blob.DefaultBuilder] public class UInt32Builder : Nuwa.Blob.PlainDataBuilder<System.UInt32> {}
    [Nuwa.Blob.DefaultBuilder] public class UInt64Builder : Nuwa.Blob.PlainDataBuilder<System.UInt64> {}
    [Nuwa.Blob.DefaultBuilder] public class UIntPtrBuilder : Nuwa.Blob.PlainDataBuilder<System.UIntPtr> {}

#if ENABLE_UNITY_MATHEMATICS
    [Nuwa.Blob.DefaultBuilder] public class bool2Builder : Nuwa.Blob.PlainDataBuilder<Unity.Mathematics.bool2> {}
    [Nuwa.Blob.DefaultBuilder] public class bool2x2Builder : Nuwa.Blob.PlainDataBuilder<Unity.Mathematics.bool2x2> {}
    [Nuwa.Blob.DefaultBuilder] public class bool2x3Builder : Nuwa.Blob.PlainDataBuilder<Unity.Mathematics.bool2x3> {}
    [Nuwa.Blob.DefaultBuilder] public class bool2x4Builder : Nuwa.Blob.PlainDataBuilder<Unity.Mathematics.bool2x4> {}
    [Nuwa.Blob.DefaultBuilder] public class bool3Builder : Nuwa.Blob.PlainDataBuilder<Unity.Mathematics.bool3> {}
    [Nuwa.Blob.DefaultBuilder] public class bool3x2Builder : Nuwa.Blob.PlainDataBuilder<Unity.Mathematics.bool3x2> {}
    [Nuwa.Blob.DefaultBuilder] public class bool3x3Builder : Nuwa.Blob.PlainDataBuilder<Unity.Mathematics.bool3x3> {}
    [Nuwa.Blob.DefaultBuilder] public class bool3x4Builder : Nuwa.Blob.PlainDataBuilder<Unity.Mathematics.bool3x4> {}
    [Nuwa.Blob.DefaultBuilder] public class bool4Builder : Nuwa.Blob.PlainDataBuilder<Unity.Mathematics.bool4> {}
    [Nuwa.Blob.DefaultBuilder] public class bool4x2Builder : Nuwa.Blob.PlainDataBuilder<Unity.Mathematics.bool4x2> {}
    [Nuwa.Blob.DefaultBuilder] public class bool4x3Builder : Nuwa.Blob.PlainDataBuilder<Unity.Mathematics.bool4x3> {}
    [Nuwa.Blob.DefaultBuilder] public class bool4x4Builder : Nuwa.Blob.PlainDataBuilder<Unity.Mathematics.bool4x4> {}
    [Nuwa.Blob.DefaultBuilder] public class double2Builder : Nuwa.Blob.PlainDataBuilder<Unity.Mathematics.double2> {}
    [Nuwa.Blob.DefaultBuilder] public class double2x2Builder : Nuwa.Blob.PlainDataBuilder<Unity.Mathematics.double2x2> {}
    [Nuwa.Blob.DefaultBuilder] public class double2x3Builder : Nuwa.Blob.PlainDataBuilder<Unity.Mathematics.double2x3> {}
    [Nuwa.Blob.DefaultBuilder] public class double2x4Builder : Nuwa.Blob.PlainDataBuilder<Unity.Mathematics.double2x4> {}
    [Nuwa.Blob.DefaultBuilder] public class double3Builder : Nuwa.Blob.PlainDataBuilder<Unity.Mathematics.double3> {}
    [Nuwa.Blob.DefaultBuilder] public class double3x2Builder : Nuwa.Blob.PlainDataBuilder<Unity.Mathematics.double3x2> {}
    [Nuwa.Blob.DefaultBuilder] public class double3x3Builder : Nuwa.Blob.PlainDataBuilder<Unity.Mathematics.double3x3> {}
    [Nuwa.Blob.DefaultBuilder] public class double3x4Builder : Nuwa.Blob.PlainDataBuilder<Unity.Mathematics.double3x4> {}
    [Nuwa.Blob.DefaultBuilder] public class double4Builder : Nuwa.Blob.PlainDataBuilder<Unity.Mathematics.double4> {}
    [Nuwa.Blob.DefaultBuilder] public class double4x2Builder : Nuwa.Blob.PlainDataBuilder<Unity.Mathematics.double4x2> {}
    [Nuwa.Blob.DefaultBuilder] public class double4x3Builder : Nuwa.Blob.PlainDataBuilder<Unity.Mathematics.double4x3> {}
    [Nuwa.Blob.DefaultBuilder] public class double4x4Builder : Nuwa.Blob.PlainDataBuilder<Unity.Mathematics.double4x4> {}
    [Nuwa.Blob.DefaultBuilder] public class float2Builder : Nuwa.Blob.PlainDataBuilder<Unity.Mathematics.float2> {}
    [Nuwa.Blob.DefaultBuilder] public class float2x2Builder : Nuwa.Blob.PlainDataBuilder<Unity.Mathematics.float2x2> {}
    [Nuwa.Blob.DefaultBuilder] public class float2x3Builder : Nuwa.Blob.PlainDataBuilder<Unity.Mathematics.float2x3> {}
    [Nuwa.Blob.DefaultBuilder] public class float2x4Builder : Nuwa.Blob.PlainDataBuilder<Unity.Mathematics.float2x4> {}
    [Nuwa.Blob.DefaultBuilder] public class float3Builder : Nuwa.Blob.PlainDataBuilder<Unity.Mathematics.float3> {}
    [Nuwa.Blob.DefaultBuilder] public class float3x2Builder : Nuwa.Blob.PlainDataBuilder<Unity.Mathematics.float3x2> {}
    [Nuwa.Blob.DefaultBuilder] public class float3x3Builder : Nuwa.Blob.PlainDataBuilder<Unity.Mathematics.float3x3> {}
    [Nuwa.Blob.DefaultBuilder] public class float3x4Builder : Nuwa.Blob.PlainDataBuilder<Unity.Mathematics.float3x4> {}
    [Nuwa.Blob.DefaultBuilder] public class float4Builder : Nuwa.Blob.PlainDataBuilder<Unity.Mathematics.float4> {}
    [Nuwa.Blob.DefaultBuilder] public class float4x2Builder : Nuwa.Blob.PlainDataBuilder<Unity.Mathematics.float4x2> {}
    [Nuwa.Blob.DefaultBuilder] public class float4x3Builder : Nuwa.Blob.PlainDataBuilder<Unity.Mathematics.float4x3> {}
    [Nuwa.Blob.DefaultBuilder] public class float4x4Builder : Nuwa.Blob.PlainDataBuilder<Unity.Mathematics.float4x4> {}
    [Nuwa.Blob.DefaultBuilder] public class halfBuilder : Nuwa.Blob.PlainDataBuilder<Unity.Mathematics.half> {}
    [Nuwa.Blob.DefaultBuilder] public class half2Builder : Nuwa.Blob.PlainDataBuilder<Unity.Mathematics.half2> {}
    [Nuwa.Blob.DefaultBuilder] public class half3Builder : Nuwa.Blob.PlainDataBuilder<Unity.Mathematics.half3> {}
    [Nuwa.Blob.DefaultBuilder] public class half4Builder : Nuwa.Blob.PlainDataBuilder<Unity.Mathematics.half4> {}
    [Nuwa.Blob.DefaultBuilder] public class int2Builder : Nuwa.Blob.PlainDataBuilder<Unity.Mathematics.int2> {}
    [Nuwa.Blob.DefaultBuilder] public class int2x2Builder : Nuwa.Blob.PlainDataBuilder<Unity.Mathematics.int2x2> {}
    [Nuwa.Blob.DefaultBuilder] public class int2x3Builder : Nuwa.Blob.PlainDataBuilder<Unity.Mathematics.int2x3> {}
    [Nuwa.Blob.DefaultBuilder] public class int2x4Builder : Nuwa.Blob.PlainDataBuilder<Unity.Mathematics.int2x4> {}
    [Nuwa.Blob.DefaultBuilder] public class int3Builder : Nuwa.Blob.PlainDataBuilder<Unity.Mathematics.int3> {}
    [Nuwa.Blob.DefaultBuilder] public class int3x2Builder : Nuwa.Blob.PlainDataBuilder<Unity.Mathematics.int3x2> {}
    [Nuwa.Blob.DefaultBuilder] public class int3x3Builder : Nuwa.Blob.PlainDataBuilder<Unity.Mathematics.int3x3> {}
    [Nuwa.Blob.DefaultBuilder] public class int3x4Builder : Nuwa.Blob.PlainDataBuilder<Unity.Mathematics.int3x4> {}
    [Nuwa.Blob.DefaultBuilder] public class int4Builder : Nuwa.Blob.PlainDataBuilder<Unity.Mathematics.int4> {}
    [Nuwa.Blob.DefaultBuilder] public class int4x2Builder : Nuwa.Blob.PlainDataBuilder<Unity.Mathematics.int4x2> {}
    [Nuwa.Blob.DefaultBuilder] public class int4x3Builder : Nuwa.Blob.PlainDataBuilder<Unity.Mathematics.int4x3> {}
    [Nuwa.Blob.DefaultBuilder] public class int4x4Builder : Nuwa.Blob.PlainDataBuilder<Unity.Mathematics.int4x4> {}
    [Nuwa.Blob.DefaultBuilder] public class quaternionBuilder : Nuwa.Blob.PlainDataBuilder<Unity.Mathematics.quaternion> {}
    [Nuwa.Blob.DefaultBuilder] public class RandomBuilder : Nuwa.Blob.PlainDataBuilder<Unity.Mathematics.Random> {}
    [Nuwa.Blob.DefaultBuilder] public class RigidTransformBuilder : Nuwa.Blob.PlainDataBuilder<Unity.Mathematics.RigidTransform> {}
    [Nuwa.Blob.DefaultBuilder] public class uint2Builder : Nuwa.Blob.PlainDataBuilder<Unity.Mathematics.uint2> {}
    [Nuwa.Blob.DefaultBuilder] public class uint2x2Builder : Nuwa.Blob.PlainDataBuilder<Unity.Mathematics.uint2x2> {}
    [Nuwa.Blob.DefaultBuilder] public class uint2x3Builder : Nuwa.Blob.PlainDataBuilder<Unity.Mathematics.uint2x3> {}
    [Nuwa.Blob.DefaultBuilder] public class uint2x4Builder : Nuwa.Blob.PlainDataBuilder<Unity.Mathematics.uint2x4> {}
    [Nuwa.Blob.DefaultBuilder] public class uint3Builder : Nuwa.Blob.PlainDataBuilder<Unity.Mathematics.uint3> {}
    [Nuwa.Blob.DefaultBuilder] public class uint3x2Builder : Nuwa.Blob.PlainDataBuilder<Unity.Mathematics.uint3x2> {}
    [Nuwa.Blob.DefaultBuilder] public class uint3x3Builder : Nuwa.Blob.PlainDataBuilder<Unity.Mathematics.uint3x3> {}
    [Nuwa.Blob.DefaultBuilder] public class uint3x4Builder : Nuwa.Blob.PlainDataBuilder<Unity.Mathematics.uint3x4> {}
    [Nuwa.Blob.DefaultBuilder] public class uint4Builder : Nuwa.Blob.PlainDataBuilder<Unity.Mathematics.uint4> {}
    [Nuwa.Blob.DefaultBuilder] public class uint4x2Builder : Nuwa.Blob.PlainDataBuilder<Unity.Mathematics.uint4x2> {}
    [Nuwa.Blob.DefaultBuilder] public class uint4x3Builder : Nuwa.Blob.PlainDataBuilder<Unity.Mathematics.uint4x3> {}
    [Nuwa.Blob.DefaultBuilder] public class uint4x4Builder : Nuwa.Blob.PlainDataBuilder<Unity.Mathematics.uint4x4> {}
#endif

}

