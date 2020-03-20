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
        private EntityCommandBufferSystem _entityCommandBufferSystem;
        private readonly EntityBlackboard _blackboard = new EntityBlackboard();
        private readonly List<BlackboardDataQuery> _blackboardDataQueryList = new List<BlackboardDataQuery>();

        protected override void OnCreate()
        {
            _entityCommandBufferSystem = World.GetOrCreateSystem<EntityCommandBufferSystem>();
            // HACK: make system keep running if there's any entity with `BlackboardDataQuery`.
            //       otherwise, this system will be disabled if there's only `Force Run On Job` entities in the scene.
            var __hack__ = GetEntityQuery(ComponentType.ReadOnly<BlackboardDataQuery>());
        }

        protected override void OnUpdate()
        {
            RunOnMainThread();
            RunOnJob();
            ChangeThread();
        }
        
        private void RunOnMainThread()
        {
            _blackboard.EntityManager = EntityManager;
            Entities.WithoutBurst().WithAny<RunOnMainThreadTag, ForceRunOnMainThreadTag>().ForEach((Entity entity, ref NodeBlobRef blob) =>
            {
                _blackboard.Entity = entity;
                VirtualMachine.Tick(blob, _blackboard);
            }).Run();
        }
        
        struct TickJob : IJobParallelFor
        {
            [DeallocateOnJobCompletion] public NativeArray<ArchetypeChunk> Chunks;
            public EntityJobChunkBlackboard Blackboard;
            [ReadOnly] public ArchetypeChunkComponentType<NodeBlobRef> NodeBlobRefType;
            // [ReadOnly] public ArchetypeChunkEntityType EntityType;
            
            public void Execute(int index)
            {
                var chunk = Chunks[index];
                // var entities = chunk.GetNativeArray(EntityType);
                var nodeBlobs = chunk.GetNativeArray(NodeBlobRefType);
                for (var i = 0; i < chunk.Count; i++)
                {
                    Blackboard.Chunk = chunk;
                    Blackboard.Index = i;
                    // Blackboard.Entity = entities[i];
                    VirtualMachine.Tick(nodeBlobs[i], Blackboard);
                }
            }
        }

        protected void RunOnJob()
        {
            _blackboardDataQueryList.Clear();
            EntityManager.GetAllUniqueSharedComponentData(_blackboardDataQueryList);

            var jobHandler = new JobHandle();
            for (var i = 0; i < _blackboardDataQueryList.Count; i++)
            {
                var query = _blackboardDataQueryList[i];
                if (query.Value == null) continue;

                if (query.EntityQuery == null)
                {
                    query.EntityQuery = GetEntityQuery(query.Value
                        .Append(ComponentType.ReadOnly<BlackboardDataQuery>())
                        .Append(ComponentType.ReadOnly<NodeBlobRef>())
                        .Append(ComponentType.Exclude<RunOnMainThreadTag>())
                        .Append(ComponentType.Exclude<ForceRunOnMainThreadTag>())
                        .ToArray()
                    );
                    query.EntityQuery.SetSharedComponentFilter(query);
                }

                var chunks = query.EntityQuery.CreateArchetypeChunkArrayAsync(Allocator.TempJob, out var deps);
                
                var job = new TickJob {
                    Chunks = chunks
                  , Blackboard = new EntityJobChunkBlackboard { GlobalSystemVersion = EntityManager.GlobalSystemVersion }
                  , NodeBlobRefType = GetArchetypeChunkComponentType<NodeBlobRef>()
                  // , EntityType = GetArchetypeChunkEntityType()
                };

                deps = JobHandle.CombineDependencies(deps, Dependency);
                deps = job.Schedule(chunks.Length, 8, deps);
                jobHandler = JobHandle.CombineDependencies(deps, jobHandler);
            }

            Dependency = jobHandler;
        }
        
        private void ChangeThread()
        {
            var ecb = _entityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
            
            var deps = Entities.WithAll<RunOnMainThreadTag>()
                .WithNone<ForceRunOnMainThreadTag>()
                .ForEach((Entity entity, int entityInQueryIndex, ref IsRunOnMainThread isRunOnMainThread) =>
                {
                    if (!isRunOnMainThread.Value) ecb.RemoveComponent<RunOnMainThreadTag>(entityInQueryIndex, entity);
                }).ScheduleParallel(Dependency);
            
            deps = Entities.WithNone<RunOnMainThreadTag, ForceRunOnMainThreadTag, ForceRunOnJobTag>()
                .ForEach((Entity entity, int entityInQueryIndex, ref IsRunOnMainThread isRunOnMainThread) =>
                {
                    if (isRunOnMainThread.Value) ecb.AddComponent<RunOnMainThreadTag>(entityInQueryIndex, entity);
                }).ScheduleParallel(deps);
            
            Dependency = deps;
            
            // Make sure that the ECB system knows about our job
            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}
