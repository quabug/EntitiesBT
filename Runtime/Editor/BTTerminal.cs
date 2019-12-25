using System;
using EntitiesBT.Core;
using EntitiesBT.Nodes;

namespace EntitiesBT.Editor
{
    public class BTTerminal : BTNode
    {
        public NodeState State = NodeState.Success;

        public override IBehaviorNode BehaviorNode
        {
            get
            {
                switch (State)
                {
                case NodeState.Success:
                    return new SuccessNode();
                case NodeState.Failure:
                    return new FailureNode();
                case NodeState.Running:
                    return new RunningNode();
                default:
                    throw new ArgumentOutOfRangeException();
                }
            }
        }

        public override unsafe void Build(void* dataPtr) {}
        public override int Size => 0;
    }
}
