using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EntitiesBT.Core
{
    public class VirtualMachine
    {
        private readonly object _tickLocker = new object();
        public IList<IBehaviorNode> BehaviorNodes { get; }
        public INodeBlob NodeBlob { get; }

        public VirtualMachine(INodeBlob nodeBlob, IBehaviorNodeFactory nodeFactory)
        {
            NodeBlob = nodeBlob;
            BehaviorNodes = Enumerable.Range(0, NodeBlob.Count)
                .Select(NodeBlob.GetNodeType)
                .Select(nodeFactory.Create)
                .ToArray()
            ;
            ResetAll();
        }

        public NodeState Tick()
        {
            lock (_tickLocker) return Tick(0);
        }

        public NodeState Tick(int index)
        {
            var node = BehaviorNodes[index];
            var state = node.Tick(this, index);
            Debug.Log($"[BT] Tick: {index}-{node.GetType().Name}-{state}");
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
            Debug.Log($"[BT] Reset: {index}-{node.GetType().Name}");
            node.Reset(this, index);
        }

        public void ResetAll()
        {
            for (var index = 0; index < BehaviorNodes.Count; ++index) Reset(index);
        }
    }
}