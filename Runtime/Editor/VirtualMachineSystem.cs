using Unity.Entities;

namespace EntitiesBT.Editor
{
    public class VirtualMachineSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, BTRoot root, ref NodeBlobRef blob) =>
                root.VirtualMachine.Tick());
        }
    }
}
