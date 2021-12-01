using System;
using EntitiesBT.Variant;
using Nuwa;
using UnityEngine;

namespace EntitiesBT
{
    public class ScopeReaderNode : VariantNode
    {
        public override string Name => $"ScopeReader<{Value?.FindValueType()?.Name}>";
        protected override IVariant Variant => Value;
        protected override Type DefaultVariantType => typeof(IVariantReader);

        [SerializeReference]
        [SerializeReferenceDrawer(TypeRestrictBySiblingTypeName = nameof(_baseTypeName), RenamePatter = @"^.*(\.|\+|/)(\w+)$||$2", Nullable = false)]
        public IVariantReader Value;

        private string _baseTypeName => _baseType.AssemblyQualifiedName;
        private Type _baseType => _valueType == null ? typeof(ScopeComponentVariant.Reader<>) : typeof(ScopeComponentVariant.Reader<>).MakeGenericType(_valueType);

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