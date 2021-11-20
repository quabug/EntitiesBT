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
        [SerializeReferenceDrawer(TypeRestrictBySiblingTypeName = nameof(_baseTypeName), RenamePatter = @"^.*(\.|\+|/)(\w+)$||$2", Nullable = false)]
        public IVariantReader Variant;

        private string _baseTypeName => _baseType.AssemblyQualifiedName;
        private Type _baseType => _valueType == null ? typeof(LocalVariant.Reader<>) : typeof(LocalVariant.Reader<>).MakeGenericType(_valueType);

        public override IntPtr Allocate(ref BlobBuilder builder, ref BlobVariant blobVariant)
        {
            return Variant.Allocate(ref builder, ref blobVariant);
        }

        private GraphNodeVariant.Any _graphNodeVariant;
        private Type _valueType => _graphNodeVariant?.FindValueType();

        public override void OnConnected(GraphNodeVariant.Any graphNodeVariant)
        {
            _graphNodeVariant = graphNodeVariant;
        }

        public override void OnDisconnected(GraphNodeVariant.Any graphNodeVariant)
        {
            if (graphNodeVariant == _graphNodeVariant) _graphNodeVariant = null;
        }
    }
}