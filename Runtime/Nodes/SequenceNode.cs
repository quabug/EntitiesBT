using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using EntitiesBT.Core;
using Unity.Entities;

namespace EntitiesBT.Nodes
{
    [StructLayout(LayoutKind.Explicit)]
    [BehaviorNode("8A3B18AE-C5E9-4F34-BCB7-BD645C5017A5", BehaviorNodeType.Composite)]
    public struct SequenceNode : INodeData
    {
        public IEnumerable<ComponentType> AccessTypes(int index, INodeBlob blob) => Enumerable.Empty<ComponentType>();
        
        public NodeState Tick(int index, INodeBlob blob, IBlackboard blackboard)
        {
            return blob.TickChildren(index, blackboard, breakCheck: state => state.IsRunningOrFailure()).LastOrDefault();
        }

        public void Reset(int index, INodeBlob blob, IBlackboard blackboard)
        {
        }
    }
}