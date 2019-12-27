namespace EntitiesBT.Core
{
    public interface IBehaviorNode
    {
        void Reset(int index, INodeBlob blob, IBlackboard bb);
        NodeState Tick(int index, INodeBlob blob, IBlackboard bb);
    }
}
