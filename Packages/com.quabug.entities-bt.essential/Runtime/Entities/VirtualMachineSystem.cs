using System;
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
        private EntityQuery _jobQuery;
        private EntityBlackboard _mainThreadBlackboard;

        protected override void OnCreate()
        {
            _jobQuery = GetEntityQuery(typeof(BehaviorTreeBufferElement));
            _endSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            _mainThreadBlackboard = new EntityBlackboard
            {
                EntityManager = EntityManager
            };
        }
        
        protected override unsafe void OnUpdate()
        {
            _mainThreadBlackboard.EntityCommandMainThread.EntityCommandBuffer =
                _endSimulationEntityCommandBufferSystem.CreateCommandBuffer();
            var behaviorTreeJobDeps = new JobHandle();
            Entities.WithoutBurst().ForEach((Entity entity, DynamicBuffer<BehaviorTreeBufferElement> buffers, ref CurrentBehaviorTreeComponent currentBehaviorTree) =>
            {
                _mainThreadBlackboard.Entity = entity;
                _mainThreadBlackboard.EntityCommandMainThread.Entity = entity;
                for (var i = 0; i < buffers.Length; i++)
                {
                    var bufferPtr = (IntPtr) buffers.GetUnsafeReadOnlyPtr() + UnsafeUtility.SizeOf<BehaviorTreeBufferElement>() * i;
                    ref var buffer = ref UnsafeUtility.AsRef<BehaviorTreeBufferElement>(bufferPtr.ToPointer());
                    if (buffer.RuntimeThread == BehaviorTreeRuntimeThread.MainThread
                        || buffer.RuntimeThread == BehaviorTreeRuntimeThread.ForceMainThread)
                    {
                        if (buffer.QueryMask.Matches(entity))
                        {
                            var blob = buffers[i].NodeBlob;
                            currentBehaviorTree.Value = bufferPtr;
                            VirtualMachine.Tick(ref blob, ref _mainThreadBlackboard);
                        }
                    }
                    else
                    {
                        // TODO: is this right way to do this? seems not optimize?
                        behaviorTreeJobDeps = JobHandle.CombineDependencies(behaviorTreeJobDeps, buffers[i].Dependency);
                    }
                }
            }).Run();
            
            Dependency = JobHandle.CombineDependencies(Dependency, behaviorTreeJobDeps);
            
            var chunks = _jobQuery.CreateArchetypeChunkArrayAsync(Allocator.TempJob, out var deps);
            Dependency = JobHandle.CombineDependencies(deps, Dependency);
            
            var job = new TickVirtualMachine {
                Chunks = chunks
              , Blackboard = new EntityJobChunkBlackboard { GlobalSystemVersion = GlobalSystemVersion }
              , BehaviorTreeBufferType = GetBufferTypeHandle<BehaviorTreeBufferElement>()
              , CurrentBehaviorTreeType = GetComponentTypeHandle<CurrentBehaviorTreeComponent>()
              , EntityType = GetEntityTypeHandle()
              , ECB = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter()
            };
            Dependency = job.Schedule(chunks.Length, 8, Dependency);
            
            _endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
        
        struct TickVirtualMachine : IJobParallelFor
        {
            public BufferTypeHandle<BehaviorTreeBufferElement> BehaviorTreeBufferType;
            public ComponentTypeHandle<CurrentBehaviorTreeComponent> CurrentBehaviorTreeType;
            [Unity.Collections.ReadOnly] public EntityTypeHandle EntityType;
            [DeallocateOnJobCompletion] public NativeArray<ArchetypeChunk> Chunks;
            public EntityJobChunkBlackboard Blackboard;
            public EntityCommandBuffer.ParallelWriter ECB;
            
            public unsafe void Execute(int index)
            {
                var chunk = Chunks[index];
                var entities = chunk.GetNativeArray(EntityType);
                var bufferAccessor = chunk.GetBufferAccessor(BehaviorTreeBufferType);
                var currentBehaviorTrees = chunk.GetNativeArray(CurrentBehaviorTreeType);
                for (var entityIndex = 0; entityIndex < chunk.Count; entityIndex++)
                {
                    var buffers = bufferAccessor[entityIndex];
                    for (var behaviorTreeIndex = 0; behaviorTreeIndex < buffers.Length; behaviorTreeIndex++)
                    {
                        var bufferPtr = (IntPtr) buffers.GetUnsafeReadOnlyPtr()
                                        + UnsafeUtility.SizeOf<BehaviorTreeBufferElement>() * behaviorTreeIndex;
                        ref var buffer = ref UnsafeUtility.AsRef<BehaviorTreeBufferElement>(bufferPtr.ToPointer());
                        if ((buffer.RuntimeThread == BehaviorTreeRuntimeThread.JobThread
                             || buffer.RuntimeThread == BehaviorTreeRuntimeThread.ForceJobThread)
                            && buffer.QueryMask.Matches(entities[entityIndex]))
                        {
                            Blackboard.Chunk = chunk;
                            Blackboard.EntityIndex = entityIndex;
                            Blackboard.EntityCommandJob = new EntityCommandJob(ECB, entities[entityIndex], index);
                            var blob = buffer.NodeBlob;
                            currentBehaviorTrees[entityIndex] = new CurrentBehaviorTreeComponent{Value = bufferPtr};
                            VirtualMachine.Tick(ref blob, ref Blackboard);
                        }
                    }
                }
            }
        }
    }
}
