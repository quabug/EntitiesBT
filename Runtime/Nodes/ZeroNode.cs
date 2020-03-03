using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using EntitiesBT.Core;
using Unity.Entities;

namespace EntitiesBT.Nodes
{
    [BehaviorNode("08A91977-0B32-455D-874E-E29B4F1A0FD1"), StructLayout(LayoutKind.Explicit)]
    public struct ZeroNode : INodeData
    {
        public static IEnumerable<ComponentType> AccessTypes(int index, INodeBlob blob) => throw new NotImplementedException();
        public static NodeState Tick(int _, INodeBlob __, IBlackboard ___) =>
            throw new NotImplementedException();
    }
}
