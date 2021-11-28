using System;
using System.Collections.Generic;
using EntitiesBT.Variant;
using Unity.Entities;
using UnityEngine;

namespace EntitiesBT
{
    [DisallowMultipleComponent]
    [ExecuteAlways]
    public abstract class VariantNode : MonoBehaviour
    {
        public abstract IVariant Variant { get; }
        public abstract string Name { get; }

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