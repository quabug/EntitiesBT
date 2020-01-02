using System;
using System.Collections.Generic;
using System.Linq;
using Entities;
using EntitiesBT.Core;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace EntitiesBT.Entities
{
    public class VirtualMachineJobSystems : ComponentSystemGroup
    {
        protected override void OnCreate()
        {
            m_systemsToUpdate.Add(World.CreateSystem<CreateBlackboardSystem>());
            m_systemsToUpdate.Add(World.CreateSystem<VirtualMachineTickSystem>());
        }
        
        [DisableAutoCreation]
        class CreateBlackboardSystem : ComponentSystem
        {
            protected override void OnUpdate()
            {
                Entities.WithAll<JobBehaviorTreeTag>().WithNone<JobBlackboard>().ForEach((Entity entity, BlackboardDataQuery query) =>
                {
                    var bb = new EntityJobChunkBlackboard();
                    bb.Types = new NativeHashMap<int, ArchetypeChunkComponentTypeDynamic>(query.Count, Allocator.Persistent);
                    EntityManager.AddComponentData(entity, new JobBlackboard { Value = bb });
                });
                
                Entities.ForEach((Entity entity, BlackboardDataQuery query, ref JobBlackboard bb) =>
                {
                    var componentTypes = query.ReadOnlyTypes.Select(ComponentType.ReadOnly).Concat(
                        query.ReadWriteTypes.Select(type => new ComponentType(type))
                    );
                    foreach (var componentType in componentTypes)
                        bb.Value.Types[componentType.TypeIndex] = GetArchetypeChunkComponentTypeDynamic(componentType);
                });
            }
        }
    
        [DisableAutoCreation]
        class VirtualMachineTickSystem : JobComponentSystem
        {
            struct Job : IJobChunk
            {
                public ArchetypeChunkComponentType<NodeBlobRef> NodeBlobRefType;
                public ArchetypeChunkComponentType<JobBlackboard> BlackboardType;
                public TimeSpan DeltaTime;
                
                public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
                {
                    var nodeBlobs = chunk.GetNativeArray(NodeBlobRefType);
                    var blackboards = chunk.GetNativeArray(BlackboardType);
                    for (var i = 0; i < chunk.Count; i++)
                    {
                        var bb = blackboards[i];
                        bb.Value.Chunk = chunk;
                        bb.Value.EntityIndex = firstEntityIndex + i;
                        // TODO: set delta time on another system?
                        bb.Value.SetComponentData(new TickDeltaTime { Value = DeltaTime });
                        VirtualMachine.Tick(nodeBlobs[i], bb.Value);
                    }
                }
            }

            protected override JobHandle OnUpdate(JobHandle inputDeps)
            {
                var dt = TimeSpan.FromSeconds(Time.DeltaTime);
                var job = new Job
                {
                    DeltaTime = dt
                  , BlackboardType = GetArchetypeChunkComponentType<JobBlackboard>()
                  , NodeBlobRefType = GetArchetypeChunkComponentType<NodeBlobRef>()
                };
                return job.Schedule(GetEntityQuery(ComponentType.ReadOnly<JobBlackboard>()), inputDeps);
            }
        }
    }
}
