using EntitiesBT.Entities;
using Unity.Entities;
using Unity.Mathematics;

namespace EntitiesBT.Samples
{
    [BehaviorTreeComponent]
    public struct BTInputMoveComponent : IComponentData
    {
        public float2 Value;
    }
    
    [BehaviorTreeComponent]
    public struct BTCharacterVelocityComponent : IComponentData
    {
        public float3 Value;
    }
}
