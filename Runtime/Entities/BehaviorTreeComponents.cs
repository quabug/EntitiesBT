using System;
using Unity.Entities;
using Random = Unity.Mathematics.Random;

namespace EntitiesBT.Entities
{
#region TickDeltaTime
    [BehaviorTreeComponent]
    public struct BehaviorTreeTickDeltaTime : IComponentData
    {
        public float Value;
    }
    
    [UpdateBefore(typeof(VirtualMachineSystem))]
    public class BehaviorTreeDeltaTimeSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((ref BehaviorTreeTickDeltaTime deltaTime) => deltaTime.Value = Time.DeltaTime);
        }
    }
#endregion

#region Random
    [BehaviorTreeComponent]
    public struct BehaviorTreeRandom : IComponentData
    {
        public Random Value;
    }
    
    [UpdateBefore(typeof(VirtualMachineSystem))]
    public class BehaviorTreeInitRandomSystem : ComponentSystem
    {
        private struct ExistTag : ISystemStateComponentData {}
        
        protected override void OnUpdate()
        {
            Entities.WithNone<ExistTag, BehaviorTreeRandomSeed>()
                .ForEach((Entity entity, ref BehaviorTreeRandom random) => {
                    random.Value.InitState((uint)Environment.TickCount);
                    EntityManager.AddComponent<ExistTag>(entity);
                });
            
            Entities.WithNone<ExistTag>()
                .ForEach((Entity entity, ref BehaviorTreeRandom random, ref BehaviorTreeRandomSeed seed) => {
                    random.Value.InitState(seed.Value);
                    EntityManager.AddComponent<ExistTag>(entity);
                });
            
            Entities.WithNone<BehaviorTreeRandom>().WithAll<ExistTag>().ForEach(entity =>
                EntityManager.RemoveComponent<ExistTag>(entity)
            );
        }
    }
#endregion
}
