





namespace Nuwa.Blob
{

    public class BooleanBuilder : Blob.PlainDataBuilder<System.Boolean> {}
    public class ByteBuilder : Blob.PlainDataBuilder<System.Byte> {}
    public class CharBuilder : Blob.PlainDataBuilder<System.Char> {}
    public class DoubleBuilder : Blob.PlainDataBuilder<System.Double> {}
    public class Int16Builder : Blob.PlainDataBuilder<System.Int16> {}
    public class Int32Builder : Blob.PlainDataBuilder<System.Int32> {}
    public class Int64Builder : Blob.PlainDataBuilder<System.Int64> {}
    public class IntPtrBuilder : Blob.PlainDataBuilder<System.IntPtr> {}
    public class SByteBuilder : Blob.PlainDataBuilder<System.SByte> {}
    public class SingleBuilder : Blob.PlainDataBuilder<System.Single> {}
    public class UInt16Builder : Blob.PlainDataBuilder<System.UInt16> {}
    public class UInt32Builder : Blob.PlainDataBuilder<System.UInt32> {}
    public class UInt64Builder : Blob.PlainDataBuilder<System.UInt64> {}
    public class UIntPtrBuilder : Blob.PlainDataBuilder<System.UIntPtr> {}

#if ENABLE_UNITY_MATHEMATICS
    public class bool2Builder : Blob.PlainDataBuilder<Unity.Mathematics.bool2> {}
    public class bool2x2Builder : Blob.PlainDataBuilder<Unity.Mathematics.bool2x2> {}
    public class bool2x3Builder : Blob.PlainDataBuilder<Unity.Mathematics.bool2x3> {}
    public class bool2x4Builder : Blob.PlainDataBuilder<Unity.Mathematics.bool2x4> {}
    public class bool3Builder : Blob.PlainDataBuilder<Unity.Mathematics.bool3> {}
    public class bool3x2Builder : Blob.PlainDataBuilder<Unity.Mathematics.bool3x2> {}
    public class bool3x3Builder : Blob.PlainDataBuilder<Unity.Mathematics.bool3x3> {}
    public class bool3x4Builder : Blob.PlainDataBuilder<Unity.Mathematics.bool3x4> {}
    public class bool4Builder : Blob.PlainDataBuilder<Unity.Mathematics.bool4> {}
    public class bool4x2Builder : Blob.PlainDataBuilder<Unity.Mathematics.bool4x2> {}
    public class bool4x3Builder : Blob.PlainDataBuilder<Unity.Mathematics.bool4x3> {}
    public class bool4x4Builder : Blob.PlainDataBuilder<Unity.Mathematics.bool4x4> {}
    public class double2Builder : Blob.PlainDataBuilder<Unity.Mathematics.double2> {}
    public class double2x2Builder : Blob.PlainDataBuilder<Unity.Mathematics.double2x2> {}
    public class double2x3Builder : Blob.PlainDataBuilder<Unity.Mathematics.double2x3> {}
    public class double2x4Builder : Blob.PlainDataBuilder<Unity.Mathematics.double2x4> {}
    public class double3Builder : Blob.PlainDataBuilder<Unity.Mathematics.double3> {}
    public class double3x2Builder : Blob.PlainDataBuilder<Unity.Mathematics.double3x2> {}
    public class double3x3Builder : Blob.PlainDataBuilder<Unity.Mathematics.double3x3> {}
    public class double3x4Builder : Blob.PlainDataBuilder<Unity.Mathematics.double3x4> {}
    public class double4Builder : Blob.PlainDataBuilder<Unity.Mathematics.double4> {}
    public class double4x2Builder : Blob.PlainDataBuilder<Unity.Mathematics.double4x2> {}
    public class double4x3Builder : Blob.PlainDataBuilder<Unity.Mathematics.double4x3> {}
    public class double4x4Builder : Blob.PlainDataBuilder<Unity.Mathematics.double4x4> {}
    public class float2Builder : Blob.PlainDataBuilder<Unity.Mathematics.float2> {}
    public class float2x2Builder : Blob.PlainDataBuilder<Unity.Mathematics.float2x2> {}
    public class float2x3Builder : Blob.PlainDataBuilder<Unity.Mathematics.float2x3> {}
    public class float2x4Builder : Blob.PlainDataBuilder<Unity.Mathematics.float2x4> {}
    public class float3Builder : Blob.PlainDataBuilder<Unity.Mathematics.float3> {}
    public class float3x2Builder : Blob.PlainDataBuilder<Unity.Mathematics.float3x2> {}
    public class float3x3Builder : Blob.PlainDataBuilder<Unity.Mathematics.float3x3> {}
    public class float3x4Builder : Blob.PlainDataBuilder<Unity.Mathematics.float3x4> {}
    public class float4Builder : Blob.PlainDataBuilder<Unity.Mathematics.float4> {}
    public class float4x2Builder : Blob.PlainDataBuilder<Unity.Mathematics.float4x2> {}
    public class float4x3Builder : Blob.PlainDataBuilder<Unity.Mathematics.float4x3> {}
    public class float4x4Builder : Blob.PlainDataBuilder<Unity.Mathematics.float4x4> {}
    public class halfBuilder : Blob.PlainDataBuilder<Unity.Mathematics.half> {}
    public class half2Builder : Blob.PlainDataBuilder<Unity.Mathematics.half2> {}
    public class half3Builder : Blob.PlainDataBuilder<Unity.Mathematics.half3> {}
    public class half4Builder : Blob.PlainDataBuilder<Unity.Mathematics.half4> {}
    public class int2Builder : Blob.PlainDataBuilder<Unity.Mathematics.int2> {}
    public class int2x2Builder : Blob.PlainDataBuilder<Unity.Mathematics.int2x2> {}
    public class int2x3Builder : Blob.PlainDataBuilder<Unity.Mathematics.int2x3> {}
    public class int2x4Builder : Blob.PlainDataBuilder<Unity.Mathematics.int2x4> {}
    public class int3Builder : Blob.PlainDataBuilder<Unity.Mathematics.int3> {}
    public class int3x2Builder : Blob.PlainDataBuilder<Unity.Mathematics.int3x2> {}
    public class int3x3Builder : Blob.PlainDataBuilder<Unity.Mathematics.int3x3> {}
    public class int3x4Builder : Blob.PlainDataBuilder<Unity.Mathematics.int3x4> {}
    public class int4Builder : Blob.PlainDataBuilder<Unity.Mathematics.int4> {}
    public class int4x2Builder : Blob.PlainDataBuilder<Unity.Mathematics.int4x2> {}
    public class int4x3Builder : Blob.PlainDataBuilder<Unity.Mathematics.int4x3> {}
    public class int4x4Builder : Blob.PlainDataBuilder<Unity.Mathematics.int4x4> {}
    public class quaternionBuilder : Blob.PlainDataBuilder<Unity.Mathematics.quaternion> {}
    public class RandomBuilder : Blob.PlainDataBuilder<Unity.Mathematics.Random> {}
    public class RigidTransformBuilder : Blob.PlainDataBuilder<Unity.Mathematics.RigidTransform> {}
    public class uint2Builder : Blob.PlainDataBuilder<Unity.Mathematics.uint2> {}
    public class uint2x2Builder : Blob.PlainDataBuilder<Unity.Mathematics.uint2x2> {}
    public class uint2x3Builder : Blob.PlainDataBuilder<Unity.Mathematics.uint2x3> {}
    public class uint2x4Builder : Blob.PlainDataBuilder<Unity.Mathematics.uint2x4> {}
    public class uint3Builder : Blob.PlainDataBuilder<Unity.Mathematics.uint3> {}
    public class uint3x2Builder : Blob.PlainDataBuilder<Unity.Mathematics.uint3x2> {}
    public class uint3x3Builder : Blob.PlainDataBuilder<Unity.Mathematics.uint3x3> {}
    public class uint3x4Builder : Blob.PlainDataBuilder<Unity.Mathematics.uint3x4> {}
    public class uint4Builder : Blob.PlainDataBuilder<Unity.Mathematics.uint4> {}
    public class uint4x2Builder : Blob.PlainDataBuilder<Unity.Mathematics.uint4x2> {}
    public class uint4x3Builder : Blob.PlainDataBuilder<Unity.Mathematics.uint4x3> {}
    public class uint4x4Builder : Blob.PlainDataBuilder<Unity.Mathematics.uint4x4> {}
#endif

}

