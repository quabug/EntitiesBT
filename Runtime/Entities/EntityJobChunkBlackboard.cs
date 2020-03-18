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
        

        private static readonly GetDataDelegate _GET_COMPONENT_DATA;
        private static readonly SetDataDelegate _SET_COMPONENT_DATA;

        static EntityJobChunkBlackboard()
        {
            {
                var getter = typeof(EntityJobChunkBlackboard).GetMethod("GetComponentData", BindingFlags.Public | BindingFlags.Instance);
                _GET_COMPONENT_DATA = (caller, type) => getter.MakeGenericMethod(type).Invoke(caller, new object[0]);
            }

            {
                var setter = typeof(EntityJobChunkBlackboard).GetMethod("SetComponentData", BindingFlags.Public | BindingFlags.Instance);
                _SET_COMPONENT_DATA = (caller, type, value) => setter.MakeGenericMethod(type).Invoke(caller, new[] { value });
            }
        }
        
        public object this[object key]
        {
            get
            {
                switch (key)
                {
                case Type type when type.IsComponentDataType():
                    return _GET_COMPONENT_DATA(this, type);
                default:
                    throw new NotImplementedException();
                }
            }
            set
            {
                switch (key)
                {
                case Type type when type.IsComponentDataType():
                    _SET_COMPONENT_DATA(this, type, value);
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

        [Preserve]
        public unsafe T GetComponentData<T>() where T : struct, IComponentData
        {
            var typeIndex = TypeManager.GetTypeIndex<T>();
            if (TypeManager.IsZeroSized(typeIndex))
                throw new ArgumentException($"GetComponentData<{typeof(T)}> can not be called with a zero sized component.");
            return UnsafeUtilityEx.AsRef<T>(Chunk.GetComponentDataWithTypeRO(Index, typeIndex));
        }
        
        [Preserve]
        public unsafe void SetComponentData<T>(T value) where T : struct, IComponentData
        {
            var typeIndex = TypeManager.GetTypeIndex<T>();
            if (TypeManager.IsZeroSized(typeIndex))
                throw new ArgumentException($"SetComponentData<{typeof(T)}> can not be called with a zero sized component.");
            UnsafeUtilityEx.AsRef<T>(Chunk.GetComponentDataWithTypeRW(Index, typeIndex, GlobalSystemVersion)) = value;
        }
    }
}
