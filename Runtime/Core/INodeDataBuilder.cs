using System.Collections.Generic;
using Unity.Entities;

namespace EntitiesBT.Core
{
    public interface INodeDataBuilder
    {
        BehaviorNodeType BehaviorNodeType { get; }
        int NodeId { get; }
        BlobAssetReference Build(ITreeNode<INodeDataBuilder>[] builders);
        INodeDataBuilder Self { get; }
        IEnumerable<INodeDataBuilder> Children { get; }
    }
}
