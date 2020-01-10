using System;
using EntitiesBT.Core;

namespace EntitiesBT.Nodes
{
    public static class SuccessionNode
    {
        [Serializable]
        public struct Data : INodeData
        {
            public int ChildIndex;
        }

        public static void Reset(int index, INodeBlob blob, IBlackboard blackboard)
        {
            blob.GetNodeData<Data>(index).ChildIndex = index + 1;
        }

        public static Func<int, INodeBlob, IBlackboard, NodeState> Tick(NodeState continueState)
        {
            return (index, blob, bb) =>
            {
                ref var childIndex = ref blob.GetNodeData<Data>(index).ChildIndex;
                var endIndex = blob.GetEndIndex(index);
                if (childIndex >= endIndex) throw new IndexOutOfRangeException();

                while (childIndex < endIndex)
                {
                    var childState = VirtualMachine.Tick(childIndex, blob, bb);

                    if (childState == NodeState.Running) return childState;

                    if (childState != continueState)
                    {
                        childIndex = endIndex;
                        return childState;
                    }

                    childIndex = blob.GetEndIndex(childIndex);
                }
                return continueState;
            };
        }
    }
}