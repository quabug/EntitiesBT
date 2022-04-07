using System;
using Blob;
using Nuwa.Blob;
using UnityEngine.InputSystem;

namespace EntitiesBT.Extensions.InputSystem
{
    public class InputActionGuidBuilder : PlainDataBuilder<Guid>
    {
        public InputActionReference InputAction;

        protected override void BuildImpl(IBlobStream stream, UnsafeBlobStreamValue<Guid> value)
        {
            value.Value = InputAction.action.id;
        }
    }

}