using EntitiesBT.Core;
using Unity.Entities;

namespace EntitiesBT.Entities
{
    public class EntityCommandMainThread : IEntityCommand
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
    }
}
