using System;
using Blob;
using Nuwa.Blob;
using UnityEngine.InputSystem;

namespace EntitiesBT.Extensions.InputSystem
{
    public class InputActionGuidBuilder : Nuwa.Blob.Builder<Guid>
    {
        public InputActionReference InputAction;

        protected override void BuildImpl(IBlobStream stream, ref Guid value)
        {
            value = InputAction.action.id;
        }
    }

}