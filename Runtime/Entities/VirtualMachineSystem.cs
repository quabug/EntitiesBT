using EntitiesBT.Core;
using Unity.Entities;

namespace EntitiesBT.Entities
{
    public class VirtualMachineSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, MainThreadOnlyBlackboard bb, ref NodeBlobRef blob) =>
                VirtualMachine.Tick(blob, bb.Value)
            );
        }
    }
}
