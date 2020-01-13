using System.Collections.Generic;

namespace EntitiesBT.Core
{
    public interface INodeDataBuilder
    {
        BehaviorNodeType NodeType { get; }
        int NodeId { get; }
        int Size { get; }
        unsafe void Build(void* dataPtr, ITreeNode<INodeDataBuilder>[] builders);
        INodeDataBuilder Self { get; }
        IEnumerable<INodeDataBuilder> Children { get; }
    }
}
