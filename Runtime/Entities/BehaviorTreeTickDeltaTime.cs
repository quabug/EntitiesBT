using Unity.Entities;

namespace EntitiesBT.Entities
{
    [BehaviorTreeComponent]
    public struct BehaviorTreeTickDeltaTime : IComponentData
    {
        public float Value;
    }
    
    [UpdateBefore(typeof(VirtualMachineSystem))]
    public class BehaviorTreeDeltaTimeSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var dt = Time.DeltaTime;
            Entities.ForEach((ref BehaviorTreeTickDeltaTime deltaTime) => deltaTime.Value = dt).ScheduleParallel();
        }
    }
}
