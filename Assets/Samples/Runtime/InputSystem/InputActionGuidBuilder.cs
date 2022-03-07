using System;
using Nuwa.Blob;
using Unity.Entities;
using UnityEngine.InputSystem;

namespace EntitiesBT.Extensions.InputSystem
{
    public class InputActionGuidBuilder : PlainDataBuilder<Guid>
    {
        public InputActionReference InputAction;

        public override void Build(BlobBuilder builder, ref Guid data)
        {
            data = InputAction.action.id;
        }
    }

}