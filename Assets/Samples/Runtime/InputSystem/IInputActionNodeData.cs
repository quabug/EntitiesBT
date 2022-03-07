using System;
using EntitiesBT.Core;

namespace EntitiesBT.Extensions.InputSystem
{
    public interface IInputActionNodeData : INodeData
    {
        Guid ActionId { get; set; }
    }
}
