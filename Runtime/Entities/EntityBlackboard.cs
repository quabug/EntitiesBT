using System;
using System.Reflection;
using EntitiesBT.Core;
using Unity.Entities;
using UnityEngine.Scripting;

namespace EntitiesBT.Entities
{
    public class EntityBlackboard : IBlackboard
    {
        private readonly EntityManager _em;
        private readonly Entity _entity;
        
        private static readonly Func<object, Type, object> _getComponentData;
        private static readonly Func<object, Type, object> _getManagedData;
        private static readonly Func<object, Type, object> _getComponentObject;
        private static readonly Action<object, Type, object> _setComponentData;
        private static readonly Func<object, Type, bool> _hasComponent;

        static EntityBlackboard()
        {
            {
                var getter = typeof(EntityBlackboard).GetMethod("GetComponentData", BindingFlags.NonPublic | BindingFlags.Instance);
                _getComponentData = (caller, type) => getter.MakeGenericMethod(type).Invoke(caller, new object[0]);
            }

            {
                var getter = typeof(EntityBlackboard).GetMethod("GetManagedData", BindingFlags.NonPublic | BindingFlags.Instance);
                _getManagedData = (caller, type) => getter.MakeGenericMethod(type).Invoke(caller, new object[0]);
            }

            {
                var getter = typeof(EntityBlackboard).GetMethod("GetUnityComponent", BindingFlags.NonPublic | BindingFlags.Instance);
                _getComponentObject = (caller, type) => getter.MakeGenericMethod(type).Invoke(caller, new object[0]);
            }
            
            {
                var setter = typeof(EntityBlackboard).GetMethod("SetComponentData", BindingFlags.NonPublic | BindingFlags.Instance);
                _setComponentData = (caller, type, value) => setter.MakeGenericMethod(type).Invoke(caller, new [] { value });
            }
            
            {
                var predicate = typeof(EntityBlackboard).GetMethod("HasComponent", BindingFlags.NonPublic | BindingFlags.Instance);
                _hasComponent = (caller, type) => (bool)predicate.MakeGenericMethod(type).Invoke(caller, new object[0]);
            }
        }
        
        public EntityBlackboard(EntityManager em, Entity entity)
        {
            _em = em;
            _entity = entity;
        }

        public object this[object key]
        {
            get
            {
                var type = key as Type;
                if (type.IsUnityComponentType()) return _getComponentObject(this, type);
                if (type.IsComponentDataType()) return _getComponentData(this, type);
                if (type.IsManagedDataType()) return _getManagedData(this, type);
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
