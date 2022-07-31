using EntitiesBT.Core;
using Unity.Entities;

namespace EntitiesBT.Components
{
    public interface IBehaviorTreeSource
    {
        // TODO: `ValueTask` would be a better choice once available.
        BlobAssetReference<NodeBlob> GetBlobAsset();
    }
}