using System.Runtime.InteropServices;
using EntitiesBT.Core;
using EntitiesBT.Entities;

namespace EntitiesBT.Nodes
{
    [StructLayout(LayoutKind.Explicit)]
    [BehaviorNode("64E0DAFB-20E2-4DF4-910E-ADFA831DB8A9", BehaviorNodeType.Decorate)]
    public struct RunOnMainThreadNode : INodeData
    {
        public NodeState Tick(int index, INodeBlob blob, IBlackboard bb)
        {
            ref var behaviorTreeElement = ref bb.GetDataRef<BehaviorTreeBufferElement>();
            if (behaviorTreeElement.RuntimeThread == BehaviorTreeRuntimeThread.ForceJobThread
                || behaviorTreeElement.RuntimeThread == BehaviorTreeRuntimeThread.ForceMainThread)
                return blob.TickChildrenReturnFirstOrDefault(index, bb);
            
            if (behaviorTreeElement.RuntimeThread == BehaviorTreeRuntimeThread.JobThread)
            {
                behaviorTreeElement.RuntimeThread = BehaviorTreeRuntimeThread.MainThread;
                return NodeState.Running;
            }
            var state = blob.TickChildrenReturnFirstOrDefault(index, bb);
            if (state != NodeState.Running) behaviorTreeElement.RuntimeThread = BehaviorTreeRuntimeThread.JobThread;
            return state;
        }

        public void Reset(int index, INodeBlob blob, IBlackboard blackboard)
        {
        }
    }
}
