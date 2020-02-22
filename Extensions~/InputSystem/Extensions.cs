using EntitiesBT.Core;
using UnityEngine.InputSystem;

namespace EntitiesBT.Extensions.InputSystem
{
    public static class InputExtensions
    {
        public static InputAction GetInputAction<TNodeData>(this IBlackboard bb, int index, INodeBlob blob)
            where TNodeData : struct, IInputActionNodeData
        {
            var input = bb.GetData<InputActionAssetComponent>().Value;
            var data = blob.GetNodeData<TNodeData>(index);
            return input.FindAction(data.ActionId);
        }
        
        public static InputActionMap GetInputActionMap<TNodeData>(this IBlackboard bb, int index, INodeBlob blob)
            where TNodeData : struct, IInputActionNodeData
        {
            var input = bb.GetData<InputActionAssetComponent>().Value;
            var data = blob.GetNodeData<TNodeData>(index);
            return input.FindActionMap(data.ActionId);
        }

        public static object ReadInputActionValueAsObject<TNodeData>(this IBlackboard bb, int index, INodeBlob blob)
            where TNodeData : struct, IInputActionNodeData
        {
            var action = bb.GetInputAction<TNodeData>(index, blob);
            return action?.ReadValueAsObject();
        }
        
        public static TValue? ReadInputActionValue<TNodeData, TValue>(this IBlackboard bb, int index, INodeBlob blob)
            where TValue : struct
            where TNodeData : struct, IInputActionNodeData
        {
            var action = bb.GetInputAction<TNodeData>(index, blob);
            return action?.ReadValue<TValue>();
        }
        
        public static bool IsInputActionTriggered<TNodeData>(this IBlackboard bb, int index, INodeBlob blob)
            where TNodeData : struct, IInputActionNodeData
        {
            var action = bb.GetInputAction<TNodeData>(index, blob);
            return action != null && action.triggered;
        }
    }
}
