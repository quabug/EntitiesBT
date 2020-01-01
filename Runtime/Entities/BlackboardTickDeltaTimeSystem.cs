using System;
using EntitiesBT.Entities;
using Unity.Entities;

namespace EntitiesBT
{
    [UpdateBefore(typeof(VirtualMachineMainThreadOnlySystem))]
    public class BlackboardTickDeltaTimeSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((MainThreadOnlyBlackboard bb, ref TickDeltaTime deltaTime) =>
                deltaTime.Value = TimeSpan.FromSeconds(Time.DeltaTime)
            );
        }
    }
}
