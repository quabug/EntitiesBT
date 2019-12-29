using System;
using EntitiesBT.Core;

namespace EntitiesBT.Nodes
{
    [BehaviorNode("8A3B18AE-C5E9-4F34-BCB7-BD645C5017A5", BehaviorNodeType.Composite)]
    public class SequenceNode
    {
        private static readonly Func<int, INodeBlob, IBlackboard, NodeState> _tickFunc = SuccessionNode.Tick(NodeState.Success);

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