using EntitiesBT.Core;
using Unity.Entities;

namespace EntitiesBT.Entities
{
    public struct EntityCommandMainThread : IEntityCommand
    {
        public EntityCommandBuffer EntityCommandBuffer;
        public Entity Entity;

        public Entity Create(EntityArchetype archetype = new EntityArchetype())
        {
            return EntityCommandBuffer.CreateEntity(archetype);
        }

        public Entity Clone()
        {
            return EntityCommandBuffer.Instantiate(Entity);
        }

        public void Destroy()
        {
            EntityCommandBuffer.DestroyEntity(Entity);
        }

        public void AddComponent<T>(T component) where T : struct, IComponentData
        {
            EntityCommandBuffer.AddComponent(Entity, component);
        }

        public void AddComponent<T>() where T : struct, IComponentData
        {
            EntityCommandBuffer.AddComponent<T>(Entity);
        }

        public void AddComponent(ComponentType componentType)
        {
            EntityCommandBuffer.AddComponent(Entity, componentType);
        }

        public void SetComponent<T>(T component) where T : struct, IComponentData
        {
            EntityCommandBuffer.SetComponent(Entity, component);
        }

        public void RemoveComponent<T>()
        {
            EntityCommandBuffer.RemoveComponent<T>(Entity);
        }

        public void RemoveComponent(ComponentType componentType)
        {
            EntityCommandBuffer.RemoveComponent(Entity, componentType);
        }

        public void AddSharedComponent<T>(T component) where T : struct, ISharedComponentData
        {
            EntityCommandBuffer.AddSharedComponent(Entity, component);
        }

        public void SetSharedComponent<T>(T component) where T : struct, ISharedComponentData
        {
            EntityCommandBuffer.SetSharedComponent(Entity, component);
        }

        public DynamicBuffer<T> AddBuffer<T>() where T : struct, IBufferElementData
        {
            return EntityCommandBuffer.AddBuffer<T>(Entity);
        }

        public DynamicBuffer<T> SetBuffer<T>() where T : struct, IBufferElementData
        {
            return EntityCommandBuffer.SetBuffer<T>(Entity);
        }

        public Entity Clone(Entity entity)
        {
            return EntityCommandBuffer.Instantiate(entity);
        }

        public void Destroy(Entity entity)
        {
            EntityCommandBuffer.DestroyEntity(entity);
        }

        public void AddComponent<T>(Entity entity, T component) where T : struct, IComponentData
        {
            EntityCommandBuffer.AddComponent(entity, component);
        }

        public void AddComponent<T>(Entity entity) where T : struct, IComponentData
        {
            EntityCommandBuffer.AddComponent<T>(entity);
        }

        public void AddComponent(Entity entity, ComponentType componentType)
        {
            EntityCommandBuffer.AddComponent(entity, componentType);
        }

        public void SetComponent<T>(Entity entity, T component) where T : struct, IComponentData
        {
            EntityCommandBuffer.SetComponent(entity, component);
        }

        public void RemoveComponent<T>(Entity entity)
        {
            EntityCommandBuffer.RemoveComponent<T>(entity);
        }

        public void RemoveComponent(Entity entity, ComponentType componentType)
        {
            EntityCommandBuffer.RemoveComponent(entity, componentType);
        }

        public void AddSharedComponent<T>(Entity entity, T component) where T : struct, ISharedComponentData
        {
            EntityCommandBuffer.AddSharedComponent(entity, component);
        }

        public void SetSharedComponent<T>(Entity entity, T component) where T : struct, ISharedComponentData
        {
            EntityCommandBuffer.SetSharedComponent(entity, component);
        }

        public DynamicBuffer<T> AddBuffer<T>(Entity entity) where T : struct, IBufferElementData
        {
            return EntityCommandBuffer.AddBuffer<T>(entity);
        }

        public DynamicBuffer<T> SetBuffer<T>(Entity entity) where T : struct, IBufferElementData
        {
            return EntityCommandBuffer.SetBuffer<T>(entity);
        }
    }
}
