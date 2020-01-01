using Entities;
using Unity.Entities;

namespace EntitiesBT.Entities
{
    public class MainThreadOnlyBlackboard : IComponentData
    {
        public EntityMainThreadBlackboard Value;
    }
    
    public class JobBlackboard : IComponentData
    {
        public EntityQuery EntityQuery;
        public EntityJobChunkBlackboard Blackboard;
    }
}
