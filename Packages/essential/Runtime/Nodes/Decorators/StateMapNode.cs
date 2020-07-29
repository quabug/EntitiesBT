using EntitiesBT.Core;

namespace EntitiesBT.Nodes
{
    [BehaviorNode("95EF5B61-6DEA-4548-B43B-ACAFA92261C8", BehaviorNodeType.Decorate)]
    public struct StateMapNode : INodeData
    {
        public NodeState MapSuccess;
        public NodeState MapFailure;
        public NodeState MapRunning;
        public NodeState MapError;

        public NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb) where TNodeBlob : struct, INodeBlob where TBlackboard : struct, IBlackboard
        {
            var childState = index.TickChild(ref blob, ref bb);
            switch (childState)
            {
            case NodeState.Success: return MapSuccess;
            case NodeState.Failure: return MapFailure;
            case NodeState.Running: return MapRunning;
            default: return MapError;
            }
        }

        public void Reset<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb) where TNodeBlob : struct, INodeBlob where TBlackboard : struct, IBlackboard
        {
        }
    }
}
