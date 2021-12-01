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
        public Type VariantType => Variant?.GetType() ?? DefaultVariantType;
        public abstract string Name { get; }
        protected abstract IVariant Variant { get; }
        protected abstract Type DefaultVariantType { get; }

        public virtual void OnConnected(GraphNodeVariant.Any graphNodeVariant) {}
        public virtual void OnDisconnected(GraphNodeVariant.Any graphNodeVariant) {}

        public virtual IntPtr Allocate(ref BlobBuilder builder, ref BlobVariant blobVariant)
        {
            return Variant.Allocate(ref builder, ref blobVariant);
        }

        private void Update()
        {
            name = Name;
        }
    }
}