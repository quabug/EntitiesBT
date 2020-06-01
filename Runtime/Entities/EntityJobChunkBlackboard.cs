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
        [NativeDisableUnsafePtrRestriction] public unsafe void* BehaviorTreeElementPtr;
        
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
            if (typeof(T) == typeof(BehaviorTreeBufferElement))
                return UnsafeUtilityEx.AsRef<T>(BehaviorTreeElementPtr);
            var index = TypeManager.GetTypeIndex<T>();
            var ptr = GetPtrByTypeIndexRO(index);
            return UnsafeUtilityEx.AsRef<T>(ptr);
        }

        public unsafe ref T GetDataRef<T>() where T : struct
        {
            if (typeof(T) == typeof(BehaviorTreeBufferElement))
                return ref UnsafeUtilityEx.AsRef<T>(BehaviorTreeElementPtr);
            var index = TypeManager.GetTypeIndex<T>();
            var ptr = GetPtrByTypeIndexRW(index);
            return ref UnsafeUtilityEx.AsRef<T>(ptr);
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
