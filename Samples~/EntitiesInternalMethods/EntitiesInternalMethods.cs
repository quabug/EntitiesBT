using System;
using System.Diagnostics;
using Unity.Entities;

namespace EntitiesBT
{
    public static class EntitiesInternalMethods
    {
        // public static unsafe void* GetComponentDataWithTypeRW(this ArchetypeChunk chunk, int chunkIndex, int typeIndex, uint globalSystemVersion)
        // {
        //     chunk.AssertEntityHasComponent(typeIndex);
        //     return ChunkDataUtility.GetComponentDataWithTypeRW(chunk.m_Chunk, chunkIndex, typeIndex, globalSystemVersion);
        // }
        //
        // public static unsafe void* GetComponentDataWithTypeRO(this ArchetypeChunk chunk, int chunkIndex, int typeIndex)
        // {
        //     chunk.AssertEntityHasComponent(typeIndex);
        //     return ChunkDataUtility.GetComponentDataWithTypeRO(chunk.m_Chunk, chunkIndex, typeIndex);
        // }
        //
        // [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        // public static void AssertEntityHasComponent(this ArchetypeChunk chunk, int typeIndex)
        // {
        //     if (chunk.GetIndexInTypeArray(typeIndex) == -1)
        //         throw new ArgumentException($"A component with typeIndex:{typeIndex} has not been added to the entity.");
        //     if (TypeManager.IsZeroSized(typeIndex))
        //         throw new ArgumentException("GetComponentData can not be called with a zero sized component.");
        // }
        //
        // public static unsafe int GetIndexInTypeArray(this ArchetypeChunk chunk, int typeIndex)
        // {
        //     return ChunkDataUtility.GetIndexInTypeArray(chunk.m_Chunk->Archetype, typeIndex);
        // }
        //
        // public static unsafe void* GetComponentDataRawRW(this Entity entity, EntityManager em, int typeIndex)
        // {
        //     return em.GetComponentDataRawRW(entity, typeIndex);
        // }
        //
        // public static unsafe void* GetComponentDataRawRO(this Entity entity, EntityManager em, int typeIndex)
        // {
        //     return em.GetComponentDataRawRO(entity, typeIndex);
        // }
        //
        // public static unsafe int GetLength<T>(this BlobAssetReference<T> blobAssetReference) where T : struct
        // {
        //     blobAssetReference.m_data.ValidateNotNull();
        //     return blobAssetReference.m_data.Header->Length;
        // }
        //
        // public static unsafe ulong GetHash<T>(this BlobAssetReference<T> blobAssetReference) where T : struct
        // {
        //     blobAssetReference.m_data.ValidateNotNull();
        //     return blobAssetReference.m_data.Header->Hash;
        // }
        // //
        // // public static unsafe Allocator GetAllocator<T>(this BlobAssetReference<T> blobAssetReference) where T : struct
        // // {
        // //     blobAssetReference.m_data.ValidateNotNull();
        // //     return blobAssetReference.m_data.Header->Allocator;
        // // }
    }
}
