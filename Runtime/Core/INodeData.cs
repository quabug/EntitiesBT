namespace EntitiesBT.Core
{
    public interface INodeData
    {
        NodeState Tick(int index, INodeBlob blob, IBlackboard bb);
        void Reset(int index, INodeBlob blob, IBlackboard bb);
    }
}
