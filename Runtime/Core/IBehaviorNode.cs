namespace EntitiesBT.Core
{
    public interface IBehaviorNode
    {
        void Initialize(VirtualMachine vm, int index);
        void Reset(VirtualMachine vm, int index);
        NodeState Tick(VirtualMachine vm, int index);
    }
}
