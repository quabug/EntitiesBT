using System;
using System.Collections.Generic;
using EntitiesBT.Variant;
using Unity.Entities;
using UnityEngine;

namespace EntitiesBT
{
    public interface IVariantNode
    {
        IReadOnlyList<IVariant> Variants { get; }
    }

    public static class VariantNodeExtension
    {
        internal static IntPtr Allocate(this IVariantNode node, ref BlobBuilder builder, ref BlobVariant blobVariant, int variantIndex)
        {
            return node.Variants[variantIndex].Allocate(ref builder, ref blobVariant);
        }
    }

    [DisallowMultipleComponent]
    [ExecuteAlways]
    public abstract class VariantNode : MonoBehaviour, IVariantNode
    {
        protected abstract string Name { get; }
        public abstract Type VariantType { get; }

        public virtual void OnConnected(GraphNodeVariant.Any graphNodeVariant) {}
        public virtual void OnDisconnected(GraphNodeVariant.Any graphNodeVariant) {}

        private void Update()
        {
            name = Name;
        }

        public abstract IReadOnlyList<IVariant> Variants { get; }
    }
}