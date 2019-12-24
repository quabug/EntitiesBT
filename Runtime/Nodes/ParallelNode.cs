using System;
using System.Linq;
using EntitiesBT.Core;

namespace EntitiesBT.Nodes
{
    public class ParallelNode : IBehaviorNode
    {
        private NodeState[] _childrenStates;

        public void Initialize(VirtualMachine vm, int index)
        {
            _childrenStates = new NodeState[vm.ChildrenCount(index)];
        }

        public void Reset(VirtualMachine vm, int index)
        {
            for (var i = 0; i < _childrenStates.Length; i++) _childrenStates[i] = NodeState.Running;
        }

        public NodeState Tick(VirtualMachine vm, int index)
        {
            if (_childrenStates.All(s => s != NodeState.Running))
                throw new IndexOutOfRangeException();
            
            var state = NodeState.Success;
            
            var localChildIndex = 0;
            var childIndex = index + 1;
            while (childIndex < vm.EndIndex(index))
            {
                var childState = _childrenStates[localChildIndex];
                if (childState == NodeState.Running)
                {
                    childState = vm.Tick(childIndex);
                    _childrenStates[localChildIndex] = childState;
                }

                childIndex = vm.EndIndex(childIndex);
                localChildIndex++;
                
                if (state == NodeState.Running) continue;
                if (childState != NodeState.Success) state = childState;
            }
            return state;
        }
    }
}