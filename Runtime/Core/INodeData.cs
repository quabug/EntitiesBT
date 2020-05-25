using JetBrains.Annotations;

namespace EntitiesBT.Core
{
    public interface INodeData
    {
        NodeState Tick(int index, [NotNull] INodeBlob blob, [NotNull] IBlackboard bb);
        void Reset(int index, [NotNull] INodeBlob blob, [NotNull] IBlackboard bb);
    }
}
