namespace EntitiesBT.Core
{
    public interface IBehaviorNode
    {
        void Reset(VirtualMachine vm, int index, IBlackboard blackboard);
        NodeState Tick(VirtualMachine vm, int index, IBlackboard blackboard);
    }
}
