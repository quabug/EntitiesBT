using System;
using Unity.Entities;

namespace EntitiesBT
{
    public class VirtualMachineSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((VirtualMachineComponent vm) =>
            {
                var deltaTime = TimeSpan.FromSeconds(Time.DeltaTime);
                vm.Value.Blackboard[typeof(TickDeltaTime)] = new TickDeltaTime(deltaTime);
                vm.Value.Tick();
            });
        }
    }
}
