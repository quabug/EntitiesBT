using EntitiesBT.Core;
using Unity.Entities;

namespace EntitiesBT.Entities
{
    public class BlackboardComponent : IComponentData
    {
        public IBlackboard Value;
    }
}
