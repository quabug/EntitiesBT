using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using EntitiesBT.Core;
using EntitiesBT.Entities;
using Unity.Entities;

namespace EntitiesBT.Nodes
{
    [StructLayout(LayoutKind.Explicit)]
    [BehaviorNode("BA0106CA-618F-409A-903A-973B89F1470A", BehaviorNodeType.Composite)]
    public struct RandomSelectorNode : INodeData
    {
        public IEnumerable<ComponentType> AccessTypes(int index, INodeBlob blob)
        {
            yield return ComponentType.ReadWrite<BehaviorTreeRandom>();
        }
        
        public NodeState Tick(int index, INodeBlob blob, IBlackboard blackboard)
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

        public void Reset(int index, INodeBlob blob, IBlackboard blackboard)
        {
        }
    }
}