using System;
using System.Reflection;
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
        public int EntityIndex;
        
        private static Func<object, Type, object> _getComponentData;
        private static Action<object, Type, object> _setComponentData;
        private static Func<object, Type, bool> _hasComponentData;

        static EntityJobChunkBlackboard()
        {
            {
                var getter = typeof(EntityJobChunkBlackboard).GetMethod("GetComponentData", BindingFlags.Public | BindingFlags.Instance);
                _getComponentData = (caller, type) => getter.MakeGenericMethod(type).Invoke(caller, new object[0]);
            }

            {
                var setter = typeof(EntityJobChunkBlackboard).GetMethod("SetComponentData", BindingFlags.Public | BindingFlags.Instance);
                _setComponentData = (caller, type, value) => setter.MakeGenericMethod(type).Invoke(caller, new[] { value });
            }

            {
                var predicate = typeof(EntityJobChunkBlackboard).GetMethod("HasComponentData", BindingFlags.Public | BindingFlags.Instance);
                _hasComponentData = (caller, type) => (bool)predicate.MakeGenericMethod(type).Invoke(caller, new object[0]);
            }
        }
        
        public object this[object key]
        {
            get
            {
                var type = ValidateKey(key);
                return _getComponentData(this, type);
            }
            set
            {
                var type = ValidateKey(key);
                _setComponentData(this, type, value);
            }
        }

        public unsafe ref T GetRef<T>(object key) where T : struct
        {
            var type = typeof(T);
            if (!type.IsComponentDataType()) throw new NotImplementedException();
            
            var typeIndex = TypeManager.GetTypeIndex<T>();
            if (TypeManager.IsZeroSized(typeIndex))
                throw new ArgumentException($"SetComponentData<{type}> can not be called with a zero sized component.");
            var ptr = ChunkDataUtility.GetComponentDataWithTypeRW(Chunk.m_Chunk, EntityIndex, typeIndex, GlobalSystemVersion);
            return ref UnsafeUtilityEx.AsRef<T>(ptr);
        }

        public bool Has(object key)
        {
            var type = ValidateKey(key);
            return _hasComponentData(this, type);
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
            var ptr = ChunkDataUtility.GetComponentDataWithTypeRO(Chunk.m_Chunk, EntityIndex, typeIndex);
            UnsafeUtility.CopyPtrToStructure(ptr, out T value);
            return value;
        }

        [Preserve]
        public unsafe void SetComponentData<T>(T value) where T : struct, IComponentData
        {
            var typeIndex = TypeManager.GetTypeIndex<T>();
            if (TypeManager.IsZeroSized(typeIndex))
                throw new ArgumentException($"SetComponentData<{typeof(T)}> can not be called with a zero sized component.");
            var ptr = ChunkDataUtility.GetComponentDataWithTypeRW(Chunk.m_Chunk, EntityIndex, typeIndex, GlobalSystemVersion);
            UnsafeUtilityEx.AsRef<T>(ptr) = value;
        }
        
        [Preserve]
        public bool HasComponentData<T>() where T : struct, IComponentData
        {
            throw new NotImplementedException();
            // var type = Types[TypeManager.GetTypeIndex<T>()];
            // return Chunk.Has(type);
        }
    }
}
