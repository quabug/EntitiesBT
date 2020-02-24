using EntitiesBT.Components;
using EntitiesBT.Core;
using Unity.Entities;
using UnityEngine;

namespace EntitiesBT.Extensions.InputSystem
{
    public class BTInputEnableDisable : BTNode
    {
        [SerializeField] private bool _enable = true;

        public override BehaviorNodeType NodeType => _enable
            ? typeof(InputEnableNode).GetBehaviorNodeAttribute().Type
            : typeof(InputDisableNode).GetBehaviorNodeAttribute().Type
        ;

        public override int NodeId => _enable
            ? typeof(InputEnableNode).GetBehaviorNodeAttribute().Id
            : typeof(InputDisableNode).GetBehaviorNodeAttribute().Id
        ;

        public override int Size => 0;
        public override unsafe void Build(void* dataPtr, ITreeNode<INodeDataBuilder>[] builders) {}
    }

    [BehaviorNode("3B46A529-3BAF-40E8-B249-CB1D35FACCF0")]
    public struct InputEnableNode : INodeData
    {
        public static readonly ComponentType[] Types =
        {
            ComponentType.ReadWrite<InputActionAssetComponent>()
        };
        
        public static NodeState Tick(int index, INodeBlob blob, IBlackboard bb)
        {
            bb.GetData<InputActionAssetComponent>().Value.Enable();
            return NodeState.Success;
        }
    }
    
    [BehaviorNode("03D959C3-D9F6-478D-A5E5-DCA4ADBD6C3D")]
    public struct InputDisableNode : INodeData
    {
        public static readonly ComponentType[] Types =
        {
            ComponentType.ReadWrite<InputActionAssetComponent>()
        };
        
        public static NodeState Tick(int index, INodeBlob blob, IBlackboard bb)
        {
            bb.GetData<InputActionAssetComponent>().Value.Enable();
            return NodeState.Success;
        }
    }
}
