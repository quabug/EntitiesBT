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

    [Serializable]
    public class BehaviorTreeSourceTextAsset : IBehaviorTreeSource
    {
        [SerializeField] private TextAsset _file = default;

        public BlobAssetReference<NodeBlob> GetBlobAsset()
        {
            return _file.ToBlob();
        }
    }

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
            var blob = Root.ToBlob();
            if (!isPrefab && AutoDestroy) Object.Destroy(Root.gameObject);
            return blob;
        }
    }
}