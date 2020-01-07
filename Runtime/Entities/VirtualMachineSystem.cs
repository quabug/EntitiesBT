using System.Collections.Generic;
using System.Linq;
using Entities;
using EntitiesBT.Core;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace EntitiesBT.Entities
{
    public class VirtualMachineSystem : ComponentSystemGroup
    {
        protected override void OnCreate()
        {
            m_systemsToUpdate.Add(World.CreateSystem<DeltaTimeSystem>());
            m_systemsToUpdate.Add(World.CreateSystem<MainThreadSystem>());
            m_systemsToUpdate.Add(World.CreateSystem<JobSystem>());
            m_systemsToUpdate.Add(World.CreateSystem<ChangeThreadSystem>());
        }

        [DisableAutoCreation]
        class DeltaTimeSystem : ComponentSystem
        {
            protected override void OnUpdate()
            {
                Entities.ForEach((ref BehaviorTreeTickDeltaTime deltaTime) => deltaTime.Value = Time.DeltaTime);
            }
        }
        
        [DisableAutoCreation]
        class ChangeThreadSystem : ComponentSystem
        {
            protected override void OnUpdate()
            {
                Entities.WithAll<RunOnMainThreadTag>()
                    .WithNone<ForceRunOnMainThreadTag>()
                    .ForEach((Entity entity, ref IsRunOnMainThread isRunOnMainThread) =>
                    {
                        if (!isRunOnMainThread.Value) EntityManager.RemoveComponent<RunOnMainThreadTag>(entity);
                    });
                
                Entities.WithNone<RunOnMainThreadTag, ForceRunOnMainThreadTag, ForceRunOnJobTag>()
                    .ForEach((Entity entity, ref IsRunOnMainThread isRunOnMainThread) =>
                    {
                        if (isRunOnMainThread.Value) EntityManager.AddComponent<RunOnMainThreadTag>(entity);
                    });
            }
        }

        [DisableAutoCreation]
        class MainThreadSystem : ComponentSystem
        {
            private EntityBlackboard _blackboard = new EntityBlackboard();

            protected override void OnUpdate()
            {
                _blackboard.EntityManager = EntityManager;
                Entities.WithAny<RunOnMainThreadTag, ForceRunOnMainThreadTag>().ForEach((Entity entity, ref NodeBlobRef blob) =>
                {
                    _blackboard.Entity = entity;
                    VirtualMachine.Tick(blob, _blackboard);
                });
            }
        }
        
        [DisableAutoCreation]
        class JobSystem : JobComponentSystem
        {
            private readonly List<BlackboardDataQuery> _blackboardDataQueryList = new List<BlackboardDataQuery>();
            private readonly List<int> _blackboardDataQueryIndices = new List<int>();
            
            struct TickJob : IJobChunk
            {
                public EntityJobChunkBlackboard Blackboard;
                public int BlackboardDataQueryIndex;
                
                [ReadOnly] public ArchetypeChunkSharedComponentType<BlackboardDataQuery> BlackboardDataQueryType;
                [ReadOnly] public ArchetypeChunkComponentType<NodeBlobRef> NodeBlobRefType;
                // [ReadOnly] public ArchetypeChunkEntityType EntityType;
                
                public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
                {
                    var index = chunk.GetSharedComponentIndex(BlackboardDataQueryType);
                    if (index != BlackboardDataQueryIndex) return;

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

            protected override JobHandle OnUpdate(JobHandle inputDeps)
            {
                _blackboardDataQueryList.Clear();
                _blackboardDataQueryIndices.Clear();
                EntityManager.GetAllUniqueSharedComponentData(_blackboardDataQueryList, _blackboardDataQueryIndices);

                var jobHandler = new JobHandle();
                for (var i = 0; i < _blackboardDataQueryList.Count; i++)
                {
                    var query = _blackboardDataQueryList[i];
                    if (query.Value == null) continue;
                    
                    var sharedIndex = _blackboardDataQueryIndices[i];
                    
                    // TODO: avoid GC? use NativeArray?
                    var jobQuery = GetEntityQuery(query.Value
                        .Append(ComponentType.ReadOnly<BlackboardDataQuery>())
                        .Append(ComponentType.ReadOnly<NodeBlobRef>())
                        .Append(ComponentType.Exclude<RunOnMainThreadTag>())
                        .Append(ComponentType.Exclude<ForceRunOnMainThreadTag>())
                        .ToArray()
                    );
                    
                    var job = new TickJob {
                        BlackboardDataQueryIndex = sharedIndex
                      , Blackboard = new EntityJobChunkBlackboard { GlobalSystemVersion = EntityManager.GlobalSystemVersion }
                      , BlackboardDataQueryType = GetArchetypeChunkSharedComponentType<BlackboardDataQuery>()
                      , NodeBlobRefType = GetArchetypeChunkComponentType<NodeBlobRef>()
                      // , EntityType = GetArchetypeChunkEntityType()
                    };
                    
                    jobHandler = JobHandle.CombineDependencies(job.Schedule(jobQuery, inputDeps), jobHandler);
                }
                return jobHandler;
            }
        }
    }
}
