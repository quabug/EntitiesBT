using EntitiesBT.Core;
using EntitiesBT.Nodes;
using UnityEngine;

namespace EntitiesBT.Components
{
    public class BTRepeat : BTNode
    {
        [Header("ForeverRepeat = RepeatTimes <= 0")]
        public int RepeatTimes;
        public NodeState BreakStates;

        public override BehaviorNodeType NodeType => BehaviorNodeType.Decorate;

        public override int NodeId => RepeatTimes <= 0
            ? typeof(RepeatForeverNode).GetBehaviorNodeAttribute().Id
            : typeof(RepeatTimesNode).GetBehaviorNodeAttribute().Id
        ;

        public override unsafe int Size => RepeatTimes <= 0
            ? sizeof(RepeatForeverNode)
            : sizeof(RepeatTimesNode)
        ;
        
        public override unsafe void Build(void* dataPtr, ITreeNode<INodeDataBuilder>[] builders)
        {
            if (RepeatTimes <= 0)
            {
                var ptr = (RepeatForeverNode*) dataPtr;
                ptr->BreakStates = BreakStates;
            }
            else
            {
                var ptr = (RepeatTimesNode*) dataPtr;
                ptr->TickTimes = RepeatTimes;
                ptr->BreakStates = BreakStates;
            }
        }
    }
}
