using EntitiesBT.Core;

namespace EntitiesBT.Nodes
{
    [BehaviorNode("3F494113-5404-49D6-ABCC-8BB285B730F8", BehaviorNodeType.Decorate)]
    public class ResetChildrenNode
    {
        public static NodeState Tick(int index, INodeBlob blob, IBlackboard blackboard)
        {
            foreach (var childIndex in blob.GetChildrenIndices(index))
                VirtualMachine.Reset(childIndex, blob, blackboard);
            return NodeState.Success;
        }
    }
}