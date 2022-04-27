using System;
using EntitiesBT.Entities;
using Unity.Entities;

namespace EntitiesBT.Components
{
    [Serializable]
    public class BehaviorTreeSourceGameObject : IBehaviorTreeSource
    {
        public BTNode Root = default;
        public bool AutoDestroy = false;
        public BlobAssetReference<NodeBlob> GetBlobAsset()
        {
            var isPrefab = Root.gameObject.IsPrefab();
            if (!isPrefab && !Root.GetComponent<StopConvertToEntity>())
                Root.gameObject.AddComponent<StopConvertToEntity>();
            var blob = Root.Node.ToBlob(Root.FindGlobalValuesList());
            if (!isPrefab && AutoDestroy) UnityEngine.Object.Destroy(Root.gameObject);
            return blob;
        }
    }
}
