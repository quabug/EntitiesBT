using System;
using EntitiesBT.Variant;

namespace EntitiesBT.Editor
{
    internal static class Utilities
    {
        public const string VariantPortClass = "variant";

        public static string VariantAccessMode(Type variantType)
        {
            if (typeof(IVariantReader).IsAssignableFrom(variantType)) return "ReadOnly";
            if (typeof(IVariantWriter).IsAssignableFrom(variantType)) return "WriteOnly";
            if (typeof(IVariantReaderAndWriter).IsAssignableFrom(variantType)) return "ReadWrite";
            throw new ArgumentException();
        }
    }
}