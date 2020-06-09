using System;
using EntitiesBT.Core;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;

namespace EntitiesBT.Entities
{
    public struct EntityBlackboard : IBlackboard
    {
        public EntityManager EntityManager;
        public Entity Entity;
        public EntityCommandMainThread EntityCommandMainThread;
        public int BehaviorTreeIndex;
        
        public bool HasData<T>() where T : struct
        {
            return EntityManager.HasComponent<T>(Entity);
        }

        public unsafe T GetData<T>() where T : struct
        {
            var index = TypeManager.GetTypeIndex<T>();
            var ptr = Entity.GetComponentDataRawRO(EntityManager, index);
            return UnsafeUtilityEx.AsRef<T>(ptr);
        }

        public unsafe ref T GetDataRef<T>() where T : struct
        {
            var type = typeof(T);
            if (type == typeof(BehaviorTreeBufferElement))
            {
                var offset = (long) BehaviorTreeIndex * UnsafeUtility.SizeOf<BehaviorTreeBufferElement>();
                var ptr = (byte*)EntityManager.GetBuffer<BehaviorTreeBufferElement>(Entity).GetUnsafePtr() + offset;
                return ref UnsafeUtilityEx.AsRef<T>(ptr);
            }
            else
            {
                var index = TypeManager.GetTypeIndex<T>();
                var ptr = Entity.GetComponentDataRawRW(EntityManager, index);
                return ref UnsafeUtilityEx.AsRef<T>(ptr);
            }
        }

        public unsafe IntPtr GetDataPtrRO(Type type)
        {
            return new IntPtr(Entity.GetComponentDataRawRO(EntityManager, TypeManager.GetTypeIndex(type)));
        }

        public unsafe IntPtr GetDataPtrRW(Type type)
        {
            return new IntPtr(Entity.GetComponentDataRawRW(EntityManager, TypeManager.GetTypeIndex(type)));
        }

        public T GetObject<T>() where T : class
        {
            if (typeof(T) == typeof(IEntityCommand))
                return EntityCommandMainThread as T;
            return EntityManager.GetComponentObject<T>(Entity);
        }
    }
}
