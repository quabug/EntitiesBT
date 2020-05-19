using System.Collections.Generic;
using System.Linq;
using EntitiesBT.Core;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;

namespace EntitiesBT.Entities
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class VirtualMachineSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _endSimulationEntityCommandBufferSystem;
        private readonly EntityBlackboard _mainThreadBlackboard = new EntityBlackboard();
        private EntityQuery _jobQuery;

        protected override void OnCreate()
        {
            _jobQuery = EntityManager.CreateEntityQuery(typeof(BehaviorTreeBufferElement));
            _endSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }
        
        protected override void OnUpdate()
        {
            _mainThreadBlackboard.EntityCommandMainThread.EntityCommandBuffer = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer();
            _mainThreadBlackboard.EntityManager = EntityManager;
            
            Entities.WithoutBurst().ForEach((Entity entity, DynamicBuffer<BehaviorTreeBufferElement> buffers) =>
            {
                for (var i = 0; i < buffers.Length; i++)
                {
                    if ((buffers[i].RuntimeThread == BehaviorTreeRuntimeThread.MainThread
                         || buffers[i].RuntimeThread == BehaviorTreeRuntimeThread.ForceMainThread)
                         && buffers[i].QueryMask.Matches(entity)
                    )
                    {
                        _mainThreadBlackboard.EntityCommandMainThread.Entity = entity;
                        _mainThreadBlackboard.Entity = entity;
                        VirtualMachine.Tick(buffers[i].NodeBlob, _mainThreadBlackboard);
                    }
                }
            }).Run();
            
            var behaviorTreeBufferType = EntityManager.GetArchetypeChunkBufferType<BehaviorTreeBufferElement>(false);
            var entityType = EntityManager.GetArchetypeChunkEntityType();
            var chunks = _jobQuery.CreateArchetypeChunkArrayAsync(Allocator.TempJob, out var deps);
            var ecb = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
            var job = new TickVirtualMachine {
                Chunks = chunks
              , Blackboard = new EntityJobChunkBlackboard { GlobalSystemVersion = EntityManager.GlobalSystemVersion }
              , BehaviorTreeBufferType = behaviorTreeBufferType
              , EntityType = entityType
              , ECB = ecb
            };
            
            deps = JobHandle.CombineDependencies(deps, Dependency);
            deps = job.Schedule(chunks.Length, 8, deps);
            _endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(deps);
        }
        
        struct TickVirtualMachine : IJobParallelFor
        {
            public ArchetypeChunkBufferType<BehaviorTreeBufferElement> BehaviorTreeBufferType;
            [Unity.Collections.ReadOnly] public ArchetypeChunkEntityType EntityType;
            [DeallocateOnJobCompletion] public NativeArray<ArchetypeChunk> Chunks;
            public EntityJobChunkBlackboard Blackboard;
            public EntityCommandBuffer.Concurrent ECB;
            
            public unsafe void Execute(int index)
            {
                var chunk = Chunks[index];
                var entities = chunk.GetNativeArray(EntityType);
                var bufferAccessor = chunk.GetBufferAccessor(BehaviorTreeBufferType);
                for (var entityIndex = 0; entityIndex < chunk.Count; entityIndex++)
                {
                    var buffers = bufferAccessor[entityIndex];
                    for (var behaviorTreeIndex = 0; behaviorTreeIndex < buffers.Length; behaviorTreeIndex++)
                    {
                        if ((buffers[behaviorTreeIndex].RuntimeThread == BehaviorTreeRuntimeThread.JobThread
                             || buffers[behaviorTreeIndex].RuntimeThread == BehaviorTreeRuntimeThread.ForceJobThread)
                            && buffers[behaviorTreeIndex].QueryMask.Matches(entities[entityIndex]))
                        {
                            Blackboard.Chunk = chunk;
                            Blackboard.EntityIndex = entityIndex;
                            Blackboard.EntityCommandJob = new EntityCommandJob(ECB, entities[entityIndex], index);
                            var offset = (long) behaviorTreeIndex * UnsafeUtility.SizeOf<BehaviorTreeBufferElement>();
                            Blackboard.BehaviorTreeElementPtr = (byte*) buffers.GetUnsafePtr() + offset;
                            VirtualMachine.Tick(buffers[behaviorTreeIndex].NodeBlob, Blackboard);
                        }
                    }
                }
            }
        }
    }
}
