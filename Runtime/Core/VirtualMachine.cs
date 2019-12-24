using System.Collections.Generic;
using System.Linq;

namespace EntitiesBT.Core
{
    public class VirtualMachine
    {
        private readonly object _tickLocker = new object();
        private readonly IBehaviorNode[] _nodes;
        private readonly INodeBlob _root;

        public VirtualMachine(INodeBlob root, IBehaviorNodeFactory nodeFactory)
        {
            _root = root;
            _nodes = Enumerable.Range(0, _root.Count)
                .Select(_root.GetNodeType)
                .Select(nodeFactory.Create)
                .ToArray();
            
            for (var index = 0; index < _nodes.Length; ++index)
                _nodes[index].Initialize(this, index);
            ResetAll();
        }

        public NodeState Tick()
        {
            lock (_tickLocker) return Tick(0);
        }

        public NodeState Tick(int index)
        {
            return _nodes[index].Tick(this, index);
        }

        public int EndIndex(int index)
        {
            return _root.GetEndIndex(index);
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

        public ref T GetNodeData<T>(int index) where T : struct, INodeData
        {
            return ref _root.GetNodeData<T>(index);
        }

        public unsafe void* GetNodeDataPtr(int index)
        {
            return _root.GetNodeDataPtr(index);
        }

        public void Reset(int index)
        {
            _nodes[index].Reset(this, index);
        }

        public void ResetAll()
        {
            for (var index = 0; index < _nodes.Length; ++index) Reset(index);
        }
    }
}