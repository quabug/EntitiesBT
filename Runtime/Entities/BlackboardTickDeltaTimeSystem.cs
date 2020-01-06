using System;
using EntitiesBT.Entities;
using Unity.Entities;

namespace EntitiesBT
{
    [UpdateBefore(typeof(VirtualMachineSystem))]
    public class BlackboardTickDeltaTimeSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((MainThreadOnlyBlackboard bb, ref TickDeltaTime deltaTime) => 
                deltaTime.Value = Time.DeltaTime
            );
        }
    }
}
