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
        public List<BlackboardDataQuery> _blackboardDataQueryList = new List<BlackboardDataQuery>();
        public List<int> _blackboardDataQueryIndices = new List<int>();
        
        struct TickJob : IJobChunk
        {
            public int BlackboardDataQueryIndex;
            public uint GlobalSystemVersion;
            
            [ReadOnly] public ArchetypeChunkSharedComponentType<BlackboardDataQuery> BlackboardDataQueryType;
            [ReadOnly] public ArchetypeChunkComponentType<NodeBlobRef> NodeBlobRefType;
            [ReadOnly] public ArchetypeChunkComponentType<JobBlackboard> BlackboardType;
            
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
                    bb.Value.Index = i;
                    VirtualMachine.Tick(nodeBlobs[i], bb.Value);
                }
            }
        }
        
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var dt = Time.DeltaTime;
            var deltaTimeJobHandle = Entities.WithAll<JobBlackboard>()
                .ForEach((ref TickDeltaTime deltaTime) => deltaTime.Value = dt)
                .Schedule(inputDeps)
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
                
                var job = new TickJob {
                    BlackboardDataQueryIndex = sharedIndex
                  , GlobalSystemVersion = EntityManager.GlobalSystemVersion
                  , BlackboardDataQueryType = GetArchetypeChunkSharedComponentType<BlackboardDataQuery>()
                  , BlackboardType = GetArchetypeChunkComponentType<JobBlackboard>()
                  , NodeBlobRefType = GetArchetypeChunkComponentType<NodeBlobRef>()
                };
                
                // TODO: avoid GC? use NativeArray?
                var entityQuery = GetEntityQuery(query.Value
                    .Append(ComponentType.ReadOnly<BlackboardDataQuery>())
                    .Append(ComponentType.ReadWrite<NodeBlobRef>())
                    .Append(ComponentType.ReadWrite<JobBlackboard>())
                    .ToArray()
                );
                jobHandler = JobHandle.CombineDependencies(job.Schedule(entityQuery, deltaTimeJobHandle), jobHandler);
            }
            return jobHandler;
        }
    }
}
