using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Entities;
using EntitiesBT.Core;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using UnityEngine.Scripting;

namespace EntitiesBT.Entities
{
    public class EntityBlackboard : IBlackboard
    {
        public EntityManager EntityManager;
        public Entity Entity;
        
        private static readonly GetDataDelegate _getComponentData;
        private static readonly GetDataDelegate _getSharedComponentData;
        private static readonly GetDataDelegate _getManagedData;
        private static readonly GetDataDelegate _getComponentObject;
        private static readonly SetDataDelegate _setComponentData;
        private static readonly HasDataDelegate _hasComponent;

        static EntityBlackboard()
        {
            {
                var getter = typeof(EntityBlackboard).GetMethod("GetComponentData", BindingFlags.Public | BindingFlags.Instance);
                _getComponentData = (caller, type) => getter.MakeGenericMethod(type).Invoke(caller, new object[0]);
            }
            
            {
                var getter = typeof(EntityBlackboard).GetMethod("GetSharedComponentData", BindingFlags.Public | BindingFlags.Instance);
                _getSharedComponentData = (caller, type) => getter.MakeGenericMethod(type).Invoke(caller, new object[0]);
            }

            {
                var getter = typeof(EntityBlackboard).GetMethod("GetManagedData", BindingFlags.Public | BindingFlags.Instance);
                _getManagedData = (caller, type) => getter.MakeGenericMethod(type).Invoke(caller, new object[0]);
            }

            {
                var getter = typeof(EntityBlackboard).GetMethod("GetUnityComponent", BindingFlags.Public | BindingFlags.Instance);
                _getComponentObject = (caller, type) => getter.MakeGenericMethod(type).Invoke(caller, new object[0]);
            }
            
            {
                var setter = typeof(EntityBlackboard).GetMethod("SetComponentData", BindingFlags.Public | BindingFlags.Instance);
                _setComponentData = (caller, type, value) => setter.MakeGenericMethod(type).Invoke(caller, new [] { value });
            }
            
            {
                var predicate = typeof(EntityBlackboard).GetMethod("HasComponent", BindingFlags.Public | BindingFlags.Instance);
                _hasComponent = (caller, type) => (bool)predicate.MakeGenericMethod(type).Invoke(caller, new object[0]);
            }
        }

        public object this[object key]
        {
            get
            {
                var type = key as Type;
                if (type.IsComponentDataType()) return _getComponentData(this, type);
                if (type.IsSharedComponentDataType()) return _getSharedComponentData(this, type);
                if (type.IsManagedDataType()) return _getManagedData(this, type);
                if (type.IsUnityComponentType()) return _getComponentObject(this, type);
                throw new NotImplementedException();
            }
            set
            {
                var type = key as Type;
                if (type.IsComponentDataType())
                {
                    _setComponentData(this, type, value);
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
                return _hasComponent(this, type);
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
