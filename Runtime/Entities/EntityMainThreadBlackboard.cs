using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EntitiesBT.Core;
using Unity.Entities;
using UnityEngine.Scripting;

namespace EntitiesBT.Entities
{
    public class EntityMainThreadBlackboard : IBlackboard
    {
        private readonly EntityManager _em;
        private readonly Entity _entity;
        
        private Func<Type, object> _getComponentData;
        private Func<Type, object> _getManagedData;
        private Func<Type, object> _getComponentObject;
        private Action<Type, object> _setComponentData;
        private Predicate<Type> _hasComponent;
        
        public EntityMainThreadBlackboard(EntityManager em, Entity entity)
        {
            _em = em;
            _entity = entity;
            
            {
                var getter = GetType().GetMethod("GetComponentData", BindingFlags.NonPublic | BindingFlags.Instance);
                _getComponentData = type => getter.MakeGenericMethod(type).Invoke(this, new object[0]);
            }

            {
                var getter = GetType().GetMethod("GetManagedData", BindingFlags.NonPublic | BindingFlags.Instance);
                _getManagedData = type => getter.MakeGenericMethod(type).Invoke(this, new object[0]);
            }

            {
                var getter = GetType().GetMethod("GetUnityComponent", BindingFlags.NonPublic | BindingFlags.Instance);
                _getComponentObject = type => getter.MakeGenericMethod(type).Invoke(this, new object[0]);
            }
            
            {
                var setter = GetType().GetMethod("SetComponentData", BindingFlags.NonPublic | BindingFlags.Instance);
                _setComponentData = (type, value) => setter.MakeGenericMethod(type).Invoke(this, new [] { value });
            }
            
            {
                var predicate = GetType().GetMethod("HasComponent", BindingFlags.NonPublic | BindingFlags.Instance);
                _hasComponent = type => (bool)predicate.MakeGenericMethod(type).Invoke(this, new object[0]);
            }
        }

        public object this[object key]
        {
            get
            {
                var type = key as Type;
                if (type.IsUnityComponentType()) return _getComponentObject(type);
                if (type.IsComponentDataType()) return _getComponentData(type);
                if (type.IsManagedDataType()) return _getManagedData(type);
                throw new NotImplementedException();
            }
            set
            {
                var type = key as Type;
                if (type.IsComponentDataType())
                {
                    _setComponentData(type, value);
                    return;
                }
                
                if (type.IsManagedDataType()) throw new Exception($"Managed data {type.Name} is not writable");
                if (type.IsUnityComponentType()) throw new Exception($"Component {type.Name} is not writable");
                throw new NotImplementedException();
            }
        }

        public bool Has(object key)
        {
            var type = key as Type;
            if (type.IsUnityComponentType() || type.IsComponentDataType() || type.IsManagedDataType())
                return _hasComponent(type);
            return false;
        }

        [Preserve]
        public T GetComponentData<T>() where T : struct, IComponentData
        {
            return _em.GetComponentData<T>(_entity);
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
