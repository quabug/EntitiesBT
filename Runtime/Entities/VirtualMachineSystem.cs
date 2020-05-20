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
            _jobQuery = GetEntityQuery(typeof(BehaviorTreeBufferElement));
            _endSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }
        
        protected override void OnUpdate()
        {
            _mainThreadBlackboard.EntityCommandMainThread.EntityCommandBuffer = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer();
            _mainThreadBlackboard.EntityManager = EntityManager;
            
            var behaviorTreeDeps = new JobHandle();
            Entities.WithoutBurst().ForEach((Entity entity, DynamicBuffer<BehaviorTreeBufferElement> buffers) =>
            {
                for (var i = 0; i < buffers.Length; i++)
                {
                    if (buffers[i].RuntimeThread == BehaviorTreeRuntimeThread.MainThread
                        || buffers[i].RuntimeThread == BehaviorTreeRuntimeThread.ForceMainThread)
                    {
                        if (buffers[i].QueryMask.Matches(entity))
                        {
                            _mainThreadBlackboard.EntityCommandMainThread.Entity = entity;
                            _mainThreadBlackboard.Entity = entity;
                            _mainThreadBlackboard.BehaviorTreeIndex = i;
                            VirtualMachine.Tick(buffers[i].NodeBlob, _mainThreadBlackboard);
                        }
                    }
                    else
                    {
                        // TODO: is this right way to do this? seems not optimize?
                        behaviorTreeDeps = JobHandle.CombineDependencies(behaviorTreeDeps, buffers[i].Dependency);
                    }
                }
            }).Run();
            
            Dependency = JobHandle.CombineDependencies(Dependency, behaviorTreeDeps);
            
            var behaviorTreeBufferType = GetArchetypeChunkBufferType<BehaviorTreeBufferElement>();
            var entityType = GetArchetypeChunkEntityType();
            var chunks = _jobQuery.CreateArchetypeChunkArrayAsync(Allocator.TempJob, out var deps);
            var ecb = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
            var job = new TickVirtualMachine {
                Chunks = chunks
              , Blackboard = new EntityJobChunkBlackboard { GlobalSystemVersion = GlobalSystemVersion }
              , BehaviorTreeBufferType = behaviorTreeBufferType
              , EntityType = entityType
              , ECB = ecb
            };
            
            Dependency = JobHandle.CombineDependencies(deps, Dependency);
            Dependency = job.Schedule(chunks.Length, 8, Dependency);
            _endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
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
