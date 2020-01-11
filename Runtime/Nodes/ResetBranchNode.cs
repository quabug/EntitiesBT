using EntitiesBT.Core;

namespace EntitiesBT.Nodes
{
    [BehaviorNode("3F494113-5404-49D6-ABCC-8BB285B730F8", BehaviorNodeType.Decorate)]
    public class ResetBranchNode
    {
        public static NodeState Tick(int index, INodeBlob blob, IBlackboard blackboard)
        {
            VirtualMachine.Reset(index, blob, blackboard, blob.GetEndIndex(index) - index);
            return NodeState.Success;
        }
    }
}