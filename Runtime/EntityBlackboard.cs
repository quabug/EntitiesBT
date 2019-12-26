using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesBT.Core;
using Unity.Entities;

namespace EntitiesBT
{
    public class EntityBlackboard : IBlackboard
    {
        private readonly Dictionary<object, object> _dictionary = new Dictionary<object, object>();
        
        private Func<Type, object> _getComponentData;
        private Func<Type, object> _getManagedData;
        private Func<Type, object> _getComponentObject;
        private Action<Type, object> _setComponentData;
        
        public EntityBlackboard(EntityManager em, Entity entity)
        {
            {
                var getComponentData = em.GetType().GetMethod("GetComponentData");
                _getComponentData = type => getComponentData.MakeGenericMethod(type).Invoke(em, new object[] {entity});
            }
            
            {
                var setComponentData = em.GetType().GetMethod("SetComponentData");
                _setComponentData = (type, value) => setComponentData.MakeGenericMethod(type).Invoke(em, new[] {entity, value});
            }

            {
                var getManagedData = typeof(EntityManagerManagedComponentExtensions).GetMethod("GetComponentData");
                _getManagedData = type => getManagedData.MakeGenericMethod(type).Invoke(null, new object[] {em, entity});
            }
            
            {
                var getComponentObject = em.GetType()
                    .GetMethods()
                    .First(m => m.Name == "GetComponentObject" && m.GetParameters().Length == 1)
                ;
                _getComponentObject = type => getComponentObject.MakeGenericMethod(type).Invoke(em, new object[] {entity});
            }
        }

        public object Get(object key)
        {
            var type = key as Type;
            if (IsUnityComponentType(type)) return _getComponentObject(type);
            if (IsComponentDataType(type)) return _getComponentData(type);
            if (IsManagedDataType(type)) return _getManagedData(type);
            _dictionary.TryGetValue(key, out var value);
            return value;
        }

        public void Set(object value, object key)
        {
            var type = key as Type;
            if (IsComponentDataType(type))
            {
                _setComponentData(type, value);
                return;
            }
            _dictionary[key] = value;
        }

        public object this[object key]
        {
            get => Get(key);
            set => Set(value, key);
        }

        bool IsComponentDataType(Type type) =>
            type != null && type.IsValueType && typeof(IComponentData).IsAssignableFrom(type);
        
        bool IsManagedDataType(Type type) =>
            type != null && type.IsClass && typeof(IComponentData).IsAssignableFrom(type);

        bool IsUnityComponentType(Type type) =>
            type != null && type.IsSubclassOf(typeof(UnityEngine.Component));
    }
}
