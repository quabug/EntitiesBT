using System.Collections.Generic;
using EntitiesBT.Core;
using Unity.Entities;

namespace EntitiesBT
{
    public class VirtualMachineSystem : ComponentSystem
    {
        private readonly Dictionary<Entity, VirtualMachine> _virtualMachines = new Dictionary<Entity, VirtualMachine>();
        
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, BehaviorNodeFactoryComponent factory, ref NodeBlobRef blob) =>
            {
                _virtualMachines.TryGetValue(entity, out var vm);
                if (vm == null)
                {
                    vm = new VirtualMachine(blob, factory.Value);
                    _virtualMachines[entity] = vm;
                }
                vm.Tick();
            });
        }
    }
}
