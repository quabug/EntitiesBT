using EntitiesBT.Core;
using EntitiesBT.Nodes;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace EntitiesBT.Components.DebugView
{
    public interface IBTDebugView
    {
        void Tick(INodeBlob blob, IBlackboard bb, int index);
    }

    public class BTDebugView : MonoBehaviour, IBTDebugView
    {
        public virtual void Tick(INodeBlob blob, IBlackboard bb, int index) {}
    }

    [BehaviorTreeDebugViewGeneric]
    public class BTDebugView<T, U> : BTDebugView
        where U : struct, INodeData
    {
        [SerializeField] private U _data;

        public override void Tick(INodeBlob blob, IBlackboard bb, int index)
        {
            var dataSize = blob.GetNodeDataSize(index);
            var typeSize = UnsafeUtility.SizeOf<U>();
            if (dataSize != typeSize)
            {
                Debug.LogWarning($"Data size not match: data-{index}({dataSize}) != {typeof(T).Name}({typeSize})");
                return;
            }
            _data = blob.GetNodeData<U>(index);
        }
    }
    
    public class BTDebugTimer : BTDebugView<TimerNode, TimerNode.Data> {}
    public class BTDebugDelayTimer : BTDebugView<DelayTimerNode, DelayTimerNode.Data> {}
    public class BTDebugRepeatForever : BTDebugView<RepeatForeverNode, RepeatForeverNode.Data> {}
    public class BTDebugRepeatTimes : BTDebugView<RepeatTimesNode, RepeatTimesNode.Data> {}
}
