using System;
using System.Reflection;
using EntitiesBT.Core;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using UnityEngine.Scripting;
using UnityEngine.UIElements;

namespace EntitiesBT.Entities
{
    public struct EntityJobChunkBlackboard : IBlackboard
    {
        public uint GlobalSystemVersion;
        public ArchetypeChunk Chunk;
        public int Index;

        private static readonly GetDataDelegate _GET_COMPONENT_DATA;
        private static readonly SetDataDelegate _SET_COMPONENT_DATA;
        private static readonly HasDataDelegate _HAS_COMPONENT_DATA;

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

            {
                var predicate = typeof(EntityJobChunkBlackboard).GetMethod("HasComponentData", BindingFlags.Public | BindingFlags.Instance);
                _HAS_COMPONENT_DATA = (caller, type) => (bool)predicate.MakeGenericMethod(type).Invoke(caller, new object[0]);
            }
        }
        
        public object this[object key]
        {
            get
            {
                var type = ValidateKey(key);
                return _GET_COMPONENT_DATA(this, type);
            }
            set
            {
                var type = ValidateKey(key);
                _SET_COMPONENT_DATA(this, type, value);
            }
        }

        public bool Has(object key)
        {
            var type = ValidateKey(key);
            return _HAS_COMPONENT_DATA(this, type);
        }

        public unsafe void* GetPtrRW(object key)
        {
            switch (key)
            {
            case Type type when type.IsComponentDataType():
                return GetPtrRW(TypeManager.GetTypeIndex(type));
            case ulong componentStableHash:
                return GetPtrRW(TypeManager.GetTypeIndexFromStableTypeHash(componentStableHash));
            default:
                 throw new NotImplementedException();
            }
        }
        
        public unsafe void* GetPtrRO(object key)
        {
            switch (key)
            {
            case Type type when type.IsComponentDataType():
                return GetPtrRO(TypeManager.GetTypeIndex(type));
            case ulong componentStableHash:
                return GetPtrRO(TypeManager.GetTypeIndexFromStableTypeHash(componentStableHash));
            default:
                 throw new NotImplementedException();
            }
        }

        private unsafe void* GetPtrRW(int typeIndex)
        {
            // TODO: EntityComponentStore->AssertEntityHasComponent(entity, typeIndex);
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (TypeManager.IsZeroSized(typeIndex))
                throw new ArgumentException("GetComponentData can not be called with a zero sized component.");
#endif
            return Chunk.GetComponentDataWithTypeRW(Index, typeIndex, GlobalSystemVersion);
        }
        
        private unsafe void* GetPtrRO(int typeIndex)
        {
            // TODO: EntityComponentStore->AssertEntityHasComponent(entity, typeIndex);
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (TypeManager.IsZeroSized(typeIndex))
                throw new ArgumentException("GetComponentData can not be called with a zero sized component.");
#endif
            return Chunk.GetComponentDataWithTypeRO(Index, typeIndex);
        }

        Type ValidateKey(object key)
        {
            if (!(key is Type type) || !type.IsComponentDataType()) throw new NotImplementedException();
            return type;
        }

        [Preserve]
        public unsafe T GetComponentData<T>() where T : struct, IComponentData
        {
            var ptr = GetPtrRO(TypeManager.GetTypeIndex<T>());
            return UnsafeUtilityEx.AsRef<T>(ptr);
        }

        [Preserve]
        public unsafe void SetComponentData<T>(T value) where T : struct, IComponentData
        {
            var ptr = GetPtrRW(TypeManager.GetTypeIndex<T>());
            UnsafeUtilityEx.AsRef<T>(ptr) = value;
        }
        
        [Preserve]
        public bool HasComponentData<T>() where T : struct, IComponentData
        {
            var typeIndex = TypeManager.GetTypeIndex<T>();
            return Chunk.GetIndexInTypeArray(typeIndex) != -1;
        }
    }
}
