using System.Linq;
using EntitiesBT.Core;
using EntitiesBT.Entities;
using Unity.Entities;

namespace EntitiesBT.Nodes
{
    [BehaviorNode("BA0106CA-618F-409A-903A-973B89F1470A", BehaviorNodeType.Composite)]
    public class RandomSelectorNode
    {
        public static readonly ComponentType[] Types = { ComponentType.ReadWrite<BehaviorTreeRandom>() };
        
        public static NodeState Tick(int index, INodeBlob blob, IBlackboard blackboard)
        {
            if (blob.GetState(index) == NodeState.Running)
            {
                var childIndex = blob.GetChildrenIndices(index, state => state == NodeState.Running).FirstOrDefault();
                return childIndex != default ? VirtualMachine.Tick(childIndex, blob, blackboard) : 0;
            }
            
            var chosenIndex = 0;
            uint maxNumber = 0;
            ref var random = ref blackboard.GetDataRef<BehaviorTreeRandom>().Value;
            foreach (var childIndex in blob.GetChildrenIndices(index))
            {
                var rn = random.NextUInt();
                if (rn >= maxNumber)
                {
                    chosenIndex = childIndex;
                    maxNumber = rn;
                }
            }

            return VirtualMachine.Tick(chosenIndex, blob, blackboard);
        }
    }
}