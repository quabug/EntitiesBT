using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace EntitiesBT.Entities
{
    public enum BehaviorTreeRuntimeThread
    {
        MainThread
      , ForceMainThread
      , JobThread
      , ForceJobThread
    }
    
    public enum BehaviorTreeThread
    {
        ForceRunOnMainThread
      , ForceRunOnJob
      , ControlledByBehaviorTree
    }
    
    public static class BehaviorTreeThreadExtensions
    {
        public static BehaviorTreeRuntimeThread ToRuntimeThread(this BehaviorTreeThread thread)
        {
            switch (thread)
            {
                case BehaviorTreeThread.ForceRunOnMainThread:
                    return BehaviorTreeRuntimeThread.ForceMainThread;
                case BehaviorTreeThread.ForceRunOnJob:
                    return BehaviorTreeRuntimeThread.ForceJobThread;
                case BehaviorTreeThread.ControlledByBehaviorTree:
                    return BehaviorTreeRuntimeThread.JobThread;
                default:
                    throw new ArgumentOutOfRangeException(nameof(thread), thread, null);
            }
        }
    }
    
    [Flags]
    public enum AutoCreateType
    {
        None = 0
      , BehaviorTreeComponent = 1 << 0
      , ReadOnly = 1 << 1
      , ReadWrite = 1 << 2
    }

    public static class AutoCreateTypeExtensions
    {
        public static bool HasFlagFast(this AutoCreateType flags, AutoCreateType flag)
        {
            return (flags & flag) == flag;
        }
    }
    
    [InternalBufferCapacity(8)]
    public struct BehaviorTreeBufferElement : IBufferElementData
    {
        public int Order;
        public BehaviorTreeRuntimeThread RuntimeThread;
        public NodeBlobRef NodeBlob;
        public EntityQueryMask QueryMask;
        public Entity BehaviorTree;
        public JobHandle Dependency;
    }

    public struct BehaviorTreeTargetComponent : IComponentData
    {
        public Entity Value;
    }

    public struct BehaviorTreeOrderComponent : IComponentData
    {
        public int Value;
    }
    
    public struct BehaviorTreeComponent : IComponentData
    {
        public NodeBlobRef Blob;
        public BehaviorTreeThread Thread;
        public AutoCreateType AutoCreation;
    }
}