using System;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using EntitiesBT.Core;

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
      , All = int.MaxValue
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

        public BehaviorTreeBufferElement(int order, BehaviorTreeRuntimeThread runtimeThread, NodeBlobRef nodeBlob, EntityQueryMask queryMask, Entity behaviorTree, JobHandle dependency)
        {
            Order = order;
            RuntimeThread = runtimeThread;
            NodeBlob = nodeBlob;
            QueryMask = queryMask;
            BehaviorTree = behaviorTree;
            Dependency = dependency;
        }
    }

    public struct CurrentBehaviorTreeComponent : IComponentData
    {
        public IntPtr/* BehaviorTreeBufferElement* */ Value;
        public unsafe ref BehaviorTreeBufferElement RefValue =>
            ref UnsafeUtility.AsRef<BehaviorTreeBufferElement>(Value.ToPointer());
    }

    public readonly struct BehaviorTreeTargetComponent : IComponentData
    {
        public readonly Entity Value;

        public BehaviorTreeTargetComponent(Entity value)
        {
            Value = value;
        }
    }

    public readonly struct BehaviorTreeOrderComponent : IComponentData
    {
        public readonly int Value;

        public BehaviorTreeOrderComponent(int value)
        {
            Value = value;
        }
    }
    
    public readonly struct BehaviorTreeComponent : IComponentData
    {
        public readonly NodeBlobRef Blob;
        public readonly BehaviorTreeThread Thread;
        public readonly AutoCreateType AutoCreation;

        public BehaviorTreeComponent(NodeBlobRef blob, BehaviorTreeThread thread, AutoCreateType autoCreation)
        {
            Blob = blob;
            Thread = thread;
            AutoCreation = autoCreation;
        }
    }
}