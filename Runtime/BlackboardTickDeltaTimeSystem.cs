using System;
using EntitiesBT.Core;
using EntitiesBT.Entities;
using Unity.Entities;

namespace EntitiesBT
{
    [UpdateBefore(typeof(VirtualMachineSystem))]
    public class BlackboardTickDeltaTimeSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((BlackboardComponent bb) =>
            {
                var deltaTime = TimeSpan.FromSeconds(Time.DeltaTime);
                bb.Value.SetData(new TickDeltaTime(deltaTime));
            });
        }
    }
}
