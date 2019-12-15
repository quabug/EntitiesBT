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
            Reset();
        }

        public NodeState Tick()
        {
            lock (_tickLocker) return Tick(0).state;
        }

        public (NodeState state, int endIndex) Tick(int index)
        {
            return (_nodes[index].Tick(this, index), EndIndex(index));
        }

        public int EndIndex(int index)
        {
            return _root.GetEndIndex(index);
        }

        public int ChildrenCount(int parentIndex)
        {
            var endIndex = EndIndex(parentIndex);
            var childIndex = parentIndex + 1;
            var count = 0;
            for (; childIndex < endIndex; childIndex = EndIndex(childIndex)) count++;
            return count;
        }

        public ref T GetNodeData<T>(int index) where T : struct, INodeData
        {
            return ref _root.GetNodeData<T>(index);
        }

        public unsafe void* GetNodeDataPtr(int index)
        {
            return _root.GetNodeDataPtr(index);
        }

        public void Reset()
        {
            for (int index = 0; index < _nodes.Length; ++index) _nodes[index].Reset(this, index);
        }
    }
}