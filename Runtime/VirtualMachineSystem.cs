using System;
using Unity.Entities;

namespace EntitiesBT
{
    public class VirtualMachineSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, BlackboardComponent bb, ref NodeBlobRef blob) =>
            {
                var vm = EntityManager.GetSharedComponentData<VirtualMachineComponent>(entity).Value;
                var deltaTime = TimeSpan.FromSeconds(Time.DeltaTime);
                bb.Value[typeof(TickDeltaTime)] = new TickDeltaTime(deltaTime);
                vm.Tick(blob, bb.Value);
            });
        }
    }
}
