using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EntitiesBT.Core;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using UnityEngine.Scripting;

namespace EntitiesBT.Entities
{
    public struct EntityBlackboard : IBlackboard
    {
        private readonly EntityManager _em;
        private readonly Entity _entity;
        
        private static Func<object, Type, object> _getComponentData;
        private static Func<object, Type, object> _getSharedComponentData;
        private static Func<object, Type, object> _getManagedData;
        private static Func<object, Type, object> _getComponentObject;
        private static Action<object, Type, object> _setComponentData;
        private static Func<object, Type, bool> _hasComponent;
        
        public EntityBlackboard(EntityManager em, Entity entity)
        {
            _em = em;
            _entity = entity;
            
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
            
            return ref UnsafeUtilityEx.AsRef<T>(_entity.GetComponentDataRawRW(_em, typeIndex));
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
            return _em.GetComponentData<T>(_entity);
        }
        
        [Preserve]
        public T GetSharedComponentData<T>() where T : struct, ISharedComponentData
        {
            return _em.GetSharedComponentData<T>(_entity);
        }
        
        [Preserve]
        public T GetManagedData<T>() where T : class, IComponentData
        {
            return _em.GetComponentData<T>(_entity);
        }
        
        [Preserve]
        public T GetUnityComponent<T>()
        {
            return _em.GetComponentObject<T>(_entity);
        }
        
        [Preserve]
        public void SetComponentData<T>(T value) where T : struct, IComponentData
        {
            _em.SetComponentData(_entity, value);
        }
        
        [Preserve]
        public bool HasComponent<T>() where T : struct, IComponentData
        {
            return _em.HasComponent<T>(_entity);
        }

    }
}
