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
    public abstract class VariantNode : INode<GraphRuntime<VariantNode>>
    {
        [NodeProperty(Name = "Preview")] public object PreviewValue => Variant.PreviewValue;

        public Type VariantType => ConnectedVariants.Any() && Variant != null ? Variant.GetType() : DefaultVariantType;
        public virtual string Name => $"{VariantTypeName}{AccessName}<{Variant?.FindValueType()?.Name}>";
        protected abstract string VariantTypeName { get; }
        protected abstract IVariant Variant { get; set; }
        protected abstract Type DefaultVariantType { get; }
        protected abstract Type BaseVariantGenericType { get; }

        protected string BaseTypeName => BaseType.AssemblyQualifiedName;
        protected Type BaseType => ValueType == null ? BaseVariantGenericType : BaseVariantGenericType.MakeGenericType(ValueType);
        protected Type ValueType => ConnectedVariants.FirstOrDefault()?.ValueType;

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

        protected readonly ISet<GraphNodeVariant.Any> ConnectedVariants = new System.Collections.Generic.HashSet<GraphNodeVariant.Any>();

        public IntPtr Allocate(ref BlobBuilder builder, ref BlobVariant blobVariant)
        {
            return Variant.Allocate(ref builder, ref blobVariant);
        }

        public bool IsPortCompatible(GraphRuntime<VariantNode> graph, in PortId input, in PortId output)
        {
            return true;
        }

        public void OnConnected(GraphRuntime<VariantNode> graph, in PortId input, in PortId output)
        {
//             var valueType = graphNodeVariant.ValueType;
//             if (ValueType != null && valueType != ValueType) throw new ArgumentException($"invalid variant value type {valueType.FullName}, expect {ValueType.FullName}");
//             if (!ConnectedVariants.Contains(graphNodeVariant)) ConnectedVariants.Add(graphNodeVariant);
//
// #if UNITY_EDITOR
//             if (Variant == null || Variant.FindValueType() != valueType)
//             {
//                 try
//                 {
//                     var variantType = UnityEditor.TypeCache.GetTypesDerivedFrom(BaseType).First();
//                     Variant = (IVariant)Activator.CreateInstance(variantType);
//                 }
//                 catch
//                 {
//                     // ignored
//                 }
//             }
// #endif
        }

        public void OnDisconnected(GraphRuntime<VariantNode> graph, in PortId input, in PortId output)
        {
            // ConnectedVariants.Remove(graphNodeVariant);
        }
    }

    public abstract class VariantNode<T> : VariantNode where T : IVariant
    {
        protected override IVariant Variant
        {
            get => Value;
            set => Value = (T)value;
        }

        protected override Type DefaultVariantType => typeof(T);

        // [NodeProperty(OutputPort = )]
        // [SerializeReference]
        // [SerializeReferenceDrawer(TypeRestrictBySiblingTypeName = nameof(BaseTypeName), RenamePatter = @"^.*(\.|\+|/)(\w+)$||$2", Nullable = false)]
        public T Value;
    }
}