using EntitiesBT.Variant;
using Unity.Mathematics;

[assembly: VariantValueType(typeof(byte), 0)]
[assembly: VariantValueType(typeof(short), 1)]
[assembly: VariantValueType(typeof(int), 2)]
[assembly: VariantValueType(typeof(long), 3)]
[assembly: VariantValueType(typeof(sbyte), 4)]
[assembly: VariantValueType(typeof(ushort), 5)]
[assembly: VariantValueType(typeof(uint), 6)]
[assembly: VariantValueType(typeof(ulong), 7)]
[assembly: VariantValueType(typeof(float), 8)]
[assembly: VariantValueType(typeof(double), 9)]
[assembly: VariantValueType(typeof(bool), 10)]
// [assembly: VariantValueType(typeof(bool2), 20)]
// [assembly: VariantValueType(typeof(bool2x2), 21)]
// [assembly: VariantValueType(typeof(bool2x3), 22)]
// [assembly: VariantValueType(typeof(bool2x4), 23)]
// [assembly: VariantValueType(typeof(bool3), 24)]
// [assembly: VariantValueType(typeof(bool3x2), 25)]
// [assembly: VariantValueType(typeof(bool3x3), 26)]
// [assembly: VariantValueType(typeof(bool3x4), 27)]
// [assembly: VariantValueType(typeof(bool4), 28)]
// [assembly: VariantValueType(typeof(bool4x2), 29)]
// [assembly: VariantValueType(typeof(bool4x3), 30)]
// [assembly: VariantValueType(typeof(bool4x4), 31)]
// [assembly: VariantValueType(typeof(int2), 32)]
// [assembly: VariantValueType(typeof(int2x2), 33)]
// [assembly: VariantValueType(typeof(int2x3), 34)]
// [assembly: VariantValueType(typeof(int2x4), 35)]
// [assembly: VariantValueType(typeof(int3), 36)]
// [assembly: VariantValueType(typeof(int3x2), 37)]
// [assembly: VariantValueType(typeof(int3x3), 38)]
// [assembly: VariantValueType(typeof(int3x4), 39)]
// [assembly: VariantValueType(typeof(int4), 40)]
// [assembly: VariantValueType(typeof(int4x2), 41)]
// [assembly: VariantValueType(typeof(int4x3), 42)]
// [assembly: VariantValueType(typeof(int4x4), 43)]
// [assembly: VariantValueType(typeof(uint2), 44)]
// [assembly: VariantValueType(typeof(uint2x2), 45)]
// [assembly: VariantValueType(typeof(uint2x3), 46)]
// [assembly: VariantValueType(typeof(uint2x4), 47)]
// [assembly: VariantValueType(typeof(uint3), 48)]
// [assembly: VariantValueType(typeof(uint3x2), 49)]
// [assembly: VariantValueType(typeof(uint3x3), 50)]
// [assembly: VariantValueType(typeof(uint3x4), 51)]
// [assembly: VariantValueType(typeof(uint4), 52)]
// [assembly: VariantValueType(typeof(uint4x2), 53)]
// [assembly: VariantValueType(typeof(uint4x3), 54)]
// [assembly: VariantValueType(typeof(uint4x4), 55)]
// [assembly: VariantValueType(typeof(float2), 56)]
// [assembly: VariantValueType(typeof(float2x2), 57)]
// [assembly: VariantValueType(typeof(float2x3), 58)]
// [assembly: VariantValueType(typeof(float2x4), 59)]
// [assembly: VariantValueType(typeof(float3), 60)]
// [assembly: VariantValueType(typeof(float3x2), 61)]
// [assembly: VariantValueType(typeof(float3x3), 62)]
// [assembly: VariantValueType(typeof(float3x4), 63)]
// [assembly: VariantValueType(typeof(float4), 64)]
// [assembly: VariantValueType(typeof(float4x2), 65)]
// [assembly: VariantValueType(typeof(float4x3), 66)]
// [assembly: VariantValueType(typeof(float4x4), 67)]
// [assembly: VariantValueType(typeof(double2), 68)]
// [assembly: VariantValueType(typeof(double2x2), 69)]
// [assembly: VariantValueType(typeof(double2x3), 70)]
// [assembly: VariantValueType(typeof(double2x4), 71)]
// [assembly: VariantValueType(typeof(double3), 72)]
// [assembly: VariantValueType(typeof(double3x2), 73)]
// [assembly: VariantValueType(typeof(double3x3), 74)]
// [assembly: VariantValueType(typeof(double3x4), 75)]
// [assembly: VariantValueType(typeof(double4), 76)]
// [assembly: VariantValueType(typeof(double4x2), 77)]
// [assembly: VariantValueType(typeof(double4x3), 78)]
// [assembly: VariantValueType(typeof(double4x4), 79)]
// [assembly: VariantValueType(typeof(half), 80)]
// [assembly: VariantValueType(typeof(half2), 81)]
// [assembly: VariantValueType(typeof(half3), 82)]
// [assembly: VariantValueType(typeof(half4), 83)]
