using System;
using System.Reflection;
using System.Runtime.InteropServices;
using EntitiesBT.Core;
using Unity.Entities;
using UnityEngine.Scripting;

namespace EntitiesBT.Entities
{
    public class EntityBlackboard : IBlackboard
    {
        public EntityManager EntityManager;
        public Entity Entity;
        public EntityCommandMainThread EntityCommandMainThread = new EntityCommandMainThread();
        
        public unsafe object this[object key]
        {
            get
            {
                switch (key)
                {
                case Type type when type == typeof(IEntityCommand):
                    return EntityCommandMainThread;
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
                    var ptr = GetPtrRW(key);
                    Marshal.StructureToPtr(value, new IntPtr(ptr), false);
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
    }
}
