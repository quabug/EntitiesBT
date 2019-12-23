using System;
using EntitiesBT.Core;
using EntitiesBT.Nodes;

namespace EntitiesBT.Editor
{
    public class BTTerminal : BTNode
    {
        public NodeState State = NodeState.Success;

        public override int Type
        {
            get
            {
                switch (State)
                {
                case NodeState.Success:
                    return Factory.GetTypeId<SuccessNode>();
                case NodeState.Failure:
                    return Factory.GetTypeId<FailureNode>();
                case NodeState.Running:
                    return Factory.GetTypeId<RunningNode>();
                default:
                    throw new ArgumentOutOfRangeException();
                }
            }
        }

        public override unsafe void Build(void* dataPtr) {}
        public override int Size => 0;
    }
}
