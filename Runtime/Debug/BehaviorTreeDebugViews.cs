using System;
using EntitiesBT.Core;
using EntitiesBT.Entities;
using EntitiesBT.Nodes;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using UnityEngine;

namespace EntitiesBT.DebugView
{
    public interface IBTDebugView
    {
        void Init();
        void Tick();
    }

    public class BTDebugView : MonoBehaviour, IBTDebugView
    {
        public bool IsValid => EntityManager != null && Blackboard != null && Blob.BlobRef.IsCreated;
        
        [NonSerialized] public EntityManager EntityManager;
        [NonSerialized] public Entity Entity;
        [NonSerialized] public EntityBlackboard Blackboard;
        [NonSerialized] public NodeBlobRef Blob;
        [NonSerialized] public int Index;
        
        public virtual void Init() {}
        public virtual void Tick() {}
    }

    [BehaviorTreeDebugViewGeneric]
    public class BTDebugView<T, U> : BTDebugView
        where U : struct, INodeData
    {
        public U Data;

        public override void Tick()
        {
            var dataSize = Blob.GetNodeDataSize(Index);
            var typeSize = UnsafeUtility.SizeOf<U>();
            if (dataSize != typeSize)
            {
                Debug.LogWarning($"Data size not match: data-{Index}({dataSize}) != {typeof(T).Name}({typeSize})");
                return;
            }
            Data = Blob.GetNodeData<U>(Index);
        }
    }
    
    public class BTDebugTimer : BTDebugView<TimerNode, TimerNode.Data> {}
    public class BTDebugDelayTimer : BTDebugView<DelayTimerNode, DelayTimerNode.Data> {}
    public class BTDebugRepeatForever : BTDebugView<RepeatForeverNode, RepeatForeverNode.Data> {}
    public class BTDebugRepeatTimes : BTDebugView<RepeatTimesNode, RepeatTimesNode.Data> {}
}
