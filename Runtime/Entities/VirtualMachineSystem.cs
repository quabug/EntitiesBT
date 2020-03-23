using System.Collections.Generic;
using System.Linq;
using EntitiesBT.Core;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace EntitiesBT.Entities
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class VirtualMachineSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _endSimulationEntityCommandBufferSystem;
        private readonly EntityBlackboard _mainThreadBlackboard = new EntityBlackboard();
        private readonly List<BlackboardDataQuery> _blackboardDataQueryList = new List<BlackboardDataQuery>();

        protected override void OnCreate()
        {
            _endSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            _mainThreadBlackboard.EntityCommandMainThread.EntityCommandBuffer = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer();
            _mainThreadBlackboard.EntityManager = EntityManager;

            var nodeBlobRefType = EntityManager.GetArchetypeChunkComponentType<NodeBlobRef>(true);
            var entityType = EntityManager.GetArchetypeChunkEntityType();
            
            _blackboardDataQueryList.Clear();
            EntityManager.GetAllUniqueSharedComponentData(_blackboardDataQueryList);

            var jobHandler = new JobHandle();
            for (var i = 0; i < _blackboardDataQueryList.Count; i++)
            {
                var query = _blackboardDataQueryList[i];
                if (query.Value == null) continue;
                
                RunOnMainThread(query);
                var deps = RunOnJob(query);
                jobHandler = JobHandle.CombineDependencies(jobHandler, deps);
            }

            Dependency = jobHandler;
            _endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(Dependency);

            JobHandle RunOnJob(in BlackboardDataQuery query)
            {
                query.QueryJob.SetSharedComponentFilter(query);

                var chunks = query.QueryJob.CreateArchetypeChunkArrayAsync(Allocator.TempJob, out var deps);
                var ecb = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
                var job = new TickVirtualMachine {
                    Chunks = chunks
                  , Blackboard = new EntityJobChunkBlackboard { GlobalSystemVersion = EntityManager.GlobalSystemVersion }
                  , NodeBlobRefType = nodeBlobRefType
                  , EntityType = entityType
                  , ECB = ecb
                };
                
                deps = JobHandle.CombineDependencies(deps, Dependency);
                return job.Schedule(chunks.Length, 8, deps);
            }
        
            void RunOnMainThread(in BlackboardDataQuery query)
            {
                query.QueryMainThread.SetSharedComponentFilter(query);
                
                using (var chunks = query.QueryMainThread.CreateArchetypeChunkArray(Allocator.TempJob))
                    foreach (var chunk in chunks)
                    {
                        var entities = chunk.GetNativeArray(entityType);
                        var nodeBlobs = chunk.GetNativeArray(nodeBlobRefType);
                        for (var i = 0; i < chunk.Count; i++)
                        {
                            _mainThreadBlackboard.EntityCommandMainThread.Entity = entities[i];
                            _mainThreadBlackboard.Entity = entities[i];
                            VirtualMachine.Tick(nodeBlobs[i], _mainThreadBlackboard);
                        }
                    }
            }
        }
        
        struct TickVirtualMachine : IJobParallelFor
        {
            [ReadOnly] public ArchetypeChunkComponentType<NodeBlobRef> NodeBlobRefType;
            [ReadOnly] public ArchetypeChunkEntityType EntityType;
            [DeallocateOnJobCompletion] public NativeArray<ArchetypeChunk> Chunks;
            public EntityJobChunkBlackboard Blackboard;
            public EntityCommandBuffer.Concurrent ECB;
            
            public void Execute(int index)
            {
                var chunk = Chunks[index];
                var entities = chunk.GetNativeArray(EntityType);
                var nodeBlobs = chunk.GetNativeArray(NodeBlobRefType);
                for (var i = 0; i < chunk.Count; i++)
                {
                    Blackboard.Chunk = chunk;
                    Blackboard.EntityIndex = i;
                    Blackboard.EntityCommandJob = new EntityCommandJob(ECB, entities[i], index);
                    VirtualMachine.Tick(nodeBlobs[i], Blackboard);
                }
            }
        }
    }
}
