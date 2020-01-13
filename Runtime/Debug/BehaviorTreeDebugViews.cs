using System;
using System.Linq;
using EntitiesBT.Core;
using EntitiesBT.Entities;
using EntitiesBT.Nodes;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using UnityEngine;

namespace EntitiesBT.DebugView
{
    public class BTDebugView : MonoBehaviour
    {
        public bool IsValid => EntityManager != null && Blackboard != null && Blob.BlobRef.IsCreated;
        
        [NonSerialized] public EntityManager EntityManager;
        [NonSerialized] public Entity Entity;
        [NonSerialized] public EntityBlackboard Blackboard;
        [NonSerialized] public NodeBlobRef Blob;
        [NonSerialized] public int Index;

        // TODO: not implemented yet
        [NonSerialized] public bool PauseOnTick = false;
        
        public virtual void Init() {}
        public virtual void Tick() {}
    }
    
    [BehaviorTreeDebugViewGeneric]
    public class BTDebugView<T> : BTDebugView {}

    [BehaviorTreeDebugViewGeneric]
    public class BTDebugView<T, U> : BTDebugView
        where U : struct, INodeData
    {
        public U DefaultData;
        public U RuntimeData;

        public override void Init()
        {
            RuntimeData = Blob.GetNodeData<U>(Index);
            DefaultData = Blob.GetNodeDefaultData<U>(Index);
        }

        public override void Tick()
        {
            var dataSize = Blob.GetNodeDataSize(Index);
            var typeSize = UnsafeUtility.SizeOf<U>();
            if (dataSize != typeSize)
            {
                Debug.LogWarning($"Data size not match: data-{Index}({dataSize}) != {typeof(T).Name}({typeSize})");
                return;
            }
            RuntimeData = Blob.GetNodeData<U>(Index);
        }

        protected virtual void OnValidate()
        {
            if (!IsValid) return;
            Blob.GetNodeData<U>(Index) = RuntimeData;
            Blob.GetNodeDefaultData<U>(Index) = DefaultData;
            Debug.Log("node data changed");
        }
    }
    
    public class BTDebugTimer : BTDebugView<TimerNode, TimerNode.Data> {}
    public class BTDebugDelayTimer : BTDebugView<DelayTimerNode, DelayTimerNode.Data> {}
    public class BTDebugRepeatForever : BTDebugView<RepeatForeverNode, RepeatForeverNode.Data> {}
    public class BTDebugRepeatTimes : BTDebugView<RepeatTimesNode, RepeatTimesNode.Data> {}

    public class BTDebugWeightRandomSelector : BTDebugView<WeightRandomSelectorNode>
    {
        public float[] DefaultWeights;
        public float[] RuntimeWeights;

        public override void Init()
        {
            ref var runtime = ref Blob.GetNodeData<WeightRandomSelectorNode.Data>(Index).Weights;
            RuntimeWeights = runtime.ToArray();
            
            ref var @default = ref Blob.GetNodeDefaultData<WeightRandomSelectorNode.Data>(Index).Weights;
            DefaultWeights = @default.ToArray();
        }

        public override void Tick()
        {
            ref var runtime = ref Blob.GetNodeData<WeightRandomSelectorNode.Data>(Index).Weights;
            RuntimeWeights = runtime.ToArray();
        }

        protected virtual void OnValidate()
        {
            if (!IsValid) return;
            
            ref var @default = ref Blob.GetNodeDefaultData<WeightRandomSelectorNode.Data>(Index);
            SetData(ref @default, DefaultWeights);
            
            ref var runtime = ref Blob.GetNodeData<WeightRandomSelectorNode.Data>(Index);
            SetData(ref runtime, RuntimeWeights);

            void SetData(ref WeightRandomSelectorNode.Data data, float[] array)
            {
                Array.Resize(ref array, data.Weights.Length);
                for (var i = 0; i < array.Length; i++) if (array[i] < 0) array[i] = 0;
                data.Sum = array.Sum();
                data.Weights.FromArrayUnsafe(array);
            }
        }
    }
}
