using System;
using EntitiesBT.Variant;
using Nuwa;
using Unity.Entities;
using UnityEngine;

namespace EntitiesBT
{
    public class LocalVariantReader : VariantNode
    {
        protected override string Name => $"Local<{Variant?.FindValueType()?.Name}>";
        public override Type VariantType => Variant?.GetType();

        [SerializeReference]
        [SerializeReferenceDrawer(TypeRestrict = typeof(LocalVariant.Reader<>), RenamePatter = @"^.*(\.|\+|/)(\w+)$||$2", Nullable = false)]
        public IVariantReader Variant;

        public override bool IsValid()
        {
            return Variant.HasSameTypeWith(GraphNodeVariant);
        }

        public override IntPtr Allocate(ref BlobBuilder builder, ref BlobVariant blobVariant)
        {
            return Variant.Allocate(ref builder, ref blobVariant);
        }

    }
}