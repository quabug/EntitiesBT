using System;
using System.Collections.Generic;
using System.Reflection;
using EntitiesBT.Core;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Entities
{
    public struct EntityJobChunkBlackboard : IBlackboard
    {
        // public NativeHashMap<int, ArchetypeChunkComponentTypeDynamic> Types;
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
        public T GetComponentData<T>() where T : struct, IComponentData
        {
            return default;
            // var type = Types[TypeManager.GetTypeIndex<T>()];
            // return Chunk.GetDynamicComponentDataArrayReinterpret<T>(type, UnsafeUtility.SizeOf<T>())[EntityIndex];
        }

        [Preserve]
        public void SetComponentData<T>(T value) where T : struct, IComponentData
        {
            // var type = Types[TypeManager.GetTypeIndex<T>()];
            // var array = Chunk.GetDynamicComponentDataArrayReinterpret<T>(type, UnsafeUtility.SizeOf<T>());
            // array[EntityIndex] = value;
        }
        
        [Preserve]
        public bool HasComponentData<T>() where T : struct, IComponentData
        {
            return false;
            // var type = Types[TypeManager.GetTypeIndex<T>()];
            // return Chunk.Has(type);
        }
    }
}
