using System;
using EntitiesBT.Components;
using EntitiesBT.Entities;
using Nuwa;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Assertions;

namespace EntitiesBT
{
    [Serializable]
    public class BehaviorTreeSourceGraphPrefab : IBehaviorTreeSource
    {
        public GameObject Root = default;
        [SerializeField, ChildOf(ParentName = nameof(Root))] private BehaviorNodeComponent _behaviorTree;

        public BlobAssetReference<NodeBlob> GetBlobAsset()
        {
            Assert.IsTrue(Root.IsPrefab());
            Assert.IsNotNull(_behaviorTree);
            return _behaviorTree.NodeBuilder.ToBlob(_behaviorTree.FindGlobalValuesList());
        }
    }
}