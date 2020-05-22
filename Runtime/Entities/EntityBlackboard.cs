using System;
using System.Runtime.InteropServices;
using EntitiesBT.Core;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;

namespace EntitiesBT.Entities
{
    public class EntityBlackboard : IBlackboard
    {
        public EntityManager EntityManager;
        public Entity Entity;
        public EntityCommandMainThread EntityCommandMainThread = new EntityCommandMainThread();
        public int BehaviorTreeIndex;
        
        public unsafe object this[object key]
        {
            get
            {
                if (key is Type type)
                {
                    if (type == typeof(IEntityCommand))
                        return EntityCommandMainThread;
                    if (type == typeof(BehaviorTreeBufferElement))
                        return EntityManager.GetBuffer<BehaviorTreeBufferElement>(Entity)[BehaviorTreeIndex];
                    if (type.IsValueType && typeof(IComponentData).IsAssignableFrom(type))
                    {
                        var typeIndex = TypeManager.GetTypeIndex(type);
                        var ptr = Entity.GetComponentDataRawRO(EntityManager, typeIndex);
                        return Marshal.PtrToStructure(new IntPtr(ptr), type);
                    }
                    return EntityManager.Debug.GetComponentBoxed(Entity, type);
                }
                throw new NotImplementedException();
            }
            set
            {
                if (key is Type type && type.IsValueType)
                {
                    var typeIndex = TypeManager.GetTypeIndex(type);
                    var typeInfo = TypeManager.GetTypeInfo(typeIndex);
                    if (typeInfo.Category == TypeManager.TypeCategory.ComponentData)
                    {
                        var ptr = Entity.GetComponentDataRawRW(EntityManager, typeIndex);
                        Marshal.StructureToPtr(value, new IntPtr(ptr), false);
                        return;
                    }
                }
                throw new NotImplementedException();
            }
        }
        
        public unsafe void* GetPtrRW(object key)
        {
            if (key is Type type && type == typeof(BehaviorTreeBufferElement))
            {
                var offset = (long) BehaviorTreeIndex * UnsafeUtility.SizeOf<BehaviorTreeBufferElement>();
                return (byte*)EntityManager.GetBuffer<BehaviorTreeBufferElement>(Entity).GetUnsafePtr() + offset;
            }
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
