using System;
using System.Reflection;
using System.Runtime.InteropServices;
using EntitiesBT.Core;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using UnityEngine.Scripting;

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
                var typeIndex = key.FetchTypeIndex();
                var type = TypeManager.GetType(typeIndex);
                var ptr = GetPtrRO(key);
                return Marshal.PtrToStructure(new IntPtr(ptr), type);
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
