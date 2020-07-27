using System;
using System.Threading.Tasks;
using EntitiesBT.Entities;
using Unity.Entities;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EntitiesBT.Components
{
    public interface IBehaviorTreeSource
    {
        // TODO: `ValueTask` would be a better choice once available.
        BlobAssetReference<NodeBlob> GetBlobAsset();
    }
}