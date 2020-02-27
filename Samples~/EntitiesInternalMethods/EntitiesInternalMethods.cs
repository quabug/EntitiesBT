using System;
using Unity.Entities;

namespace EntitiesBT
{
     public static class EntitiesInternalMethods
     {
//          public static unsafe void* GetComponentDataWithTypeRW(this ArchetypeChunk chunk, int chunkIndex, int typeIndex, uint globalSystemVersion)
//          {
//              return ChunkDataUtility.GetComponentDataWithTypeRW(chunk.m_Chunk, chunkIndex, typeIndex, globalSystemVersion);
//          }
//          
//          public static unsafe void* GetComponentDataWithTypeRO(this ArchetypeChunk chunk, int chunkIndex, int typeIndex)
//          {
//              return ChunkDataUtility.GetComponentDataWithTypeRO(chunk.m_Chunk, chunkIndex, typeIndex);
//          }
//          
//          public static unsafe int GetIndexInTypeArray(this ArchetypeChunk chunk, int typeIndex)
//          {
//              return ChunkDataUtility.GetIndexInTypeArray(chunk.m_Chunk->Archetype, typeIndex);
//          }
//          
//          public static unsafe void* GetComponentDataRawRW(this Entity entity, EntityManager em, int typeIndex)
//          {
//              return em.GetComponentDataRawRW(entity, typeIndex);
//          }
//
//          public static unsafe int GetLength<T>(this BlobAssetReference<T> blobAssetReference) where T : struct
//          {
//              return blobAssetReference.m_data.Header->Length;
//          }
//          
//          public static unsafe ulong GetHash<T>(this BlobAssetReference<T> blobAssetReference) where T : struct
//          {
//              return blobAssetReference.m_data.Header->Hash;
//          }
     }
}
//
// // Type: EntitiesBT.UnityEntitiesInternalMethods 
// // Assembly: EntitiesBT.Runtime, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// // MVID: 62BB981B-CC83-4D20-8B58-5C513C72FE59
// // Location: /Users/quabug/proj/EntitiesBehaviorTree/EntityBehaviorTreeExample/Temp/bin/Debug/EntitiesBT.Runtime.dll
// // Sequence point data from /Users/quabug/proj/EntitiesBehaviorTree/EntityBehaviorTreeExample/Temp/bin/Debug/EntitiesBT.Runtime.pdb
//
// .class public abstract sealed auto ansi beforefieldinit
//   EntitiesBT.UnityEntitiesInternalMethods
//     extends [netstandard]System.Object
// {
//   .custom instance void [netstandard]System.Runtime.CompilerServices.ExtensionAttribute::.ctor()
//     = (01 00 00 00 )
//
//   .method public hidebysig static void*
//     GetComponentDataWithTypeRW(
//       valuetype [Unity.Entities]Unity.Entities.ArchetypeChunk chunk,
//       int32 chunkIndex,
//       int32 typeIndex,
//       unsigned int32 globalSystemVersion
//     ) cil managed
//   {
//     .custom instance void [netstandard]System.Runtime.CompilerServices.ExtensionAttribute::.ctor()
//       = (01 00 00 00 )
//     .maxstack 4
//     .locals init (
//       [0] void* V_0
//     )
//
//     // [8 9 - 8 10]
//     IL_0000: nop
//
//     // [9 13 - 9 123]
//     IL_0001: ldarg.0      // chunk
//     IL_0002: ldfld        valuetype [Unity.Entities]Unity.Entities.Chunk* [Unity.Entities]Unity.Entities.ArchetypeChunk::m_Chunk
//     IL_0007: ldarg.1      // chunkIndex
//     IL_0008: ldarg.2      // typeIndex
//     IL_0009: ldarg.3      // globalSystemVersion
//     IL_000a: call         unsigned int8* [Unity.Entities]Unity.Entities.ChunkDataUtility::GetComponentDataWithTypeRW(valuetype [Unity.Entities]Unity.Entities.Chunk*, int32, int32, unsigned int32)
//     IL_000f: stloc.0      // V_0
//     IL_0010: br.s         IL_0012
//
//     // [10 9 - 10 10]
//     IL_0012: ldloc.0      // V_0
//     IL_0013: ret
//
//   } // end of method UnityEntitiesInternalMethods::GetComponentDataWithTypeRW
//
//   .method public hidebysig static void*
//     GetComponentDataWithTypeRO(
//       valuetype [Unity.Entities]Unity.Entities.ArchetypeChunk chunk,
//       int32 chunkIndex,
//       int32 typeIndex
//     ) cil managed
//   {
//     .custom instance void [netstandard]System.Runtime.CompilerServices.ExtensionAttribute::.ctor()
//       = (01 00 00 00 )
//     .maxstack 3
//     .locals init (
//       [0] void* V_0
//     )
//
//     // [13 9 - 13 10]
//     IL_0000: nop
//
//     // [14 13 - 14 102]
//     IL_0001: ldarg.0      // chunk
//     IL_0002: ldfld        valuetype [Unity.Entities]Unity.Entities.Chunk* [Unity.Entities]Unity.Entities.ArchetypeChunk::m_Chunk
//     IL_0007: ldarg.1      // chunkIndex
//     IL_0008: ldarg.2      // typeIndex
//     IL_0009: call         unsigned int8* [Unity.Entities]Unity.Entities.ChunkDataUtility::GetComponentDataWithTypeRO(valuetype [Unity.Entities]Unity.Entities.Chunk*, int32, int32)
//     IL_000e: stloc.0      // V_0
//     IL_000f: br.s         IL_0011
//
//     // [15 9 - 15 10]
//     IL_0011: ldloc.0      // V_0
//     IL_0012: ret
//
//   } // end of method UnityEntitiesInternalMethods::GetComponentDataWithTypeRO
//
//   .method public hidebysig static int32
//     GetIndexInTypeArray(
//       valuetype [Unity.Entities]Unity.Entities.ArchetypeChunk chunk,
//       int32 typeIndex
//     ) cil managed
//   {
//     .custom instance void [netstandard]System.Runtime.CompilerServices.ExtensionAttribute::.ctor()
//       = (01 00 00 00 )
//     .maxstack 2
//     .locals init (
//       [0] int32 V_0
//     )
//
//     // [18 9 - 18 10]
//     IL_0000: nop
//
//     // [19 13 - 19 94]
//     IL_0001: ldarg.0      // chunk
//     IL_0002: ldfld        valuetype [Unity.Entities]Unity.Entities.Chunk* [Unity.Entities]Unity.Entities.ArchetypeChunk::m_Chunk
//     IL_0007: ldfld        valuetype [Unity.Entities]Unity.Entities.Archetype* [Unity.Entities]Unity.Entities.Chunk::Archetype
//     IL_000c: ldarg.1      // typeIndex
//     IL_000d: call         int32 [Unity.Entities]Unity.Entities.ChunkDataUtility::GetIndexInTypeArray(valuetype [Unity.Entities]Unity.Entities.Archetype*, int32)
//     IL_0012: stloc.0      // V_0
//     IL_0013: br.s         IL_0015
//
//     // [20 9 - 20 10]
//     IL_0015: ldloc.0      // V_0
//     IL_0016: ret
//
//   } // end of method UnityEntitiesInternalMethods::GetIndexInTypeArray
//
//   .method public hidebysig static void*
//     GetComponentDataRawRW(
//       valuetype [Unity.Entities]Unity.Entities.Entity entity,
//       class [Unity.Entities]Unity.Entities.EntityManager em,
//       int32 typeIndex
//     ) cil managed
//   {
//     .custom instance void [netstandard]System.Runtime.CompilerServices.ExtensionAttribute::.ctor()
//       = (01 00 00 00 )
//     .maxstack 3
//     .locals init (
//       [0] void* V_0
//     )
//
//     // [23 9 - 23 10]
//     IL_0000: nop
//
//     // [24 13 - 24 64]
//     IL_0001: ldarg.1      // em
//     IL_0002: ldarg.0      // entity
//     IL_0003: ldarg.2      // typeIndex
//     IL_0004: callvirt     instance void* [Unity.Entities]Unity.Entities.EntityManager::GetComponentDataRawRW(valuetype [Unity.Entities]Unity.Entities.Entity, int32)
//     IL_0009: stloc.0      // V_0
//     IL_000a: br.s         IL_000c
//
//     // [25 9 - 25 10]
//     IL_000c: ldloc.0      // V_0
//     IL_000d: ret
//
//   } // end of method UnityEntitiesInternalMethods::GetComponentDataRawRW
// } // end of class EntitiesBT.UnityEntitiesInternalMethods
