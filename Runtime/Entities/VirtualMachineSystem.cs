using System.Collections.Generic;
using System.Linq;
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
                
                Entities.WithNone<RunOnMainThreadTag, ForceRunOnMainThreadTag>()
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
                public int BlackboardDataQueryIndex;
                public uint GlobalSystemVersion;
                
                [ReadOnly] public ArchetypeChunkSharedComponentType<BlackboardDataQuery> BlackboardDataQueryType;
                [ReadOnly] public ArchetypeChunkComponentType<NodeBlobRef> NodeBlobRefType;
                [ReadOnly] public ArchetypeChunkComponentType<JobBlackboard> BlackboardType;
                [ReadOnly] public ArchetypeChunkEntityType EntityType;
                
                public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
                {
                    var index = chunk.GetSharedComponentIndex(BlackboardDataQueryType);
                    if (index != BlackboardDataQueryIndex) return;

                    var entities = chunk.GetNativeArray(EntityType);
                    var nodeBlobs = chunk.GetNativeArray(NodeBlobRefType);
                    var blackboards = chunk.GetNativeArray(BlackboardType);
                    for (var i = 0; i < chunk.Count; i++)
                    {
                        var bb = blackboards[i];
                        bb.Value.GlobalSystemVersion = GlobalSystemVersion;
                        bb.Value.Chunk = chunk;
                        bb.Value.Index = i;
                        bb.Value.Entity = entities[i];
                        VirtualMachine.Tick(nodeBlobs[i], bb.Value);
                    }
                }
            }

            protected override JobHandle OnUpdate(JobHandle inputDeps)
            {
                Entities.WithoutBurst()
                    .WithAll<JobBlackboard>()
                    .ForEach((ref BehaviorTreeTickDeltaTime dt) => dt.Value = Time.DeltaTime)
                    .Run()
                ;
                
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
                        .Append(ComponentType.ReadOnly<JobBlackboard>())
                        .Append(ComponentType.ReadOnly<NodeBlobRef>())
                        .Append(ComponentType.Exclude<RunOnMainThreadTag>())
                        .Append(ComponentType.Exclude<ForceRunOnMainThreadTag>())
                        .ToArray()
                    );
                    
                    var job = new TickJob {
                        BlackboardDataQueryIndex = sharedIndex
                      , GlobalSystemVersion = EntityManager.GlobalSystemVersion
                      , BlackboardDataQueryType = GetArchetypeChunkSharedComponentType<BlackboardDataQuery>()
                      , BlackboardType = GetArchetypeChunkComponentType<JobBlackboard>()
                      , NodeBlobRefType = GetArchetypeChunkComponentType<NodeBlobRef>()
                      , EntityType = GetArchetypeChunkEntityType()
                    };
                    
                    jobHandler = JobHandle.CombineDependencies(job.Schedule(jobQuery, inputDeps), jobHandler);
                }
                return jobHandler;
            }
        }
    }
}
