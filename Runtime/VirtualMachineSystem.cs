using Unity.Entities;
using Unity.Jobs;

namespace EntitiesBT.Editor
{
    public class VirtualMachineSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((VirtualMachineComponent vm) => vm.Value.Tick());
        }
    }
}
