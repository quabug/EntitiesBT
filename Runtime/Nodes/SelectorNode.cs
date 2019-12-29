using System;
using EntitiesBT.Core;

namespace EntitiesBT.Nodes
{
    [BehaviorNode("BD4C1D8F-BA8E-4D74-9039-7D1E6010B058", BehaviorNodeType.Composite)]
    public class SelectorNode
    {
        private static readonly Func<int, INodeBlob, IBlackboard, NodeState> _tickFunc = SuccessionNode.Tick(NodeState.Failure);

        public static void Reset(int index, INodeBlob blob, IBlackboard blackboard)
        {
            SuccessionNode.Reset(index, blob, blackboard);
        }

        public static NodeState Tick(int index, INodeBlob blob, IBlackboard blackboard)
        {
            return _tickFunc(index, blob, blackboard);
        }
    }
}