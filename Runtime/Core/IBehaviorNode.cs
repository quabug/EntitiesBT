namespace EntitiesBT.Core
{
    public interface IBehaviorNode
    {
        void Reset(VirtualMachine vm, int index);
        NodeState Tick(VirtualMachine vm, int index);
    }
}
