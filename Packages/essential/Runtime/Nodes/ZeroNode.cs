using System;
using System.Runtime.InteropServices;
using EntitiesBT.Core;

namespace EntitiesBT.Nodes
{
    [BehaviorNode("08A91977-0B32-455D-874E-E29B4F1A0FD1"), StructLayout(LayoutKind.Explicit)]
    public struct ZeroNode : INodeData, ICustomResetAction
    {
        public NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            throw new NotImplementedException();
        }

        public void Reset<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            throw new NotImplementedException();
        }
    }
}
