using System;
using EntitiesBT.Variant;
using Unity.Entities;
using UnityEngine;

namespace EntitiesBT
{
    [DisallowMultipleComponent]
    [ExecuteAlways]
    public abstract class VariantNode : MonoBehaviour
    {
        protected abstract string Name { get; }
        public abstract Type VariantType { get; }

        public abstract IntPtr Allocate(ref BlobBuilder builder, ref BlobVariant blobVariant);
        public virtual void OnConnected(GraphNodeVariant.Any graphNodeVariant) {}
        public virtual void OnDisconnected(GraphNodeVariant.Any graphNodeVariant) {}

        private void Update()
        {
            name = Name;
        }
    }
}