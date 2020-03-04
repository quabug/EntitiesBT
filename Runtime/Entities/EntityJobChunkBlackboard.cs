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
        public int Index;
        
        public unsafe object this[object key]
        {
            get
            {
                switch (key)
                {
                case Type type when type.IsComponentDataType():
                    var obj = Activator.CreateInstance(type);
                    var typeInfo = TypeManager.GetTypeInfo(key.FetchTypeIndex());
                    if (typeInfo.IsZeroSized) return obj;
                    
                    var src = GetPtrRO(typeInfo.TypeIndex);
                    var dest = UnsafeUtility.PinGCObjectAndGetAddress(obj, out var handle);
                    UnsafeUtility.MemCpy(dest, src, typeInfo.SizeInChunk);
                    UnsafeUtility.ReleaseGCObject(handle);
                    return obj;
                default:
                    throw new NotImplementedException();
                }
            }
            set
            {
                switch (key)
                {
                case Type type when type.IsComponentDataType():
                    var typeInfo = TypeManager.GetTypeInfo(key.FetchTypeIndex());
                    if (typeInfo.IsZeroSized) return;
                    
                    var dest = GetPtrRW(typeInfo.TypeIndex);
                    var src = UnsafeUtility.PinGCObjectAndGetAddress(value, out var handle);
                    UnsafeUtility.MemCpy(dest, src, typeInfo.SizeInChunk);
                    UnsafeUtility.ReleaseGCObject(handle);
                    return;
                default:
                    throw new NotImplementedException();
                }
            }
        }

        public bool Has(object key)
        {
            return Chunk.GetIndexInTypeArray(key.FetchTypeIndex()) != -1;
        }

        public unsafe void* GetPtrRW(object key)
        {
            return GetPtrByTypeIndexRW(key.FetchTypeIndex());
        }
        
        public unsafe void* GetPtrRO(object key)
        {
            return GetPtrByTypeIndexRO(key.FetchTypeIndex());
        }

        private unsafe void* GetPtrByTypeIndexRW(int typeIndex)
        {
            return Chunk.GetComponentDataWithTypeRW(Index, typeIndex, GlobalSystemVersion);
        }
        
        private unsafe void* GetPtrByTypeIndexRO(int typeIndex)
        {
            // TODO: EntityComponentStore->AssertEntityHasComponent(entity, typeIndex);
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (TypeManager.IsZeroSized(typeIndex))
                throw new ArgumentException("GetComponentData can not be called with a zero sized component.");
#endif
            return Chunk.GetComponentDataWithTypeRO(Index, typeIndex);
        }
    }
}
