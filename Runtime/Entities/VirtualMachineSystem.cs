using System;
using EntitiesBT.Core;
using Unity.Entities;

namespace EntitiesBT.Entities
{
    public class VirtualMachineSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, BlackboardComponent bb, ref NodeBlobRef blob) =>
            {
                var deltaTime = TimeSpan.FromSeconds(Time.DeltaTime);
                bb.Value[typeof(TickDeltaTime)] = new TickDeltaTime(deltaTime);
                VirtualMachine.Tick(blob, bb.Value);
            });
        }
    }
}
