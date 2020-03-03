using System;
using System.Reflection;
using EntitiesBT.Core;
using Unity.Entities;
using UnityEngine.Scripting;

namespace EntitiesBT.Entities
{
    public class EntityBlackboard : IBlackboard
    {
        public EntityManager EntityManager;
        public Entity Entity;
        
        private static readonly SetDataDelegate _SET_COMPONENT_DATA;
        
        static EntityBlackboard()
        {
            {
                var setter = typeof(EntityBlackboard).GetMethod("SetComponentData", BindingFlags.Public | BindingFlags.Instance);
                _SET_COMPONENT_DATA = (caller, type, value) => setter.MakeGenericMethod(type).Invoke(caller, new [] { value });
            }
        }

        public object this[object key]
        {
            get
            {
                switch (key)
                {
                case Type type:
                    return EntityManager.Debug.GetComponentBoxed(Entity, type);
                default:
                    throw new NotImplementedException();
                }
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
        
        public unsafe void* GetPtrRW(object key)
        {
            return Entity.GetComponentDataRawRW(EntityManager, key.FetchTypeIndex());
        }
        
        public unsafe void* GetPtrRO(object key)
        {
            return Entity.GetComponentDataRawRO(EntityManager, key.FetchTypeIndex());
        }

        public bool Has(object key)
        {
            return EntityManager.HasComponent(Entity, ComponentType.FromTypeIndex(key.FetchTypeIndex()));
        }
        
        [Preserve]
        public void SetComponentData<T>(T value) where T : struct, IComponentData
        {
            EntityManager.SetComponentData(Entity, value);
        }
    }
}
