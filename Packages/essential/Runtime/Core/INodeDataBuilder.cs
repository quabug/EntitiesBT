using System.Collections.Generic;
using JetBrains.Annotations;

namespace EntitiesBT.Core
{
    public interface INodeDataBuilder
    {
        int NodeId { get; }
        BlobAssetReference Build([NotNull] ITreeNode<INodeDataBuilder>[] builders);
        INodeDataBuilder Self { get; }
        IEnumerable<INodeDataBuilder> Children { get; }
    }
}
