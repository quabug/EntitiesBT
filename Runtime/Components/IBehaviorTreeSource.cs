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
        Task<BlobAssetReference<NodeBlob>> GetBlobAsset();
    }

    [Serializable]
    public class BehaviorTreeSourceTextAsset : IBehaviorTreeSource
    {
        [SerializeField] private TextAsset _file = default;
        
        public Task<BlobAssetReference<NodeBlob>> GetBlobAsset()
        {
            return Task.FromResult(_file.ToBlob());
        }
    }

    [Serializable]
    public class BehaviorTreeSourceGameObject : IBehaviorTreeSource
    {
        public BTNode Root = default;
        public bool AutoDestroy = false;
        public Task<BlobAssetReference<NodeBlob>> GetBlobAsset()
        {
            var isPrefab = Root.gameObject.IsPrefab();
            if (!isPrefab && !Root.GetComponent<StopConvertToEntity>())
                Root.gameObject.AddComponent<StopConvertToEntity>();
            var task = Task.FromResult(Root.ToBlob());
            if (!isPrefab && AutoDestroy) Object.Destroy(Root.gameObject);
            return task;
        }
    }
}