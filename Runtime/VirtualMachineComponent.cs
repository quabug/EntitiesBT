using System;
using EntitiesBT.Core;
using Unity.Entities;

namespace EntitiesBT
{
    [Serializable]
    public struct VirtualMachineComponent : ISharedComponentData, IEquatable<VirtualMachineComponent>
    {
        public VirtualMachine Value;
        public VirtualMachineComponent(VirtualMachine value) => Value = value;

        public bool Equals(VirtualMachineComponent other)
        {
            return Equals(Value, other.Value);
        }

        public override bool Equals(object obj)
        {
            return obj is VirtualMachineComponent other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (Value != null ? Value.GetHashCode() : 0);
        }
    }
}
