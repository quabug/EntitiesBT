using System;
using System.Runtime.InteropServices;
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
        
        public unsafe object this[object key]
        {
            get
            {
                {
                    switch (key)
                    {
                    case Type type when type == typeof(IEntityCommand):
                        return EntityCommandJob;
                    case Type type when type == typeof(BehaviorTreeBufferElement):
                        return UnsafeUtilityEx.AsRef<BehaviorTreeBufferElement>(BehaviorTreeElementPtr);
                    }
                }

                {
                    var typeIndex = key.FetchTypeIndex();
                    var type = TypeManager.GetType(typeIndex);
                    var ptr = GetPtrRO(key);
                    return Marshal.PtrToStructure(new IntPtr(ptr), type);
                }
            }
            set
            {
                var typeIndex = key.FetchTypeIndex();
                var dest = GetPtrByTypeIndexRW(typeIndex);
                Marshal.StructureToPtr(value, new IntPtr(dest), false);
            }
        }

        public bool Has(object key)
        {
            return Chunk.GetIndexInTypeArray(key.FetchTypeIndex()) != -1;
        }

        public unsafe void* GetPtrRW(object key)
        {
            if (key is Type type && type == typeof(BehaviorTreeBufferElement)) return BehaviorTreeElementPtr;
            return GetPtrByTypeIndexRW(key.FetchTypeIndex());
        }
        
        public unsafe void* GetPtrRO(object key)
        {
            return GetPtrByTypeIndexRO(key.FetchTypeIndex());
        }

        private unsafe void* GetPtrByTypeIndexRW(int typeIndex)
        {
            return Chunk.GetComponentDataWithTypeRW(EntityIndex, typeIndex, GlobalSystemVersion);
        }
        
        private unsafe void* GetPtrByTypeIndexRO(int typeIndex)
        {
            return Chunk.GetComponentDataWithTypeRO(EntityIndex, typeIndex);
        }
    }
}
