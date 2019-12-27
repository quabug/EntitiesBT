using EntitiesBT.Core;
using Unity.Entities;

namespace EntitiesBT
{
    public class BlackboardComponent : IComponentData
    {
        public IBlackboard Value;
    }
}
