using System;
using System.Collections.Generic;
using EntitiesBT.Variant;
using Nuwa;
using Unity.Entities;
using UnityEngine;

namespace EntitiesBT
{
    public class LocalVariantNode : VariantNode
    {
        public override string Name => $"Local<{Reader?.FindValueType()?.Name}>";
        public override IVariant Variant => Reader;

        [SerializeReference]
        [SerializeReferenceDrawer(TypeRestrictBySiblingTypeName = nameof(_baseTypeName), RenamePatter = @"^.*(\.|\+|/)(\w+)$||$2", Nullable = false)]
        public IVariantReader Reader;

        private string _baseTypeName => _baseType.AssemblyQualifiedName;
        private Type _baseType => _valueType == null ? typeof(LocalVariant.Reader<>) : typeof(LocalVariant.Reader<>).MakeGenericType(_valueType);

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