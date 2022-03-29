using EntitiesBT.Core;
using Unity.Entities;

namespace EntitiesBT.Entities
{
    public struct EntityCommandJob : IEntityCommand
    {
        private EntityCommandBuffer.ParallelWriter _ecb;
        private readonly Entity _entity;
        private readonly int _jobIndex;
        
        public EntityCommandJob(EntityCommandBuffer.ParallelWriter ecb, Entity entity, int jobIndex)
        {
            _ecb = ecb;
            _entity = entity;
            _jobIndex = jobIndex;
        }

        public Entity Create(EntityArchetype archetype = new EntityArchetype())
        {
            return _ecb.CreateEntity(_jobIndex, archetype);
        }

        public Entity Clone()
        {
            return _ecb.Instantiate(_jobIndex, _entity);
        }

        public void Destroy()
        {
            _ecb.DestroyEntity(_jobIndex, _entity);
        }

        public void AddComponent<T>(T component) where T : struct, IComponentData
        {
            _ecb.AddComponent(_jobIndex, _entity, component);
        }

        public void AddComponent<T>() where T : struct, IComponentData
        {
            _ecb.AddComponent<T>(_jobIndex, _entity);
        }

        public void AddComponent(ComponentType componentType)
        {
            _ecb.AddComponent(_jobIndex, _entity, componentType);
        }
        
        public DynamicBuffer<T> AddBuffer<T>() where T : struct, IBufferElementData
        {
            return _ecb.AddBuffer<T>(_jobIndex, _entity);
        }
        
        public DynamicBuffer<T> SetBuffer<T>() where T : struct, IBufferElementData
        {
            return _ecb.SetBuffer<T>(_jobIndex, _entity);
        }
        
        public void SetComponent<T>(T component) where T : struct, IComponentData
        {
            _ecb.SetComponent(_jobIndex, _entity, component);
        }
        
        public void RemoveComponent<T>()
        {
            _ecb.RemoveComponent<T>(_jobIndex, _entity);
        }
        
        public void RemoveComponent(ComponentType componentType)
        {
            _ecb.RemoveComponent(_jobIndex, _entity, componentType);
        }
        
        public void AddSharedComponent<T>(T component) where T : struct, ISharedComponentData
        {
            _ecb.AddSharedComponent(_jobIndex, _entity, component);
        }
        
        public void SetSharedComponent<T>(T component) where T : struct, ISharedComponentData
        {
            _ecb.SetSharedComponent(_jobIndex, _entity, component);
        }

        public Entity Clone(Entity entity)
        {
            return _ecb.Instantiate(_jobIndex, entity);
        }

        public void Destroy(Entity entity)
        {
            _ecb.DestroyEntity(_jobIndex, entity);
        }

        public void AddComponent<T>(Entity entity, T component) where T : struct, IComponentData
        {
            _ecb.AddComponent(_jobIndex, entity, component);
        }

        public void AddComponent<T>(Entity entity) where T : struct, IComponentData
        {
            _ecb.AddComponent<T>(_jobIndex, entity);
        }

        public void AddComponent(Entity entity, ComponentType componentType)
        {
            _ecb.AddComponent(_jobIndex, entity, componentType);
        }
        
        public DynamicBuffer<T> AddBuffer<T>(Entity entity) where T : struct, IBufferElementData
        {
            return _ecb.AddBuffer<T>(_jobIndex, entity);
        }
        
        public DynamicBuffer<T> SetBuffer<T>(Entity entity) where T : struct, IBufferElementData
        {
            return _ecb.SetBuffer<T>(_jobIndex, entity);
        }
        
        public void SetComponent<T>(Entity entity, T component) where T : struct, IComponentData
        {
            _ecb.SetComponent(_jobIndex, entity, component);
        }
        
        public void RemoveComponent<T>(Entity entity)
        {
            _ecb.RemoveComponent<T>(_jobIndex, entity);
        }
        
        public void RemoveComponent(Entity entity, ComponentType componentType)
        {
            _ecb.RemoveComponent(_jobIndex, entity, componentType);
        }
        
        public void AddSharedComponent<T>(Entity entity, T component) where T : struct, ISharedComponentData
        {
            _ecb.AddSharedComponent(_jobIndex, entity, component);
        }
        
        public void SetSharedComponent<T>(Entity entity, T component) where T : struct, ISharedComponentData
        {
            _ecb.SetSharedComponent(_jobIndex, entity, component);
        }
    }
}
