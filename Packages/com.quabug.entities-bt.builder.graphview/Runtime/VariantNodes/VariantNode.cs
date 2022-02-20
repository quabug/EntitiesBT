using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesBT.Editor;
using EntitiesBT.Variant;
using GraphExt;
using Unity.Entities;
using UnityEngine;

namespace EntitiesBT
{
    [Serializable]
    public abstract class VariantNode : INode<GraphRuntime<VariantNode>>
    {
        public const string INPUT_PORT = "input";
        public const string OUTPUT_PORT = "output";

        public object PreviewValue => Variant.PreviewValue;
        public abstract IVariant Variant { get; set; }

        protected abstract string VariantTypeName { get; }
        protected abstract Type DefaultVariantType { get; }
        protected abstract Type BaseVariantGenericType { get; }

        public readonly ISet<GraphNodeVariant.Any> ConnectedVariants = new System.Collections.Generic.HashSet<GraphNodeVariant.Any>();
        public Type VariantType => ConnectedVariants.Any() && Variant != null ? Variant.GetType() : DefaultVariantType;
        public Type ValueType => ConnectedVariants.FirstOrDefault()?.ValueType;
        public Type BaseType => ValueType == null ? BaseVariantGenericType : BaseVariantGenericType.MakeGenericType(ValueType);

        public string Name => $"{VariantTypeName}{AccessName}<{Variant?.FindValueType()?.Name}>";
        public string BaseTypeName => BaseType.AssemblyQualifiedName;
        public string AccessName => GetVariantAccessName(DefaultVariantType);

        public IntPtr Allocate(ref BlobBuilder builder, ref BlobVariant blobVariant)
        {
            return Variant.Allocate(ref builder, ref blobVariant);
        }

        public bool IsPortCompatible(GraphRuntime<VariantNode> graph, in PortId input, in PortId output) => true;
        public void OnConnected(GraphRuntime<VariantNode> graph, in PortId input, in PortId output) {}
        public void OnDisconnected(GraphRuntime<VariantNode> graph, in PortId input, in PortId output) {}

        public static string GetVariantAccessName(Type variantType)
        {
            if (typeof(IVariantReader).IsAssignableFrom(variantType)) return "RO";
            if (typeof(IVariantWriter).IsAssignableFrom(variantType)) return "WO";
            if (typeof(IVariantReaderAndWriter).IsAssignableFrom(variantType)) return "RW";
            throw new ArgumentException();
        }
    }

    public abstract class VariantNode<T> : VariantNode where T : IVariant
    {
        public override IVariant Variant
        {
            get => Value;
            set => Value = (T)value;
        }

        protected override Type DefaultVariantType => typeof(T);

        [SerializeReference]
        [Nuwa.SerializeReferenceDrawer(TypeRestrictBySiblingTypeName = nameof(BaseTypeName), RenamePatter = @"^.*(\.|\+|/)(\w+)$||$2", Nullable = false)]
        public T Value;
    }
}