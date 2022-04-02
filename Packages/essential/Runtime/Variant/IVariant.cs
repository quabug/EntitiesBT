using System;
using Blob;
using EntitiesBT.Core;
using JetBrains.Annotations;
using Unity.Entities;

namespace EntitiesBT.Variant
{
    public interface IVariant
    {
        void Allocate(IBlobStream stream);
        object PreviewValue { get; }
    }

    public interface IVariantReader : IVariant {}
    public interface IVariantWriter : IVariant {}
    public interface IVariantReaderAndWriter : IVariant {}
    public interface IVariant<out T> : IVariant where T : unmanaged {}
    public interface IVariantReader<out T> : IVariantReader, IVariant<T> where T : unmanaged {}
    public interface IVariantWriter<out T> : IVariantWriter, IVariant<T> where T : unmanaged {}
    public interface IVariantReaderAndWriter<out T> : IVariantReaderAndWriter, IVariant<T> where T : unmanaged {}

    public static partial class VariantExtension
    {
        [CanBeNull] public static Type FindValueType([NotNull] this IVariant variant)
        {
            var type = variant.GetType();
            return type.GetInterface(typeof(IVariant<int>).Name)?.GenericTypeArguments[0];
        }

        public static bool HasSameTypeWith(this IVariant lhs, IVariant rhs)
        {
            return lhs != null && rhs != null &&
                   lhs.FindValueType() == rhs.FindValueType() &&
                   (
                       lhs is IVariantReader && rhs is IVariantReader ||
                       lhs is IVariantWriter && rhs is IVariantWriter ||
                       lhs is IVariantReaderAndWriter && rhs is IVariantReaderAndWriter
                   )
            ;
        }


        public static bool IsReadOnly(this IVariant variant) => variant is IVariantReader;
        public static bool IsReadWrite(this IVariant variant) => variant is IVariantReaderAndWriter;
        public static bool IsWriteOnly(this IVariant variant) => variant is IVariantWriter;
    }
}
