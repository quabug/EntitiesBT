using System;
using System.Runtime.InteropServices;
using EntitiesBT.Core;

namespace EntitiesBT.Nodes
{
    [BehaviorNode("08A91977-0B32-455D-874E-E29B4F1A0FD1"), StructLayout(LayoutKind.Explicit)]
    public struct ZeroNode : INodeData
    {
        public static NodeState Tick(int _, INodeBlob __, IBlackboard ___) =>
            throw new NotImplementedException();
    }
}
