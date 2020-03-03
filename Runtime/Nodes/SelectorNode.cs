using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using EntitiesBT.Core;
using Unity.Entities;

namespace EntitiesBT.Nodes
{
    [StructLayout(LayoutKind.Explicit)]
    [BehaviorNode("BD4C1D8F-BA8E-4D74-9039-7D1E6010B058", BehaviorNodeType.Composite)]
    public struct SelectorNode : INodeData
    {
        public static IEnumerable<ComponentType> AccessTypes(int index, INodeBlob blob) => Enumerable.Empty<ComponentType>();
        
        public static NodeState Tick(int index, INodeBlob blob, IBlackboard blackboard)
        {
            return blob.TickChildren(index, blackboard, breakCheck: state => state.IsRunningOrSuccess()).LastOrDefault();
        }
    }
}