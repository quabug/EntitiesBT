using Unity.Entities;

namespace EntitiesBT.Core
{
    public interface IEntityCommand
    {
        Entity Create(EntityArchetype archetype = new EntityArchetype());
        Entity Clone();
        void Destroy();
        
        void AddComponent<T>(T component) where T : struct, IComponentData;
        void AddComponent<T>() where T : struct, IComponentData;
        void AddComponent(ComponentType componentType);
        
        void SetComponent<T>(T component) where T : struct, IComponentData;
        
        void RemoveComponent<T>();
        void RemoveComponent(ComponentType componentType);

        void AddSharedComponent<T>(T component) where T : struct, ISharedComponentData;
        void SetSharedComponent<T>(T component) where T : struct, ISharedComponentData;
        
        DynamicBuffer<T> AddBuffer<T>() where T : struct, IBufferElementData;
        DynamicBuffer<T> SetBuffer<T>() where T : struct, IBufferElementData;
        
        Entity Clone(Entity entity);
        void Destroy(Entity entity);
        
        void AddComponent<T>(Entity entity, T component) where T : struct, IComponentData;
        void AddComponent<T>(Entity entity) where T : struct, IComponentData;
        void AddComponent(Entity entity, ComponentType componentType);
        
        void SetComponent<T>(Entity entity, T component) where T : struct, IComponentData;
        
        void RemoveComponent<T>(Entity entity);
        void RemoveComponent(Entity entity, ComponentType componentType);

        void AddSharedComponent<T>(Entity entity, T component) where T : struct, ISharedComponentData;
        void SetSharedComponent<T>(Entity entity, T component) where T : struct, ISharedComponentData;
        
        DynamicBuffer<T> AddBuffer<T>(Entity entity) where T : struct, IBufferElementData;
        DynamicBuffer<T> SetBuffer<T>(Entity entity) where T : struct, IBufferElementData;
    }
}
