using EntitiesBT.Core;
using Unity.Entities;

namespace EntitiesBT
{
    public struct VirtualMachineComponent : ISharedComponentData
    {
        public VirtualMachine Value;
    }
}
