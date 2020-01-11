using System.Linq;
using EntitiesBT.Core;

namespace EntitiesBT.Nodes
{
    [BehaviorNode("8A3B18AE-C5E9-4F34-BCB7-BD645C5017A5", BehaviorNodeType.Composite)]
    public class SequenceNode
    {
        public static NodeState Tick(int index, INodeBlob blob, IBlackboard blackboard)
        {
            return blob.TickChildren(index, blackboard, breakCheck: state => state.IsRunningOrFailure()).LastOrDefault();
        }
    }
}