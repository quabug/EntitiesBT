using EntitiesBT.Entities;
using Unity.Entities;

namespace EntitiesBT.Components.Odin
{
    public class OdinBehaviorTree : IBehaviorTreeSource
    {
        public OdinNode Root = default;
        public bool AutoDestroy = false;

        public BlobAssetReference<NodeBlob> GetBlobAsset()
        {
            var isPrefab = Root.gameObject.IsPrefab();
            if (!isPrefab && !Root.GetComponent<StopConvertToEntity>())
                Root.gameObject.AddComponent<StopConvertToEntity>();
            var blob = Root.ToBlob();
            if (!isPrefab && AutoDestroy) UnityEngine.Object.Destroy(Root.gameObject);
            return blob;
        }
    }
}