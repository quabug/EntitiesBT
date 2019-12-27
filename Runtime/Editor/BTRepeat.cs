using EntitiesBT.Core;
using EntitiesBT.Nodes;

namespace EntitiesBT.Editor
{
    public class BTRepeat : BTNode<ZeroNodeData>
    {
        public int RepeatTimes;
        public NodeState BreakStates;
        
        public override int NodeId => RepeatTimes <= 0
            ? RepeatForeverNode.Id
            : RepeatTimesNode.Id
        ;

        public override unsafe int Size => RepeatTimes <= 0
            ? sizeof(RepeatForeverNode.Data)
            : sizeof(RepeatTimesNode.Data)
        ;
        
        public override unsafe void Build(void* dataPtr)
        {
            if (RepeatTimes <= 0)
            {
                var ptr = (RepeatForeverNode.Data*) dataPtr;
                ptr->BreakStates = BreakStates;
            }
            else
            {
                var ptr = (RepeatTimesNode.Data*) dataPtr;
                ptr->TargetTimes = RepeatTimes;
                ptr->BreakStates = BreakStates;
            }
        }
    }
}
