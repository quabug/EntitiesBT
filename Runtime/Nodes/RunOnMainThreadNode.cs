using System.Linq;
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
            if (bb.HasData<ForceRunOnMainThreadTag>() || bb.HasData<ForceRunOnJobTag>())
                return blob.TickChildren(index, bb).FirstOrDefault();
            
            var isRunOnMainThread = bb.HasData<RunOnMainThreadTag>();
            if (!isRunOnMainThread)
            {
                bb.GetData<IEntityCommand>().AddComponent<RunOnMainThreadTag>();
                return NodeState.Running;
            }
            var state = blob.TickChildren(index, bb).FirstOrDefault();
            if (state != NodeState.Running) bb.GetData<IEntityCommand>().RemoveComponent<RunOnMainThreadTag>();
            return state;
        }

        public void Reset(int index, INodeBlob blob, IBlackboard blackboard)
        {
        }
    }
}
