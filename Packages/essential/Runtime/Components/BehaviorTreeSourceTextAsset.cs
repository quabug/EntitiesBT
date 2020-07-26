using System;
using EntitiesBT.Entities;
using Unity.Entities;
using UnityEngine;

namespace EntitiesBT.Components
{
    [Serializable]
    public class BehaviorTreeSourceTextAsset : IBehaviorTreeSource
    {
        [SerializeField] private TextAsset _file = default;

        public BlobAssetReference<NodeBlob> GetBlobAsset()
        {
            return _file.ToBlob();
        }
    }
}
