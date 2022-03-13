using System.Runtime.InteropServices;
using EntitiesBT.Core;

namespace EntitiesBT.Extensions.InputSystem
{
    [BehaviorNode("3B46A529-3BAF-40E8-B249-CB1D35FACCF0"), StructLayout(LayoutKind.Explicit)]
    public struct InputEnableNode : INodeData
    {
        public NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            bb.GetObject<InputActionAssetComponent>().Value.Enable();
            return NodeState.Success;
        }
    }
    
    [BehaviorNode("03D959C3-D9F6-478D-A5E5-DCA4ADBD6C3D"), StructLayout(LayoutKind.Explicit)]
    public struct InputDisableNode : INodeData
    {
        public NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            bb.GetObject<InputActionAssetComponent>().Value.Enable();
            return NodeState.Success;
        }
    }
}
