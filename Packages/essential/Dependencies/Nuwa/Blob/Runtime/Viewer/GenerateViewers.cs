





namespace Nuwa.Blob
{

    public class BooleanViewer : Blob.PlainDataViewer<System.Boolean> {}
    public class ByteViewer : Blob.PlainDataViewer<System.Byte> {}
    public class CharViewer : Blob.PlainDataViewer<System.Char> {}
    public class DoubleViewer : Blob.PlainDataViewer<System.Double> {}
    public class Int16Viewer : Blob.PlainDataViewer<System.Int16> {}
    public class Int32Viewer : Blob.PlainDataViewer<System.Int32> {}
    public class Int64Viewer : Blob.PlainDataViewer<System.Int64> {}
    public class IntPtrViewer : Blob.PlainDataViewer<System.IntPtr> {}
    public class SByteViewer : Blob.PlainDataViewer<System.SByte> {}
    public class SingleViewer : Blob.PlainDataViewer<System.Single> {}
    public class UInt16Viewer : Blob.PlainDataViewer<System.UInt16> {}
    public class UInt32Viewer : Blob.PlainDataViewer<System.UInt32> {}
    public class UInt64Viewer : Blob.PlainDataViewer<System.UInt64> {}
    public class UIntPtrViewer : Blob.PlainDataViewer<System.UIntPtr> {}

#if ENABLE_UNITY_MATHEMATICS
    public class bool2Viewer : Blob.PlainDataViewer<Unity.Mathematics.bool2> {}
    public class bool2x2Viewer : Blob.PlainDataViewer<Unity.Mathematics.bool2x2> {}
    public class bool2x3Viewer : Blob.PlainDataViewer<Unity.Mathematics.bool2x3> {}
    public class bool2x4Viewer : Blob.PlainDataViewer<Unity.Mathematics.bool2x4> {}
    public class bool3Viewer : Blob.PlainDataViewer<Unity.Mathematics.bool3> {}
    public class bool3x2Viewer : Blob.PlainDataViewer<Unity.Mathematics.bool3x2> {}
    public class bool3x3Viewer : Blob.PlainDataViewer<Unity.Mathematics.bool3x3> {}
    public class bool3x4Viewer : Blob.PlainDataViewer<Unity.Mathematics.bool3x4> {}
    public class bool4Viewer : Blob.PlainDataViewer<Unity.Mathematics.bool4> {}
    public class bool4x2Viewer : Blob.PlainDataViewer<Unity.Mathematics.bool4x2> {}
    public class bool4x3Viewer : Blob.PlainDataViewer<Unity.Mathematics.bool4x3> {}
    public class bool4x4Viewer : Blob.PlainDataViewer<Unity.Mathematics.bool4x4> {}
    public class double2Viewer : Blob.PlainDataViewer<Unity.Mathematics.double2> {}
    public class double2x2Viewer : Blob.PlainDataViewer<Unity.Mathematics.double2x2> {}
    public class double2x3Viewer : Blob.PlainDataViewer<Unity.Mathematics.double2x3> {}
    public class double2x4Viewer : Blob.PlainDataViewer<Unity.Mathematics.double2x4> {}
    public class double3Viewer : Blob.PlainDataViewer<Unity.Mathematics.double3> {}
    public class double3x2Viewer : Blob.PlainDataViewer<Unity.Mathematics.double3x2> {}
    public class double3x3Viewer : Blob.PlainDataViewer<Unity.Mathematics.double3x3> {}
    public class double3x4Viewer : Blob.PlainDataViewer<Unity.Mathematics.double3x4> {}
    public class double4Viewer : Blob.PlainDataViewer<Unity.Mathematics.double4> {}
    public class double4x2Viewer : Blob.PlainDataViewer<Unity.Mathematics.double4x2> {}
    public class double4x3Viewer : Blob.PlainDataViewer<Unity.Mathematics.double4x3> {}
    public class double4x4Viewer : Blob.PlainDataViewer<Unity.Mathematics.double4x4> {}
    public class float2Viewer : Blob.PlainDataViewer<Unity.Mathematics.float2> {}
    public class float2x2Viewer : Blob.PlainDataViewer<Unity.Mathematics.float2x2> {}
    public class float2x3Viewer : Blob.PlainDataViewer<Unity.Mathematics.float2x3> {}
    public class float2x4Viewer : Blob.PlainDataViewer<Unity.Mathematics.float2x4> {}
    public class float3Viewer : Blob.PlainDataViewer<Unity.Mathematics.float3> {}
    public class float3x2Viewer : Blob.PlainDataViewer<Unity.Mathematics.float3x2> {}
    public class float3x3Viewer : Blob.PlainDataViewer<Unity.Mathematics.float3x3> {}
    public class float3x4Viewer : Blob.PlainDataViewer<Unity.Mathematics.float3x4> {}
    public class float4Viewer : Blob.PlainDataViewer<Unity.Mathematics.float4> {}
    public class float4x2Viewer : Blob.PlainDataViewer<Unity.Mathematics.float4x2> {}
    public class float4x3Viewer : Blob.PlainDataViewer<Unity.Mathematics.float4x3> {}
    public class float4x4Viewer : Blob.PlainDataViewer<Unity.Mathematics.float4x4> {}
    public class halfViewer : Blob.PlainDataViewer<Unity.Mathematics.half> {}
    public class half2Viewer : Blob.PlainDataViewer<Unity.Mathematics.half2> {}
    public class half3Viewer : Blob.PlainDataViewer<Unity.Mathematics.half3> {}
    public class half4Viewer : Blob.PlainDataViewer<Unity.Mathematics.half4> {}
    public class int2Viewer : Blob.PlainDataViewer<Unity.Mathematics.int2> {}
    public class int2x2Viewer : Blob.PlainDataViewer<Unity.Mathematics.int2x2> {}
    public class int2x3Viewer : Blob.PlainDataViewer<Unity.Mathematics.int2x3> {}
    public class int2x4Viewer : Blob.PlainDataViewer<Unity.Mathematics.int2x4> {}
    public class int3Viewer : Blob.PlainDataViewer<Unity.Mathematics.int3> {}
    public class int3x2Viewer : Blob.PlainDataViewer<Unity.Mathematics.int3x2> {}
    public class int3x3Viewer : Blob.PlainDataViewer<Unity.Mathematics.int3x3> {}
    public class int3x4Viewer : Blob.PlainDataViewer<Unity.Mathematics.int3x4> {}
    public class int4Viewer : Blob.PlainDataViewer<Unity.Mathematics.int4> {}
    public class int4x2Viewer : Blob.PlainDataViewer<Unity.Mathematics.int4x2> {}
    public class int4x3Viewer : Blob.PlainDataViewer<Unity.Mathematics.int4x3> {}
    public class int4x4Viewer : Blob.PlainDataViewer<Unity.Mathematics.int4x4> {}
    public class quaternionViewer : Blob.PlainDataViewer<Unity.Mathematics.quaternion> {}
    public class RandomViewer : Blob.PlainDataViewer<Unity.Mathematics.Random> {}
    public class RigidTransformViewer : Blob.PlainDataViewer<Unity.Mathematics.RigidTransform> {}
    public class uint2Viewer : Blob.PlainDataViewer<Unity.Mathematics.uint2> {}
    public class uint2x2Viewer : Blob.PlainDataViewer<Unity.Mathematics.uint2x2> {}
    public class uint2x3Viewer : Blob.PlainDataViewer<Unity.Mathematics.uint2x3> {}
    public class uint2x4Viewer : Blob.PlainDataViewer<Unity.Mathematics.uint2x4> {}
    public class uint3Viewer : Blob.PlainDataViewer<Unity.Mathematics.uint3> {}
    public class uint3x2Viewer : Blob.PlainDataViewer<Unity.Mathematics.uint3x2> {}
    public class uint3x3Viewer : Blob.PlainDataViewer<Unity.Mathematics.uint3x3> {}
    public class uint3x4Viewer : Blob.PlainDataViewer<Unity.Mathematics.uint3x4> {}
    public class uint4Viewer : Blob.PlainDataViewer<Unity.Mathematics.uint4> {}
    public class uint4x2Viewer : Blob.PlainDataViewer<Unity.Mathematics.uint4x2> {}
    public class uint4x3Viewer : Blob.PlainDataViewer<Unity.Mathematics.uint4x3> {}
    public class uint4x4Viewer : Blob.PlainDataViewer<Unity.Mathematics.uint4x4> {}
#endif

}

