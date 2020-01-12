using Unity.Entities;

namespace EntitiesBT.Entities
{
    [BehaviorTreeComponent]
    public struct BehaviorTreeTickDeltaTime : IComponentData
    {
        public float Value;
    }
    
    [UpdateBefore(typeof(VirtualMachineSystem))]
    public class DeltaTimeSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((ref BehaviorTreeTickDeltaTime deltaTime) => deltaTime.Value = Time.DeltaTime);
        }
    }
}
