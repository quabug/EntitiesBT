using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesBT.Variant;
using JetBrains.Annotations;
using Nuwa;
using Unity.Entities;
using UnityEngine;

namespace EntitiesBT
{
    [DisallowMultipleComponent]
    [ExecuteAlways]
    public abstract class VariantNode : MonoBehaviour
    {
        public Type VariantType => ConnectedVariants.Any() && Variant != null ? Variant.GetType() : DefaultVariantType;
        public virtual string Name => $"{VariantTypeName}{AccessName}<{ValueType?.Name}>";
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

        protected readonly ISet<GraphNodeVariant.Any> ConnectedVariants = new HashSet<GraphNodeVariant.Any>();

        public virtual void OnConnected([NotNull] GraphNodeVariant.Any graphNodeVariant)
        {
            var valueType = graphNodeVariant.ValueType;
            if (ValueType != null && valueType != ValueType) throw new ArgumentException($"invalid variant value type {valueType.FullName}, expect {ValueType.FullName}");
            if (!ConnectedVariants.Contains(graphNodeVariant)) ConnectedVariants.Add(graphNodeVariant);

#if UNITY_EDITOR
            if (Variant == null || Variant.FindValueType() != valueType)
            {
                try
                {
                    var variantType = UnityEditor.TypeCache.GetTypesDerivedFrom(BaseType).First();
                    Variant = (IVariant)Activator.CreateInstance(variantType);
                }
                catch
                {
                    // ignored
                }
            }
#endif
        }

        public virtual void OnDisconnected([NotNull] GraphNodeVariant.Any graphNodeVariant)
        {
            ConnectedVariants.Remove(graphNodeVariant);
        }

        public IntPtr Allocate(ref BlobBuilder builder, ref BlobVariant blobVariant)
        {
            return Variant.Allocate(ref builder, ref blobVariant);
        }

        private void Update()
        {
            name = Name;
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

        [SerializeReference]
        [SerializeReferenceDrawer(TypeRestrictBySiblingTypeName = nameof(BaseTypeName), RenamePatter = @"^.*(\.|\+|/)(\w+)$||$2", Nullable = false)]
        public T Value;
    }
}