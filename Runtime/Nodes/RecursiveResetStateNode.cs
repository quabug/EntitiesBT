using System.Runtime.InteropServices;
using EntitiesBT.Core;

namespace EntitiesBT.Nodes
{
    [StructLayout(LayoutKind.Explicit)]
    [BehaviorNode("7656C8CB-EBC9-4C82-A374-511D4CB4D7FA", BehaviorNodeType.Decorate)]
    public struct RecursiveResetStateNode : INodeData
    {
        public NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            var endIndex = blob.GetEndIndex(index);
            var childIndex = index + 1;
            for (var i = childIndex + 1 /* always reset directly child */; i < endIndex; i++)
            {
                if (blob.GetState(i) == NodeState.Running)
                {
                    endIndex = i;
                    break;
                }
            }
            var count = endIndex - childIndex;
            // count will be 0 if there's no child
            blob.ResetStates(childIndex, count);
            return index.TickChildrenReturnFirstOrDefault(ref blob, ref bb);
        }

        public void Reset<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard blackboard)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
        }
    }
}
