using EntitiesBT.Core;
using Unity.Entities;

namespace EntitiesBT.Nodes
{
    [BehaviorNode("64E0DAFB-20E2-4DF4-910E-ADFA831DB8A9", BehaviorNodeType.Decorate)]
    public class RunOnMainThreadNode
    {
        public static ComponentType[] Types => new [] { ComponentType.ReadWrite<IsRunOnMainThread>() };

        public static NodeState Tick(int index, INodeBlob blob, IBlackboard bb)
        {
            if (!bb.HasData<IsRunOnMainThread>()) return RunChildNode();
            ref var isRunOnMainThread = ref bb.GetDataRef<IsRunOnMainThread>();
            if (!isRunOnMainThread.Value)
            {
                isRunOnMainThread.Value = true;
                return NodeState.Running;
            }
            var state = RunChildNode();
            if (state != NodeState.Running) isRunOnMainThread.Value = false;
            return state;

            NodeState RunChildNode()
            {
                var childIndex = index + 1;
                var childState = NodeState.Failure;
                if (childIndex < blob.GetEndIndex(index))
                    childState = VirtualMachine.Tick(childIndex, blob, bb);
                return childState;
            }
        }
    }
}
