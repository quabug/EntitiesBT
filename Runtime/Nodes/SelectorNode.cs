using System.Linq;
using EntitiesBT.Core;

namespace EntitiesBT.Nodes
{
    [BehaviorNode("BD4C1D8F-BA8E-4D74-9039-7D1E6010B058", BehaviorNodeType.Composite)]
    public class SelectorNode
    {
        public static NodeState Tick(int index, INodeBlob blob, IBlackboard blackboard)
        {
            return blob.TickChildren(index, blackboard, breakCheck: state => state.IsRunningOrSuccess()).LastOrDefault();
        }
    }
}