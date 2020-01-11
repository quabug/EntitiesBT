using System.Linq;
using EntitiesBT.Core;
using EntitiesBT.Entities;
using Unity.Entities;

namespace EntitiesBT.Nodes
{
    [BehaviorNode("64E0DAFB-20E2-4DF4-910E-ADFA831DB8A9", BehaviorNodeType.Decorate)]
    public class RunOnMainThreadNode
    {
        public static readonly ComponentType[] Types = { ComponentType.ReadWrite<IsRunOnMainThread>() };

        public static NodeState Tick(int index, INodeBlob blob, IBlackboard bb)
        {
            if (!bb.HasData<IsRunOnMainThread>())
                return blob.TickChildren(index, bb).FirstOrDefault();
            
            ref var isRunOnMainThread = ref bb.GetDataRef<IsRunOnMainThread>();
            if (!isRunOnMainThread.Value)
            {
                isRunOnMainThread.Value = true;
                return NodeState.Running;
            }
            var state = blob.TickChildren(index, bb).FirstOrDefault();
            if (state != NodeState.Running) isRunOnMainThread.Value = false;
            return state;
        }
    }
}
