using System;
using System.Collections.Generic;
using System.Linq;
using Entities;
using EntitiesBT.Core;
using EntitiesBT.Entities;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace EntitiesBT.Components
{
    public enum BehaviorTreeThread
    {
        ForceRunOnMainThread
      , ForceRunOnJob
      , ControlledByBehaviorTree
    }
    
    public static class BehaviorTreeExtensions
    {
        public static BlobAssetReference<NodeBlob> ToBlob(this BTNode root, Allocator allocator = Allocator.Persistent)
        {
            return root.Flatten(Utilities.Children).ToArray().ToBlob(allocator);
        }

        public static BlobAssetReference<NodeBlob> ToBlob(this TextAsset file)
        {
            var result = BlobAssetReference<NodeBlob>.TryRead(file.bytes, NodeBlob.VERSION, out var blobRef);
            if (!result) throw new FormatException("Version is not match.");
            return blobRef;
        }

        public static ISet<ComponentType> GetAccessTypes(this BlobAssetReference<NodeBlob> blob)
        {
            return new HashSet<ComponentType>(Enumerable
                .Range(0, blob.Value.Types.Length)
                .Select(i => blob.Value.Types[i])
                .SelectMany(VirtualMachine.GetAccessTypes)
            );
        }

        public static void AddBehaviorTree(
            this Entity entity
          , EntityManager dstManager
          , BlobAssetReference<NodeBlob> blobRef
          , BehaviorTreeThread thread = BehaviorTreeThread.ForceRunOnMainThread
        )
        {
            var blob = new NodeBlobRef { BlobRef = blobRef };
            
            switch (thread)
            {
            case BehaviorTreeThread.ForceRunOnMainThread:
            {
                var bb = new EntityBlackboard { EntityManager = dstManager, Entity = entity };
                VirtualMachine.Reset(blob, bb);
                dstManager.AddComponentData(entity, new ForceRunOnMainThreadTag());
                break;
            }
            case BehaviorTreeThread.ForceRunOnJob:
            {
                AddJobComponents();
                dstManager.AddComponentData(entity, new ForceRunOnJobTag());
                break;
            }
            case BehaviorTreeThread.ControlledByBehaviorTree:
            {
                AddJobComponents();
                break;
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(thread), thread, null);
            }
            
            dstManager.AddComponentData(entity, blob);
            dstManager.AddComponentData(entity, new BehaviorTreeTickDeltaTime());

            void AddJobComponents()
            {
                var dataQuery = new BlackboardDataQuery {Value = blobRef.GetAccessTypes()};
                var bb = new EntityBlackboard();
                VirtualMachine.Reset(blob, bb);
                dstManager.AddComponentData(entity, new IsRunOnMainThread { Value = false });
                dstManager.AddSharedComponentData(entity, dataQuery);
            }
        }
    }
    
    public static class UnityExtensions
    {
        public static GameObject FindOrCreateGameObject(this string name)
        {
            var obj = GameObject.Find(name);
            return obj ? obj : new GameObject(name);
        }
    }
}
