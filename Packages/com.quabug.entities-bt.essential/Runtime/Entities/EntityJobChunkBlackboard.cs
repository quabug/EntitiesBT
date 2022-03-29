using System;
using EntitiesBT.Core;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;

namespace EntitiesBT.Entities
{
    public struct EntityJobChunkBlackboard : IBlackboard
    {
        public uint GlobalSystemVersion;
        public ArchetypeChunk Chunk;
        public int EntityIndex;
        public EntityCommandJob EntityCommandJob;
        
        private unsafe void* GetPtrByTypeIndexRW(int typeIndex)
        {
            return Chunk.GetComponentDataWithTypeRW(EntityIndex, typeIndex, GlobalSystemVersion);
        }
        
        private unsafe void* GetPtrByTypeIndexRO(int typeIndex)
        {
            return Chunk.GetComponentDataWithTypeRO(EntityIndex, typeIndex);
        }

        public bool HasData<T>() where T : struct
        {
            var index = TypeManager.GetTypeIndex<T>();
            return Chunk.GetIndexInTypeArray(index) != -1;
        }

        public unsafe T GetData<T>() where T : struct
        {
            var index = TypeManager.GetTypeIndex<T>();
            var ptr = GetPtrByTypeIndexRO(index);
            return UnsafeUtility.AsRef<T>(ptr);
        }

        public unsafe ref T GetDataRef<T>() where T : struct
        {
            var index = TypeManager.GetTypeIndex<T>();
            var ptr = GetPtrByTypeIndexRW(index);
            return ref UnsafeUtility.AsRef<T>(ptr);
        }

        public bool HasData(Type type)
        {
            var index = TypeManager.GetTypeIndex(type);
            return Chunk.GetIndexInTypeArray(index) != -1;
        }

        public unsafe IntPtr GetDataPtrRO(Type type)
        {
            return new IntPtr(GetPtrByTypeIndexRO(TypeManager.GetTypeIndex(type)));
        }

        public unsafe IntPtr GetDataPtrRW(Type type)
        {
            return new IntPtr(GetPtrByTypeIndexRW(TypeManager.GetTypeIndex(type)));
        }

        public T GetObject<T>() where T : class
        {
            if (typeof(T) == typeof(IEntityCommand))
                return EntityCommandJob as T;
            throw new NotImplementedException();
        }
    }
}
