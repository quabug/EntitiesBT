using System;
using System.Collections.Generic;
using System.Linq;
using Entities;
using EntitiesBT.Core;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;

namespace EntitiesBT.Entities
{
    public class VirtualMachineJobSystems : ComponentSystemGroup
    {
        protected override void OnCreate()
        {
            // m_systemsToUpdate.Add(World.CreateSystem<CreateBlackboardSystem>());
            m_systemsToUpdate.Add(World.CreateSystem<VirtualMachineTickSystem>());
        }
        
        [DisableAutoCreation]
        class CreateBlackboardSystem : ComponentSystem
        {
            protected override void OnUpdate()
            {
                // Entities.WithAll<JobBehaviorTreeTag>().WithNone<JobBlackboard>().ForEach((Entity entity, BlackboardDataQuery query, ref TickDeltaTime deltaTime) =>
                // {
                //     var bb = new EntityJobChunkBlackboard();
                //     bb.Types = new Dictionary<int, IntPtr>(query.Count);
                //     EntityManager.AddComponentData(entity, new JobBlackboard { Value = bb });
                // });
                //
                // Entities.ForEach((Entity entity, BlackboardDataQuery query, ref JobBlackboard bb) =>
                // {
                //     var componentTypes = query.ReadOnlyTypes.Select(ComponentType.ReadOnly).Concat(
                //         query.ReadWriteTypes.Select(type => new ComponentType(type))
                //     );
                //
                //     foreach (var componentType in componentTypes)
                //     {
                //         EntityManager.GetComponentData<>()
                //         bb.Value.Types[componentType.TypeIndex] = GetArchetypeChunkComponentTypeDynamic(componentType);
                //     }
                // });
            }
        }
    
        [DisableAutoCreation]
        class VirtualMachineTickSystem : JobComponentSystem
        {
            struct Job : IJobChunk
            {
                public int BlackboardDataQueryIndex;
                public TimeSpan DeltaTime;
                [ReadOnly] public ArchetypeChunkSharedComponentType<BlackboardDataQuery> BlackboardDataQueryType;
                public ArchetypeChunkComponentType<NodeBlobRef> NodeBlobRefType;
                public ArchetypeChunkComponentType<JobBlackboard> BlackboardType;
                
                public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
                {
                    var index = chunk.GetSharedComponentIndex(BlackboardDataQueryType);
                    if (index != BlackboardDataQueryIndex) return;
                    
                    var nodeBlobs = chunk.GetNativeArray(NodeBlobRefType);
                    var blackboards = chunk.GetNativeArray(BlackboardType);
                    for (var i = 0; i < chunk.Count; i++)
                    {
                        var bb = blackboards[i];
                        bb.Value.Chunk = chunk;
                        bb.Value.EntityIndex = firstEntityIndex + i;
                        // // TODO: set delta time on another system?
                        // bb.Value.SetComponentData(new TickDeltaTime { Value = DeltaTime });
                        VirtualMachine.Tick(nodeBlobs[i], bb.Value);
                    }
                }
            }

            protected override JobHandle OnUpdate(JobHandle inputDeps)
            {
                var dt = TimeSpan.FromSeconds(Time.DeltaTime);
                var jobs = new Dictionary<int, (Job job, EntityQuery query)>();
                Entities.WithoutBurst().ForEach((Entity entity, BlackboardDataQuery blackboardDataQuery) =>
                {
                    var index = EntityManager.GetSharedComponentDataIndex<BlackboardDataQuery>(entity);
                    if (!jobs.ContainsKey(index))
                    {
                        var job = new Job {
                            BlackboardDataQueryIndex = index
                          , DeltaTime = dt
                          , BlackboardDataQueryType = GetArchetypeChunkSharedComponentType<BlackboardDataQuery>()
                          , BlackboardType = GetArchetypeChunkComponentType<JobBlackboard>()
                          , NodeBlobRefType = GetArchetypeChunkComponentType<NodeBlobRef>()
                        };

                        var query = EntityManager.CreateEntityQuery(
                            blackboardDataQuery.ReadOnlyTypes.Select(ComponentType.ReadOnly)
                                .Concat(blackboardDataQuery.ReadWriteTypes.Select(type => new ComponentType(type)))
                                .Append(ComponentType.ReadOnly<BlackboardDataQuery>())
                                .Append(ComponentType.ReadWrite<NodeBlobRef>())
                                .Append(ComponentType.ReadWrite<JobBlackboard>())
                                .ToArray()
                        );

                        jobs[index] = (job, query);
                    }
                }).Run();

                var jobHandle = inputDeps;
                foreach (var (job, query) in jobs.Values) jobHandle = job.Schedule(query, jobHandle);
                return jobHandle;
            }
        }
    }
}
