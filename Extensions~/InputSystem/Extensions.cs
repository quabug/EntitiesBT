using EntitiesBT.Core;
using UnityEngine.InputSystem;

namespace EntitiesBT.Extensions.InputSystem
{
    public static class InputExtensions
    {
        public static InputAction GetInputAction<TNodeData, TNodeBlob, TBlackboard>(this int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
            where TNodeData : struct, IInputActionNodeData
        {
            var input = bb.GetObject<InputActionAssetComponent>().Value;
            var data = blob.GetNodeData<TNodeData, TNodeBlob>(index);
            return input.FindAction(data.ActionId);
        }
        
        public static InputActionMap GetInputActionMap<TNodeData, TNodeBlob, TBlackboard>(this int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
            where TNodeData : struct, IInputActionNodeData
        {
            var input = bb.GetObject<InputActionAssetComponent>().Value;
            var data = blob.GetNodeData<TNodeData, TNodeBlob>(index);
            return input.FindActionMap(data.ActionId);
        }

        public static object ReadInputActionValueAsObject<TNodeData, TNodeBlob, TBlackboard>(this int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
            where TNodeData : struct, IInputActionNodeData
        {
            var action = index.GetInputAction<TNodeData, TNodeBlob, TBlackboard>(ref blob, ref bb);
            return action?.ReadValueAsObject();
        }
        
        public static TValue? ReadInputActionValue<TNodeData, TValue, TNodeBlob, TBlackboard>(this int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
            where TValue : struct
            where TNodeData : struct, IInputActionNodeData
        {
            var action = index.GetInputAction<TNodeData, TNodeBlob, TBlackboard>(ref blob, ref bb);
            return action?.ReadValue<TValue>();
        }
        
        public static bool IsInputActionTriggered<TNodeData, TNodeBlob, TBlackboard>(this int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
            where TNodeData : struct, IInputActionNodeData
        {
            var action = index.GetInputAction<TNodeData, TNodeBlob, TBlackboard>(ref blob, ref bb);
            return action != null && action.triggered;
        }
    }
}
