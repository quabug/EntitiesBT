using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

namespace EntitiesBT.Sample
{
    public partial class RuntimeEntitySystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities
                .WithoutBurst()
                .WithStructuralChanges()
                .WithAll<EntitySpawnData>()
                .ForEach((Entity entity) => EntityManager.AddBuffer<EntityLifeTimeBuffer>(entity))
                .Run()
            ;

            var deltaTime = TimeSpan.FromSeconds(World.Time.DeltaTime);
            var random = new Random((uint)Environment.TickCount);
            Entities
                .WithoutBurst()
                .WithStructuralChanges()
                .ForEach((Entity spawner, DynamicBuffer<EntityLifeTimeBuffer> entities, in EntitySpawnData data) =>
                {
                    var ecb = new EntityCommandBuffer(Allocator.Temp);

                    var newEntities = ecb.SetBuffer<EntityLifeTimeBuffer>(spawner);
                    foreach (var entity in entities)
                    {
                        var buffer = entity;
                        buffer.LifeTime -= deltaTime;
                        if (buffer.LifeTime <= TimeSpan.Zero) ecb.DestroyEntity(buffer.Entity);
                        else newEntities.Add(buffer);
                    }

                    var count = data.Count - entities.Length;
                    for (var i = 0; i < count; i++)
                    {
                        var instance = ecb.Instantiate(data.Prefab);
                        var position = random.NextFloat3(data.PositionBounds.min, data.PositionBounds.max);
                        ecb.SetComponent(instance, new Translation {Value = position});
                        var lifetime = TimeSpan.FromSeconds(random.NextFloat(data.LifeTimeRange.x, data.LifeTimeRange.y));
                        newEntities.Add(new EntityLifeTimeBuffer {Entity = instance, LifeTime = lifetime});
                    }

                    ecb.Playback(EntityManager);
                    ecb.Dispose();
                }).Run();
        }
    }
}