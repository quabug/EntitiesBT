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
        protected override string Name => $"Local<{Reader?.FindValueType()?.Name}>";
        public override Type VariantType => Reader?.GetType();

        [SerializeReference]
        [SerializeReferenceDrawer(TypeRestrictBySiblingTypeName = nameof(_baseTypeName), RenamePatter = @"^.*(\.|\+|/)(\w+)$||$2", Nullable = false)]
        public IVariantReader Reader;

        public IVariantReaderAndWriter ReaderAndWriter;

        public override IReadOnlyList<IVariant> Variants => new IVariant[] { Reader, ReaderAndWriter };

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

        public LocalVariantNodeA A;
    }

    public class VariantPortAttribute : Attribute
    {
        public string Name { get; }
    }

    [Serializable]
    public class LocalVariantNodeA
    {
        [VariantPort]
        [SerializeReference]
        [SerializeReferenceDrawer(RenamePatter = @"^.*(\.|\+|/)(\w+)$||$2", Nullable = false)]
        public IVariantReader Reader;

        [VariantPort]
        [SerializeReference]
        [SerializeReferenceDrawer(RenamePatter = @"^.*(\.|\+|/)(\w+)$||$2", Nullable = false)]
        public IVariantReaderAndWriter ReaderAndWriter;
    }
}