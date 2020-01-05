using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesBT.Core;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace EntitiesBT.Entities
{
    public class VirtualMachineJobSystems : JobComponentSystem
    {
        struct TickJob : IJobChunk
        {
            public int BlackboardDataQueryIndex;
            public TimeSpan DeltaTime;
            public uint GlobalSystemVersion;
            
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
                    bb.Value.GlobalSystemVersion = GlobalSystemVersion;
                    bb.Value.Chunk = chunk;
                    bb.Value.EntityIndex = firstEntityIndex + i;
                    bb.Value.GetDataRef<TickDeltaTime>().Value = DeltaTime;
                    VirtualMachine.Tick(nodeBlobs[i], bb.Value);
                }
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var dt = TimeSpan.FromSeconds(Time.DeltaTime);
            var jobs = new Dictionary<int, (TickJob job, EntityQuery query)>();
            Entities.WithoutBurst().ForEach((Entity entity, BlackboardDataQuery blackboardDataQuery) =>
            {
                var index = EntityManager.GetSharedComponentDataIndex<BlackboardDataQuery>(entity);
                if (!jobs.ContainsKey(index))
                {
                    var job = new TickJob {
                        BlackboardDataQueryIndex = index
                      , DeltaTime = dt
                      , GlobalSystemVersion = EntityManager.GlobalSystemVersion
                      , BlackboardDataQueryType = GetArchetypeChunkSharedComponentType<BlackboardDataQuery>()
                      , BlackboardType = GetArchetypeChunkComponentType<JobBlackboard>()
                      , NodeBlobRefType = GetArchetypeChunkComponentType<NodeBlobRef>()
                    };

                    var query = EntityManager.CreateEntityQuery(
                        blackboardDataQuery.Value.Select(type => new ComponentType(type))
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
