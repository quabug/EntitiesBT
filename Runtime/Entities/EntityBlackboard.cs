using System;
using System.Reflection;
using EntitiesBT.Core;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using UnityEngine.Scripting;

namespace EntitiesBT.Entities
{
    public class EntityBlackboard : IBlackboard
    {
        public EntityManager EntityManager;
        public Entity Entity;
        
        private static readonly GetDataDelegate _GET_COMPONENT_DATA;
        private static readonly GetDataDelegate _GET_SHARED_COMPONENT_DATA;
        private static readonly GetDataDelegate _GET_MANAGED_DATA;
        private static readonly GetDataDelegate _GET_COMPONENT_OBJECT;
        private static readonly SetDataDelegate _SET_COMPONENT_DATA;
        private static readonly HasDataDelegate _HAS_COMPONENT;

        static EntityBlackboard()
        {
            {
                var getter = typeof(EntityBlackboard).GetMethod("GetComponentData", BindingFlags.Public | BindingFlags.Instance);
                _GET_COMPONENT_DATA = (caller, type) => getter.MakeGenericMethod(type).Invoke(caller, new object[0]);
            }
            
            {
                var getter = typeof(EntityBlackboard).GetMethod("GetSharedComponentData", BindingFlags.Public | BindingFlags.Instance);
                _GET_SHARED_COMPONENT_DATA = (caller, type) => getter.MakeGenericMethod(type).Invoke(caller, new object[0]);
            }

            {
                var getter = typeof(EntityBlackboard).GetMethod("GetManagedData", BindingFlags.Public | BindingFlags.Instance);
                _GET_MANAGED_DATA = (caller, type) => getter.MakeGenericMethod(type).Invoke(caller, new object[0]);
            }

            {
                var getter = typeof(EntityBlackboard).GetMethod("GetUnityComponent", BindingFlags.Public | BindingFlags.Instance);
                _GET_COMPONENT_OBJECT = (caller, type) => getter.MakeGenericMethod(type).Invoke(caller, new object[0]);
            }
            
            {
                var setter = typeof(EntityBlackboard).GetMethod("SetComponentData", BindingFlags.Public | BindingFlags.Instance);
                _SET_COMPONENT_DATA = (caller, type, value) => setter.MakeGenericMethod(type).Invoke(caller, new [] { value });
            }
            
            {
                var predicate = typeof(EntityBlackboard).GetMethod("HasComponent", BindingFlags.Public | BindingFlags.Instance);
                _HAS_COMPONENT = (caller, type) => (bool)predicate.MakeGenericMethod(type).Invoke(caller, new object[0]);
            }
        }

        public object this[object key]
        {
            get
            {
                var type = key as Type;
                if (type.IsComponentDataType()) return _GET_COMPONENT_DATA(this, type);
                if (type.IsSharedComponentDataType()) return _GET_SHARED_COMPONENT_DATA(this, type);
                if (type.IsManagedDataType()) return _GET_MANAGED_DATA(this, type);
                if (type.IsUnityComponentType()) return _GET_COMPONENT_OBJECT(this, type);
                throw new NotImplementedException();
            }
            set
            {
                var type = key as Type;
                if (type.IsComponentDataType())
                {
                    _SET_COMPONENT_DATA(this, type, value);
                    return;
                }
                
                if (type.IsManagedDataType()) throw new Exception($"Managed data {type.Name} is not writable");
                if (type.IsUnityComponentType()) throw new Exception($"Component {type.Name} is not writable");
                throw new NotImplementedException();
            }
        }
        
        public unsafe ref T GetRef<T>(object key) where T : struct
        {
            var type = typeof(T);
            if (!type.IsComponentDataType()) throw new NotImplementedException();
            
            var typeIndex = TypeManager.GetTypeIndex<T>();
            
            if (ComponentType.FromTypeIndex(typeIndex).IsZeroSized)
                throw new ArgumentException($"SetComponentData<{type}> can not be called with a zero sized component.");
            
            return ref UnsafeUtilityEx.AsRef<T>(Entity.GetComponentDataRawRW(EntityManager, typeIndex));
        }

        public bool Has(object key)
        {
            var type = key as Type;
            if (type.IsUnityComponentType() || type.IsComponentDataType() || type.IsManagedDataType())
                return _HAS_COMPONENT(this, type);
            return false;
        }

        [Preserve]
        public T GetComponentData<T>() where T : struct, IComponentData
        {
            return EntityManager.GetComponentData<T>(Entity);
        }
        
        [Preserve]
        public T GetSharedComponentData<T>() where T : struct, ISharedComponentData
        {
            return EntityManager.GetSharedComponentData<T>(Entity);
        }
        
        [Preserve]
        public T GetManagedData<T>() where T : class, IComponentData
        {
            return EntityManager.GetComponentData<T>(Entity);
        }
        
        [Preserve]
        public T GetUnityComponent<T>()
        {
            return EntityManager.GetComponentObject<T>(Entity);
        }
        
        [Preserve]
        public void SetComponentData<T>(T value) where T : struct, IComponentData
        {
            EntityManager.SetComponentData(Entity, value);
        }
        
        [Preserve]
        public bool HasComponent<T>() where T : struct, IComponentData
        {
            return EntityManager.HasComponent<T>(Entity);
        }

    }
}
