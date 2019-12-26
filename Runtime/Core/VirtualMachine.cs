using System;
using System.Collections.Generic;
using System.Linq;

namespace EntitiesBT.Core
{
    [Serializable]
    public class VirtualMachine
    {
        private readonly object _tickLocker = new object();

        public readonly IList<IBehaviorNode> BehaviorNodes;
        public readonly INodeBlob NodeBlob;
        public readonly IBlackboard Blackboard;

        public VirtualMachine(INodeBlob nodeBlob, IList<IBehaviorNode> nodes, IBlackboard blackboard)
        {
            Blackboard = blackboard;
            NodeBlob = nodeBlob;
            BehaviorNodes = nodes;
            ResetAll();
        }

        public NodeState Tick()
        {
            lock (_tickLocker) return Tick(0);
        }

        public NodeState Tick(int index)
        {
            var node = BehaviorNodes[index];
            var state = node.Tick(this, index, Blackboard);
            // Debug.Log($"[BT] tick: {index}-{node.GetType().Name}-{state}");
            return state;
        }

        public int EndIndex(int index)
        {
            return NodeBlob.GetEndIndex(index);
        }

        public int ChildrenCount(int parentIndex)
        {
            return GetChildrenIndices(parentIndex).Count();
        }

        public IEnumerable<int> GetChildrenIndices(int parentIndex)
        {
            var endIndex = EndIndex(parentIndex);
            var childIndex = parentIndex + 1;
            for (; childIndex < endIndex; childIndex = EndIndex(childIndex))
            {
                yield return childIndex;
            }
        }
        
        public IEnumerable<int> GetDescendantsIndices(int parentIndex)
        {
            var endIndex = EndIndex(parentIndex);
            var firstChildIndex = parentIndex + 1;
            return Enumerable.Range(firstChildIndex, endIndex - firstChildIndex);
        }

        public IBehaviorNode GetBehaviorNode(int index)
        {
            return BehaviorNodes[index];
        }

        public ref T GetNodeData<T>(int index) where T : struct, INodeData
        {
            return ref NodeBlob.GetNodeData<T>(index);
        }

        public unsafe void* GetNodeDataPtr(int index)
        {
            return NodeBlob.GetNodeDataPtr(index);
        }

        public void Reset(int index)
        {
            var node = BehaviorNodes[index];
            // Debug.Log($"[BT] reset: {index}-{node.GetType().Name}");
            node.Reset(this, index, Blackboard);
        }

        public void ResetAll()
        {
            for (var index = 0; index < BehaviorNodes.Count; ++index) Reset(index);
        }
    }
}