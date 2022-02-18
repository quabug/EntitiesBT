using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesBT.Variant;
using GraphExt;
using Nuwa;
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

        public Type VariantType => ConnectedVariants.Any() && Variant != null ? Variant.GetType() : DefaultVariantType;
        public virtual string Name => $"{VariantTypeName}{AccessName}<{Variant?.FindValueType()?.Name}>";
        public abstract IVariant Variant { get; set; }
        public Type ValueType => ConnectedVariants.FirstOrDefault()?.ValueType;
        public readonly ISet<GraphNodeVariant.Any> ConnectedVariants = new System.Collections.Generic.HashSet<GraphNodeVariant.Any>();
        public Type BaseType => ValueType == null ? BaseVariantGenericType : BaseVariantGenericType.MakeGenericType(ValueType);

        protected abstract string VariantTypeName { get; }
        protected abstract Type DefaultVariantType { get; }
        protected abstract Type BaseVariantGenericType { get; }

        protected string BaseTypeName => BaseType.AssemblyQualifiedName;

        private string AccessName
        {
            get
            {
                if (typeof(IVariantReader).IsAssignableFrom(DefaultVariantType)) return "RO";
                if (typeof(IVariantWriter).IsAssignableFrom(DefaultVariantType)) return "WO";
                if (typeof(IVariantReaderAndWriter).IsAssignableFrom(DefaultVariantType)) return "RW";
                throw new ArgumentException();
            }
        }

        public IntPtr Allocate(ref BlobBuilder builder, ref BlobVariant blobVariant)
        {
            return Variant.Allocate(ref builder, ref blobVariant);
        }

        public bool IsPortCompatible(GraphRuntime<VariantNode> graph, in PortId input, in PortId output) => true;
        public void OnConnected(GraphRuntime<VariantNode> graph, in PortId input, in PortId output) {}
        public void OnDisconnected(GraphRuntime<VariantNode> graph, in PortId input, in PortId output) {}
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