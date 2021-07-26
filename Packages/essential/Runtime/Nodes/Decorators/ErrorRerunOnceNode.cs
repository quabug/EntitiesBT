using EntitiesBT.Core;

namespace EntitiesBT.Nodes
{
    [BehaviorNode("B1C31393-4041-47E6-98CA-2F5E3BE4E1BD", BehaviorNodeType.Decorate)]
    public struct ErrorRerunOnceNode : INodeData
    {
        public NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb) where TNodeBlob : struct, INodeBlob where TBlackboard : struct, IBlackboard
        {
            var childState = index.TickChild(ref blob, ref bb);
            if (childState == 0)
            {
                index.ResetChildren(ref blob, ref bb);
                childState = index.TickChild(ref blob, ref bb);
            }
            return childState;
        }
    }
}
