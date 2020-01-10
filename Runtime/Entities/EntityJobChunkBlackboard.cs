using System;
using System.Reflection;
using EntitiesBT;
using EntitiesBT.Core;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Entities
{
    public struct EntityJobChunkBlackboard : IBlackboard
    {
        public uint GlobalSystemVersion;
        public ArchetypeChunk Chunk;
        public int Index;
        // public Entity Entity;
        // public unsafe EntityCommandBuffer.Concurrent* ECB;

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

        public unsafe ref T GetRef<T>(object key) where T : struct
        {
            var type = typeof(T);
            if (!type.IsComponentDataType()) throw new NotImplementedException();
            
            var typeIndex = TypeManager.GetTypeIndex<T>();
            if (TypeManager.IsZeroSized(typeIndex))
                throw new ArgumentException($"SetComponentData<{type}> can not be called with a zero sized component.");
            return ref UnsafeUtilityEx.AsRef<T>(Chunk.GetComponentDataWithTypeRW(Index, typeIndex, GlobalSystemVersion));
        }

        Type ValidateKey(object key)
        {
            if (!(key is Type type) || !type.IsComponentDataType()) throw new NotImplementedException();
            return type;
        }

        [Preserve]
        public unsafe T GetComponentData<T>() where T : struct, IComponentData
        {
            var typeIndex = TypeManager.GetTypeIndex<T>();
            if (TypeManager.IsZeroSized(typeIndex))
                throw new ArgumentException($"SetComponentData<{typeof(T)}> can not be called with a zero sized component.");
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
        
        [Preserve]
        public bool HasComponentData<T>() where T : struct, IComponentData
        {
            var typeIndex = TypeManager.GetTypeIndex<T>();
            return Chunk.GetIndexInTypeArray(typeIndex) != -1;
        }
    }
}
